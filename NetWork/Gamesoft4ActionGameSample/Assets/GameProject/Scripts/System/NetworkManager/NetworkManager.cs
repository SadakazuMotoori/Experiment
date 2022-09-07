using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using Photon.Pun;
using Photon.Realtime;

using UnityEngine;

namespace KdGame.Net
{
    public partial class NetworkManager : MonoBehaviourPunCallbacks
    {
        public static NetworkManager Instance { get; private set; }
        public const int NETWORK_TIME_OUT_WAIT = 10000; // タイムアウト時間(ms)

        private PhotonView          m_View;
        private stPlayerInfo        m_PlayerInfo;
        private int                 m_MyID = 0;
        private long                m_VersatileCount;

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

            // LZ4 圧縮利用
            var option = MessagePack.MessagePackSerializerOptions.Standard
                .WithCompression(MessagePack.MessagePackCompression.Lz4BlockArray)
                .WithResolver(MessagePack.Resolvers.StaticCompositeResolver.Instance);
            MessagePack.MessagePackSerializer.DefaultOptions = option;
        }

        void Start()
        {
            int a = 0;
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

            m_PlayerInfo            = new stPlayerInfo();
            m_PlayerInfo.playerlist = new List<stPlayerData>();
            m_LastError             = ENETWORK_ERROR_CODE.ERR_NORE;
        }

        public List<stPlayerData> GetPlayerList()
        {
            return m_PlayerInfo.playerlist;
        }

        public int GetMemberNum()
        {
            return m_PlayerInfo.playerlist.Count;
        }

        public string GetMemberName(int index = 0)
        {
            if (index >= m_PlayerInfo.playerlist.Count) return "";

            return m_PlayerInfo.playerlist[index].playername;
        }

        public string GetMyName()
        {
            if (m_MyID >= m_PlayerInfo.playerlist.Count) return "";

            return m_PlayerInfo.playerlist[m_MyID].playername;
        }

        public int GetMyID()
        {
            return m_MyID;
        }

        public string GetMyName(int aIndex)
        {
            if (m_MyID >= m_PlayerInfo.playerlist.Count) return "";

            return m_PlayerInfo.playerlist[aIndex].playername;
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
            m_View.Synchronization  = ViewSynchronization.Off;
            //PhotonNetwork.AllocateViewID(m_View);
            m_View.ViewID           = 1001;

            // プレイヤーデータを作成する
            stPlayerData _playerData    = new stPlayerData();
            _playerData.playername      = PhotonNetwork.LocalPlayer.NickName;
            _playerData.playerid        = PhotonNetwork.CountOfPlayers;
            m_MyID                      = _playerData.playerid - 1;

            // マスターならユーザーデータを追加する
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                m_PlayerInfo.playerlist.Add(_playerData);

                AppManager.Instance.ChangeScene("WaitRoomScene");
            }
        }

        // ルームに誰かが入室した
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
            stPlayerData _playerData    = new stPlayerData();
            _playerData.playername      = newPlayer.NickName;
            _playerData.playerid        = PhotonNetwork.CountOfPlayers - 1;

            m_PlayerInfo.playerlist.Add(_playerData);

            // 更新したプレイヤーリストを送信する
            m_PlayerInfo.viewid         = m_View.ViewID;
            CreateSendData(ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST, RpcTarget.Others, m_PlayerInfo);
        }

        // 誰かがルームから退室した
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
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
