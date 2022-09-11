using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

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
        public static NetworkManager        Instance { get; private set; }

        // ----------------------------------------------------------------

        private bool                        m_IsInitializedManagerSystem;
        private bool                        m_IsInitializedNetWork;
        private ENETWORK_ERROR_CODE         m_LastError;
        private List<Queue<stReceiveData>>  m_NetworkCmdList;
        // ----------------------------------------------------------------

        private int                         m_RoomID;
        private stNetInfo                   m_NetInfo;
        // ----------------------------------------------------------------

#if USE_STEAMWORKS
#else
        private PhotonView                  m_View;
#endif
        // ----------------------------------------------------------------

        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null) return;
            Instance                        = this;

            m_IsInitializedManagerSystem    = false;
            m_IsInitializedNetWork          = false;

            // シリアライザの初期設定
            {
                MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(
                                                                                    MessagePack.Resolvers.GeneratedResolver.Instance, // コード生成した型解決クラス
                                                                                    MessagePack.Unity.UnityResolver.Instance,
                                                                                    MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                                                                                    MessagePack.Resolvers.StandardResolver.Instance
                                                                                );

                // LZ4 圧縮利用
                var option = MessagePack.MessagePackSerializerOptions.Standard.
                             WithCompression(MessagePack.MessagePackCompression.Lz4BlockArray).
                             WithResolver(MessagePack.Resolvers.StaticCompositeResolver.Instance);
                MessagePack.MessagePackSerializer.DefaultOptions = option;
            }
        }

        void Start()
        {
        }

        async void Update()
        {
            if (!IsNetworkSystemReady()) return;

            for (int i = 0; i < m_NetworkCmdList.Count; i++)
            {
                if (m_NetworkCmdList[i].Count == 0) continue;

                stReceiveData _data = m_NetworkCmdList[i].Dequeue();
                await AnalyzeNetworkData(_data);
            }
        }
        // ----------------------------------------------------------------

        /// <summary>
        /// ルームマスタークライアントかどうかを返す
        /// </summary>
        /// <returns>無効なリスト内インデックス</returns>
        private int GetInvalidIndexByInfoList()
        {
            for(int i = 0; i < INROOM_PLAYER_MAX; i++)
            {
                if (m_NetInfo.infolist[i].id == -1) return i;
            }
            return -1;
        }

        /// <summary>
        /// 準備完了していない及び特定の動作中なら待機する
        /// </summary>
        private async UniTask NetworkWait()
        {
            while(true)
            {
                if (IsNetworkSystemReady())                             break;

                await UniTask.Delay(1000);

                if (m_LastError != ENETWORK_ERROR_CODE.NET_ERR_NORE)    break;
            }
        }

        /// <summary>
        /// ルーム入室待機する
        /// </summary>
        private async UniTask JoinRoomWait()
        {
            while (true)
            {
#if USE_STEAMWORKS
#else
                if (PhotonNetwork.InRoom)                               break;
#endif
                await UniTask.Delay(1000);

                if (m_LastError != ENETWORK_ERROR_CODE.NET_ERR_NORE)    break;
            }
        }
        // ----------------------------------------------------------------

        /// <summary>
        /// ネットワークマネージャを初期化する
        /// </summary>
        public void InitializeManagerSystem()
        {
            if (m_IsInitializedManagerSystem) return;

            // ネットワーク用仮情報を設定
            m_RoomID            = -1;
            m_NetInfo           = new stNetInfo();
            m_NetInfo.infolist  = new List<stNetData>();
            m_NetworkCmdList    = new List<Queue<stReceiveData>>();
            for (int i = 0; i < INROOM_PLAYER_MAX; i++)
            {
                m_NetInfo.infolist.Add(new stNetData(m_RoomID, "No Name"));
                m_NetworkCmdList.Add(new Queue<stReceiveData>());
            }
        
            m_LastError                     = ENETWORK_ERROR_CODE.NET_ERR_NORE;
            m_IsInitializedManagerSystem    = true;
        }

        /// <summary>
        /// ネットワークシステムを初期化する
        /// </summary>
        /// <returns>エラーコード</returns>
        public ENETWORK_ERROR_CODE InitializeNetWorkSystem()
        {
            if (m_IsInitializedNetWork) return ENETWORK_ERROR_CODE.NET_ERR_INITIALIZE_ALREADY;

#if USE_STEAM_WORKS
            // SteamSystem初期化
            {
                if (SteamManager.Initialized)
                {
                    Debug.Log(SteamFriends.GetPersonaName());
                }
                else
                {
                    return ENETWORK_ERROR_CODE.NET_ERR_INITIALIZE;
                }
            }
#else
            // PhotonSystem初期化
            {
                // 通信用ネットワークオブジェクトを作成
                m_View                  = this.gameObject.AddComponent<PhotonView>();
                m_View.Synchronization  = ViewSynchronization.Off;
                m_View.ViewID           = 1001;

                if (PhotonNetwork.ConnectUsingSettings())
                {
                }
                else
                {
                    Debug.Log("Photonサーバーへの接続エラー");
                    return ENETWORK_ERROR_CODE.NET_ERR_INITIALIZE;
                }
            }
#endif
            m_IsInitializedNetWork = true;

            return ENETWORK_ERROR_CODE.NET_ERR_NORE;
        }

        /// <summary>
        /// ルームマスタークライアントかどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool IsMasterClient()
        {
            if (IsNetworkSystemReady()) return false;

#if USE_STEAMWORKS
            return false;
#else
            return PhotonNetwork.IsMasterClient;
#endif
        }

        /// <summary>
        /// ネットワークシステムが利用可能かどうかを返す
        /// </summary>
        /// <returns></returns>
        public bool IsNetworkSystemReady()
        {
            return (m_IsInitializedManagerSystem && m_IsInitializedNetWork);
        }

        /// <summary>
        /// プレイヤー名を設定する
        /// </summary>
        /// <param name="aPlayerName"></param>
        public void SetMyPlayerName(string aPlayerName)
        {
            Debug.Log("プレイヤー名を変更 = "+aPlayerName);

            string _nowPlayerName   = aPlayerName;

            stNetData _info         = m_NetInfo.infolist[0];
            _info.name              = _nowPlayerName;
            m_NetInfo.infolist[0]   = _info;

#if USE_STEAM_WORKS
            SteamFriends.SetPersonaName(aPlayerName);
#else
            PhotonNetwork.NickName  = aPlayerName;
#endif
        }

        /// <summary>
        /// プレイヤー名を取得する
        /// </summary>
        /// <returns>設定プレイヤー名</returns>
        public string GetMyPlayerName()
        {
#if USE_STEAM_WORKS
            return SteamFriends.GetPersonaName();
#else
            return PhotonNetwork.NickName;
#endif
        }

        /// <summary>
        /// ルームを作成する
        /// </summary>
        /// <param name="aRoomName"></param>
        public async UniTask<ENETWORK_ERROR_CODE> CreateRoom(string aRoomName)
        {
            m_LastError = ENETWORK_ERROR_CODE.NET_ERR_NORE;

#if USE_STEAM_WORKS
            return ENETWORK_ERROR_CODE.NET_ERR_CREATEROOM;
#else
            if (!PhotonNetwork.CreateRoom(aRoomName, new RoomOptions(), TypedLobby.Default))
            {
                m_LastError = ENETWORK_ERROR_CODE.NET_ERR_CREATEROOM;
            }
#endif
            // 結果が帰ってくるまで待つ
            await NetworkWait();
            await JoinRoomWait();

            return m_LastError;
        }

        /// <summary>
        /// ルームに入室する
        /// </summary>
        /// <param name="aRoomName"></param>
        public async UniTask<ENETWORK_ERROR_CODE> JoinRoom(string aRoomName)
        {
            m_LastError = ENETWORK_ERROR_CODE.NET_ERR_NORE;

#if USE_STEAM_WORKS
            return ENETWORK_ERROR_CODE.NET_ERR_CREATEROOM;
#else
            if (!PhotonNetwork.JoinRoom(aRoomName))
            {
                m_LastError = ENETWORK_ERROR_CODE.NET_ERR_JOINROOM;
            }
#endif
            // 結果が帰ってくるまで待つ
            await NetworkWait();
            await JoinRoomWait();

            return m_LastError;
        }

        /// <summary>
        /// ルーム内の有効なプレイヤー数を取得する
        /// </summary>
        /// <returns>有効プレイヤー数</returns>
        public int GetInRoomMemberNum()
        {
            for(int i = 0; i < m_NetInfo.infolist.Count; i++)
            {
                if (m_NetInfo.infolist[i].id == -1) return i;
            }

            return 0;
        }

        /// <summary>
        /// ルーム内の任意のプレイヤー名を取得する
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns>指定プレイヤー名</returns>
        public string GetMemberNameByIndex(int aIndex = 0)
        {
            if (aIndex >= m_NetInfo.infolist.Count) return "";

            return m_NetInfo.infolist[aIndex].name;
        }

        // ----------------------------------------------------------------

