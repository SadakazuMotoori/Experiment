using System;
using Cysharp.Threading.Tasks;

using MessagePack;

#if USE_STEAMWORKS
using Steamworks;
#else
using Photon.Pun;
using Photon.Realtime;
#endif

namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        // 送信データを作成する
        public void CreateSendData<T>(ENETWORK_COMMAND aCmd, RpcTarget aTarget, T aData)
        {
            var serialized = MessagePackSerializer.Serialize(aData);

            m_View.RPC(nameof(RpcSendMessage), aTarget, aCmd, m_RoomID, serialized);
        }

        // 受信データ解析実行
        private async UniTask AnalyzeNetworkData(stReceiveData aData)
        {
            await NetworkWait();

            switch (aData.cmd)
            {
                // クライアント限定
                case ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST:
                {
                    var _deserialized   = MessagePackSerializer.Deserialize<stNetInfo>(aData.data);

                    m_NetInfo.infolist.Clear();
                    m_NetInfo.infolist  = _deserialized.infolist;

                    for(int i = 0; i < m_NetInfo.infolist.Count; i++)
                    {
                        if (GetMyPlayerName().Equals(m_NetInfo.infolist[i].name))
                        {
                            m_RoomID = i;
                            break;
                        }
                    }
#if USE_STEAMWORKS
using Steamworks;
#else
                    m_View.ViewID = 1001;
#endif
                }
                break;

                // 全員対象
                case ENETWORK_COMMAND.CMD_COUNTDOWN:
                {
                    var _deserialized   = MessagePackSerializer.Deserialize<int>(aData.data);

                    DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                    long _startTime     = (long)(DateTime.Now - _unixEpoch).TotalSeconds + 10;
                    SetVersatileCount(_startTime);
                }
                break;

                case ENETWORK_COMMAND.CMD_CREATECHARACTER:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stCreateCharacterParameter>(aData.data);
                    GameSceneManager.Instance.OnCreateCharacter(_deserialized);
                }
                break;

                case ENETWORK_COMMAND.CMD_CHARACTEMOVE:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stNetworkParameter>(aData.data);

                    Character.CharacterBrain _brain = GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid);
                    if (_brain)
                    {
                        _brain.SetMove(_deserialized.axis, _deserialized.attack, _deserialized.isgrounded, _deserialized.isdied);
                    }
                //  OnNetworkUpdate(_deserialized.axis, _deserialized.attack, _deserialized.isgrounded, _deserialized.isdied);
                }
                break;

                case ENETWORK_COMMAND.CMD_SYNCPOS:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stSyncPos>(aData.data);
                    
                    Character.CharacterBrain _brain = GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid);
                    if (_brain)
                    {
                        _brain.SetSyncPosAndHP(_deserialized.pos, _deserialized.hp);
                    }
                }
                break;

                case ENETWORK_COMMAND.CMD_SYNCKEY:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stSyncKey>(aData.data);

                    Character.CharacterBrain _brain = GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid);
                    if (_brain)
                    {
                        _brain.SetSyncKey(_deserialized.key);
                    }
                }
                break;

                case ENETWORK_COMMAND.CMD_SYNCATTACK:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stSyncAttack>(aData.data);

                    Character.CharacterBrain _brain = GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid);
                    if (_brain)
                    {
                        _brain.SetSyncAttack(_deserialized.attack);
                    }
                }
                break;

                case ENETWORK_COMMAND.CMD_SYNCDEAD:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<int>(aData.data);
                    Character.CharacterBrain _brain = GameSceneManager.Instance.GetCharacterBrain(_deserialized);
                    if (_brain)
                    {
                        _brain.SetSyncDead(true);
                    }
                }
                break;
            }
        }
    }
}