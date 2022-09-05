using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePack;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        private PhotonView          m_View;
        private List<stPlayerData>  m_PlayerList;
        public const int NETWORK_TIME_OUT_WAIT = 10000; // タイムアウト時間(ms)

        // 受信データ
        private struct stReceiveData
        {
            public ENETWORK_COMMAND cmd     { get; set; }
            public byte[]           data    { get; set; }
        }
        // ------------------------------------------------

        private bool                    m_IsInitialized;
        private ENETWORK_ERROR_CODE     m_LastError;
        private Queue<stReceiveData>    m_NetworkCmdList;
        // ------------------------------------------------

        // Start is called before the first frame update
        void Awake()
        {
            m_IsInitialized     = false;
            m_NetworkCmdList    = new Queue<stReceiveData>();
            m_NetworkCmdList.Clear();

            m_PlayerList        = new List<stPlayerData>();
            m_LastError         = ENETWORK_ERROR_CODE.ERR_NORE;
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsConnectedAndReady() || m_NetworkCmdList.Count == 0) return;

            stReceiveData _data = m_NetworkCmdList.Dequeue();
            AnalyzeNetworkData(_data);
        }
        // ------------------------------------------------

        private bool IsConnectedAndReady()
        {
            return (m_IsInitialized && PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.NetworkClientState.Equals(ClientState.JoiningLobby));
        }

        private async UniTask NetworkWait()
        {
            while(true)
            {
                // 準備完了していない、及び特定の動作中なら待機する
                if (IsConnectedAndReady()) break;

                await UniTask.Delay(10);
            }
        }

        private async UniTask<ENETWORK_ERROR_CODE> RequestConnect(string aName)
        {
            PhotonNetwork.LocalPlayer.NickName = aName;

            // Photonサーバーへ接続する
            Debug.Log("Connect to master");
            if (!PhotonNetwork.ConnectUsingSettings())
            {
                m_LastError = ENETWORK_ERROR_CODE.ERR_CONNECT;
            }
            await NetworkWait();

            // ロビーへ接続する
            Debug.Log("Connect to lobby");
            if (!PhotonNetwork.JoinLobby())
            {
                m_LastError = ENETWORK_ERROR_CODE.ERR_JOINLOBBY;
            }
            await NetworkWait();

            // ルームを作成/参加する
            CreateOrJoinDebugRoom();
            await NetworkWait();

            return m_LastError;
        }

        private ENETWORK_ERROR_CODE CreateOrJoinDebugRoom()
        {
            if (PhotonNetwork.CountOfRooms == 0)
            {
                if (!PhotonNetwork.CreateRoom("DEBUG ROOM", new RoomOptions(), TypedLobby.Default))
                {
                    m_LastError = ENETWORK_ERROR_CODE.ERR_CREATEROOM;
                }
            }
            else if (!PhotonNetwork.JoinRoom("DEBUG ROOM"))
            {
                m_LastError = ENETWORK_ERROR_CODE.ERR_JOINROOM;
            }
            return m_LastError;
        }
        // ------------------------------------------------

        public ENETWORK_ERROR_CODE GetNetworkLastError()
        {
            return m_LastError;
        }

        public async UniTask InitializeNetworkStatus(string aName)
        {
            if (m_IsInitialized) return;

            // ネットワーク初期化時にまずはサーバーへ接続
            if (!PhotonNetwork.IsConnected)
            {
                ENETWORK_ERROR_CODE _lasterr = await RequestConnect(aName);
                Debug.Log("last error = " + _lasterr);

                if(_lasterr != ENETWORK_ERROR_CODE.ERR_NORE) return;
            }
        }

        // ------------------------------------------------

        // マスターサーバーへ接続した
        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to master!");
            
            m_IsInitialized = true;
        }

        // ロビーに入室した
        public override void OnJoinedLobby()
        {
            Debug.Log("joined lobby!!!");
        }

        // ルームを作成した
        public override void OnCreatedRoom()
        {
            Debug.Log("create room!!!");
        }

        // ルームに入室した
        public override void OnJoinedRoom()
        {
            Debug.Log("joined room!!!");

            // 通信用ネットワークオブジェクトを作成
            m_View = this.GetComponent<PhotonView>();
            // VIEWIDを生成(今は決め打ち)
            m_View.ViewID = 1001;

            // プレイヤーデータを作成する
            stPlayerData _playerData = new stPlayerData();
            _playerData.name    = PhotonNetwork.LocalPlayer.NickName;
            _playerData.id      = PhotonNetwork.LocalPlayer.ActorNumber;

            // マスターならユーザーデータを追加する
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                m_PlayerList.Add(_playerData);
            }
            // クライアントなら自身のユーザーデータをマスターに送信して、リストを貰う
            else
            {
                CreateSendData(ENETWORK_COMMAND.CMD_JOINROOM, RpcTarget.MasterClient, _playerData);
            }
        }
        // ------------------------------------------------

        [PunRPC]
        private void RpcSendMessage(ENETWORK_COMMAND aCMD , byte[] aData)
        {
            stReceiveData _receiveData = new stReceiveData
            {
                cmd     = aCMD ,
                data    = aData
            };
            m_NetworkCmdList.Enqueue(_receiveData);
        }
    }
}