#if USE_STEAM_WORKS
#else
        /// <summary>
        /// Photon専用コールバック:ルームを作成した
        /// </summary>
        public override void OnCreatedRoom()
        {
            // プレイヤーデータを作成する
            if (!PhotonNetwork.IsMasterClient) return;

            m_RoomID                        = 0;
            stNetData _info                 = m_NetInfo.infolist[m_RoomID];
            _info.id                        = 0;

            m_NetInfo.infolist[m_RoomID]    = _info;
        }

        /// <summary>
        /// Photon専用コールバック:ルームに入室した
        /// </summary>
        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.IsMasterClient)  return;
        }

        /// <summary>
        /// Photon専用コールバック:ルームに誰かが入室した
        /// </summary>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!PhotonNetwork.IsMasterClient) return;

            // 入室したプレイヤー情報を作成する
            stNetData _newPlayer        = new stNetData();
            int _newID                  = GetInvalidIndexByInfoList();
            _newPlayer.name             = newPlayer.NickName;
            _newPlayer.id               = _newID;

            m_NetInfo.infolist[_newID]  = _newPlayer;

            // 更新したプレイヤーリストを送信する
            CreateSendData(ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST, RpcTarget.Others, m_NetInfo);
        }

        /// <summary>
        /// Photon専用コールバック:ルーム作成に失敗した
        /// </summary>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            m_LastError = ENETWORK_ERROR_CODE.NET_ERR_CREATEROOM;
        }

        /// <summary>
        /// Photon専用コールバック:ルームへの入室に失敗した
        /// </summary>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            m_LastError = ENETWORK_ERROR_CODE.NET_ERR_JOINROOM;
        }
#endif
        /*
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

                    // マスターならユーザーデータを追加する
                    if (PhotonNetwork.LocalPlayer.IsMasterClient)
                    {
                        _playerData.playerid    = 0;
                        m_MyID                  = 0;

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
                    _playerData.playerid        = m_PlayerInfo.playerlist.Count;

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
        */

        /// <summary>
        /// 受信したネットワークコマンドをキューに入れる
        /// </summary>
        [PunRPC]
        private void RpcSendMessage(ENETWORK_COMMAND aCMD , int aID , byte[] aData)
        {
            stReceiveData _receiveData = new stReceiveData
            {
                cmd     = aCMD,
                data    = aData
            };
            m_NetworkCmdList[aID].Enqueue(_receiveData);
        }
    }
}
