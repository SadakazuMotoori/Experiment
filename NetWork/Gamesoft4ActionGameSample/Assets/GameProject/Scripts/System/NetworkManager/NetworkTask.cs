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
                // マスタークライアント限定
                case ENETWORK_COMMAND.CMD_JOINROOM:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<stPlayerData>(aData.data);
                    m_PlayerList.Add(_deserialized);

                    CreateSendData(ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST, RpcTarget.Others, 0);
                }
                break;

                // クライアント限定
                case ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST:
                {
                    m_PlayerList.Clear();
                    int index = 0;
                    foreach(var player in PhotonNetwork.PlayerList)
                    {
                        stPlayerData _player = new stPlayerData();
                        _player.name    = player.NickName;
                        _player.id      = index++;

                        m_PlayerList.Add(_player);
                    }
/*
                    var _deserialized = MessagePackSerializer.Deserialize<List<stPlayerData>>(aData.data);
                    m_PlayerList.Clear();
                    m_PlayerList = _deserialized;

                    for(short i = 0; i < m_PlayerList.Count; i++)
                    {
                        Debug.Log("PlayerName = " + m_PlayerList[i].name + " id = "+ m_PlayerList[i].id);
                    }
*/
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

                case ENETWORK_COMMAND.CMD_INPUT:
                {
                    var _deserialized = MessagePackSerializer.Deserialize<string>(aData.data);
                    Debug.Log("なんか入力したね！？ = " + _deserialized);
                }
                break;
            }
        }
    }
}