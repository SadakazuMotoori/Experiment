using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MessagePack;
using MessagePack.Resolvers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance { get; private set; }

        private PhotonView          m_View;
        private List<stPlayerData>  m_PlayerList;
        private int                 m_MyID = 0;
        private long                m_VersatileCount;
        public const int NETWORK_TIME_OUT_WAIT = 10000; // タイムアウト時間(ms)

        // 受信データ
        private struct stReceiveData
        {
            public ENETWORK_COMMAND cmd     { get; set; }
            public byte[]           data    { get; set; }
        }
        // ------------------------------------------------

        private bool                    m_IsInitializedSystem;
        private bool                    m_IsInitializedNetwork;
        private ENETWORK_ERROR_CODE     m_LastError;
        private Queue<stReceiveData>    m_NetworkCmdList;
        // ------------------------------------------------

        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null) return;

            Instance                = this;
            m_IsInitializedSystem   = false;
            m_IsInitializedNetwork  = false;

            // シリアライザの初期設定
            MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance, // コード生成した型解決クラス
                MessagePack.Unity.UnityResolver.Instance,
                MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );
            var option = MessagePack.MessagePackSerializerOptions.Standard
                .WithCompression(MessagePack.MessagePackCompression.Lz4BlockArray) // LZ4 圧縮利用
                .WithResolver(MessagePack.Resolvers.StaticCompositeResolver.Instance);
            MessagePack.MessagePackSerializer.DefaultOptions = option;
        }

        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        //    Debug.Log("IsConnectedAndReady() = "+ IsConnectedAndReady());
            if (!IsConnectedAndReady() || m_NetworkCmdList.Count == 0) return;

            stReceiveData _data = m_NetworkCmdList.Dequeue();
            AnalyzeNetworkData(_data);
        }
        // ------------------------------------------------

        private bool IsConnectedAndReady()
        {
            return (m_IsInitializedNetwork && PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.NetworkClientState.Equals(ClientState.JoiningLobby));
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
            // マスターならルームを作成する
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

        public void InitializeNetworkManager()
        {
            if (m_IsInitializedSystem) return;

            m_IsInitializedSystem   = true;
            m_NetworkCmdList        = new Queue<stReceiveData>();
            m_NetworkCmdList.Clear();

            m_PlayerList            = new List<stPlayerData>();
            m_LastError             = ENETWORK_ERROR_CODE.ERR_NORE;
        }

        public List<stPlayerData> GetPlayerList()
        {
            return m_PlayerList;
        }

        public int GetMemberNum()
        {
            return m_PlayerList.Count;
        }

        public string GetMemberName(int index = 0)
        {
            if (index >= m_PlayerList.Count) return "";

            return m_PlayerList[index].name;
        }

        public string GetMyName()
        {
            if (m_MyID >= m_PlayerList.Count) return "";

            return m_PlayerList[m_MyID].name;
        }

        public void SetPlayerName(string aPlayerName)
        {
            PhotonNetwork.LocalPlayer.NickName = aPlayerName;
        }

        public ENETWORK_ERROR_CODE GetNetworkLastError()
        {
            return m_LastError;
        }

        public async UniTask InitializeNetworkStatus(string aName)
        {
            if (m_IsInitializedNetwork) return;

            // ネットワーク初期化時にまずはサーバーへ接続
            if (!PhotonNetwork.IsConnected)
            {
                ENETWORK_ERROR_CODE _lasterr = await RequestConnect(aName);
                Debug.Log("last error = " + _lasterr);

                if(_lasterr != ENETWORK_ERROR_CODE.ERR_NORE) return;
            }
        }

        public void GameStartNotification(long aStartTime)
        {
            m_VersatileCount = 0;
            CreateSendData(ENETWORK_COMMAND.CMD_GAMESTARTCOUNTDOWN, RpcTarget.All, aStartTime);
        }

        public long GetVersatileCount()
        {
            return m_VersatileCount;
        }
        public void SetVersatileCount(long aCount)
        {
            m_VersatileCount = aCount;
        }
        public void ResetVersatileCount()
        {
            m_VersatileCount = 0;
        }
        
        // ------------------------------------------------

        // マスターサーバーへ接続した
        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to master!");

            m_IsInitializedNetwork = true;
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
            m_View = this.gameObject.AddComponent<PhotonView>();
            m_View.Synchronization = ViewSynchronization.Off;

            // VIEWIDを生成
            m_View.ViewID = 1001;
            //PhotonNetwork.AllocateViewID(m_View);

            // プレイヤーデータを作成する
            stPlayerData _playerData    = new stPlayerData();
            _playerData.name            = PhotonNetwork.LocalPlayer.NickName;
            _playerData.id              = PhotonNetwork.CountOfPlayers;
            m_MyID                      = _playerData.id - 1;

            // マスターならユーザーデータを追加する
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                m_PlayerList.Add(_playerData);

                AppManager.Instance.ChangeScene("WaitRoomScene");
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
