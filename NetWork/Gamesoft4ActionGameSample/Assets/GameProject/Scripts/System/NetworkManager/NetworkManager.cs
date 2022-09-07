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
        public const int NETWORK_TIME_OUT_WAIT = 10000; // �^�C���A�E�g����(ms)

        private PhotonView          m_View;
        private stPlayerInfo        m_PlayerInfo;
        private int                 m_MyID = 0;
        private long                m_VersatileCount;

        // ��M�f�[�^
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

            // �V���A���C�U�̏����ݒ�
            MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance, // �R�[�h���������^�����N���X
                MessagePack.Unity.UnityResolver.Instance,
                MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            // LZ4 ���k���p
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
                // �����������Ă��Ȃ��A�y�ѓ���̓��쒆�Ȃ�ҋ@����
                if (IsConnectedAndReady()) break;

                await UniTask.Delay(10);
            }
        }

        private async UniTask<ENETWORK_ERROR_CODE> RequestConnect(string aName)
        {
            // Photon�T�[�o�[�֐ڑ�����
            Debug.Log("Connect to master");
            if (!PhotonNetwork.ConnectUsingSettings())
            {
                m_LastError = ENETWORK_ERROR_CODE.ERR_CONNECT;
            }
            await NetworkWait();

            // ���r�[�֐ڑ�����
            Debug.Log("Connect to lobby");
            if (!PhotonNetwork.JoinLobby())
            {
                m_LastError = ENETWORK_ERROR_CODE.ERR_JOINLOBBY;
            }
            await NetworkWait();

            // ���[�����쐬/�Q������
            CreateOrJoinDebugRoom();
            await NetworkWait();

            return m_LastError;
        }

        private ENETWORK_ERROR_CODE CreateOrJoinDebugRoom()
        {
            // �}�X�^�[�Ȃ烋�[�����쐬����
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

            // �l�b�g���[�N���������ɂ܂��̓T�[�o�[�֐ڑ�
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

        // �}�X�^�[�T�[�o�[�֐ڑ�����
        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to master!");

            m_IsInitializedNetwork = true;
        }

        // ���r�[�ɓ�������
        public override void OnJoinedLobby()
        {
            Debug.Log("joined lobby!!!");
        }

        // ���[�����쐬����
        public override void OnCreatedRoom()
        {
            Debug.Log("create room!!!");
        }

        // ���[���ɓ�������
        public override void OnJoinedRoom()
        {
            Debug.Log("joined room!!!");

            // �ʐM�p�l�b�g���[�N�I�u�W�F�N�g���쐬
            m_View = this.gameObject.AddComponent<PhotonView>();
            m_View.Synchronization  = ViewSynchronization.Off;
            //PhotonNetwork.AllocateViewID(m_View);
            m_View.ViewID           = 1001;

            // �v���C���[�f�[�^���쐬����
            stPlayerData _playerData    = new stPlayerData();
            _playerData.playername      = PhotonNetwork.LocalPlayer.NickName;
            _playerData.playerid        = PhotonNetwork.CountOfPlayers;
            m_MyID                      = _playerData.playerid - 1;

            // �}�X�^�[�Ȃ烆�[�U�[�f�[�^��ǉ�����
            if (PhotonNetwork.LocalPlayer.IsMasterClient)
            {
                m_PlayerInfo.playerlist.Add(_playerData);

                AppManager.Instance.ChangeScene("WaitRoomScene");
            }
        }

        // ���[���ɒN������������
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
            stPlayerData _playerData    = new stPlayerData();
            _playerData.playername      = newPlayer.NickName;
            _playerData.playerid        = PhotonNetwork.CountOfPlayers - 1;

            m_PlayerInfo.playerlist.Add(_playerData);

            // �X�V�����v���C���[���X�g�𑗐M����
            m_PlayerInfo.viewid         = m_View.ViewID;
            CreateSendData(ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST, RpcTarget.Others, m_PlayerInfo);
        }

        // �N�������[������ގ�����
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
