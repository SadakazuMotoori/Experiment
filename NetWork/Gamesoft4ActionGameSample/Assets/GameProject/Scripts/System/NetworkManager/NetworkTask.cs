using System.Collections.Generic;
using MessagePack;
using Photon.Pun;
using UnityEngine;

namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        // 送信データを作成する
        public void CreateSendData<T>(ENETWORK_COMMAND aCmd, RpcTarget aTarget, T aData)
        {
            var serialized = MessagePackSerializer.Serialize(aData);

            m_View.RPC(nameof(RpcSendMessage), aTarget, aCmd, serialized);
        }

        // 受信データ解析実行
        private void AnalyzeNetworkData(stReceiveData aData)
        {
            switch (aData.cmd)
            {
                // クライアント限定
                case ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stPlayerInfo>(aData.data);
                    m_PlayerInfo.playerlist.Clear();
                    m_PlayerInfo.playerlist = _deserialized.playerlist;
                    m_View.ViewID           = _deserialized.viewid;
                    for (short i = 0; i < m_PlayerInfo.playerlist.Count; i++)
                    {
                        Debug.Log("PlayerName = " + m_PlayerInfo.playerlist[i].playername + " id = "+ m_PlayerInfo.playerlist[i].playerid);
                    }

                    AppManager.Instance.ChangeScene("WaitRoomScene");
                }
                break;

                // 全員対象
                case ENETWORK_COMMAND.CMD_GAMESTARTCOUNTDOWN:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<long>(aData.data);
                    SetVersatileCount(_deserialized);
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
                    GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid).SetMove(_deserialized.axis, _deserialized.attack, _deserialized.isgrounded, _deserialized.isdied);
                //  OnNetworkUpdate(_deserialized.axis, _deserialized.attack, _deserialized.isgrounded, _deserialized.isdied);
                }
                break;

                case ENETWORK_COMMAND.CMD_INPUT:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<string>(aData.data);
                    Debug.Log("なんか入力したね！？ = " + _deserialized);
                }
                break;

                case ENETWORK_COMMAND.CMD_SYNCPOS:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stSyncPos>(aData.data);
                    GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid).SetSyncPos(_deserialized.pos);
                }
                break;

                case ENETWORK_COMMAND.CMD_SYNCKEY:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stSyncKey>(aData.data);
                    GameSceneManager.Instance.GetCharacterBrain(_deserialized.playerid).SetSyncKey(_deserialized.key);
                }
                break;
            }
        }
    }
}