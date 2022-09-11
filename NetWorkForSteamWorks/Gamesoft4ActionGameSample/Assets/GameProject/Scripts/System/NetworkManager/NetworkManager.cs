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

        private long                        m_VersatileCount;
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

            // �V���A���C�U�̏����ݒ�
            {
                MessagePack.Resolvers.StaticCompositeResolver.Instance.Register(
                                                                                    MessagePack.Resolvers.GeneratedResolver.Instance, // �R�[�h���������^�����N���X
                                                                                    MessagePack.Unity.UnityResolver.Instance,
                                                                                    MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
                                                                                    MessagePack.Resolvers.StandardResolver.Instance
                                                                                );

                // LZ4 ���k���p
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
        /// ���[���}�X�^�[�N���C�A���g���ǂ�����Ԃ�
        /// </summary>
        /// <returns>�����ȃ��X�g���C���f�b�N�X</returns>
        private int GetInvalidIndexByInfoList()
        {
            for(int i = 0; i < INROOM_PLAYER_MAX; i++)
            {
                if (m_NetInfo.infolist[i].id == -1) return i;
            }
            return -1;
        }

        /// <summary>
        /// �����������Ă��Ȃ��y�ѓ���̓��쒆�Ȃ�ҋ@����
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
        /// ���[�������ҋ@����
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
        /// �l�b�g���[�N�}�l�[�W��������������
        /// </summary>
        public void InitializeManagerSystem()
        {
            if (m_IsInitializedManagerSystem) return;

            // �l�b�g���[�N�p������ݒ�
            m_RoomID            = -1;
            m_NetInfo           = new stNetInfo();
            m_NetInfo.infolist  = new List<stNetData>();
            m_NetworkCmdList    = new List<Queue<stReceiveData>>();
            for (int i = 0; i < INROOM_PLAYER_MAX; i++)
            {
                m_NetInfo.infolist.Add(new stNetData(m_RoomID, "No Name"));
                m_NetworkCmdList.Add(new Queue<stReceiveData>());
            }

            // ���̑��ėp�n�����o������
            ResetVersatileCount();

            m_LastError                     = ENETWORK_ERROR_CODE.NET_ERR_NORE;
            m_IsInitializedManagerSystem    = true;
        }

        /// <summary>
        /// �l�b�g���[�N�V�X�e��������������
        /// </summary>
        /// <returns>�G���[�R�[�h</returns>
        public ENETWORK_ERROR_CODE InitializeNetWorkSystem()
        {
            if (m_IsInitializedNetWork) return ENETWORK_ERROR_CODE.NET_ERR_INITIALIZE_ALREADY;

#if USE_STEAM_WORKS
            // SteamSystem������
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
            // PhotonSystem������
            {
                // �ʐM�p�l�b�g���[�N�I�u�W�F�N�g���쐬
                m_View                  = this.gameObject.AddComponent<PhotonView>();
                m_View.Synchronization  = ViewSynchronization.Off;
                m_View.ViewID           = 1001;

                if (PhotonNetwork.ConnectUsingSettings())
                {
                }
                else
                {
                    Debug.Log("Photon�T�[�o�[�ւ̐ڑ��G���[");
                    return ENETWORK_ERROR_CODE.NET_ERR_INITIALIZE;
                }
            }
#endif
            m_IsInitializedNetWork = true;

            return ENETWORK_ERROR_CODE.NET_ERR_NORE;
        }

        /// <summary>
        /// ���[���}�X�^�[�N���C�A���g���ǂ�����Ԃ�
        /// </summary>
        /// <returns></returns>
        public bool IsMasterClient()
        {
            if (!IsNetworkSystemReady()) return false;

#if USE_STEAMWORKS
            return false;
#else
            return PhotonNetwork.IsMasterClient;
#endif
        }

        /// <summary>
        /// �l�b�g���[�N�V�X�e�������p�\���ǂ�����Ԃ�
        /// </summary>
        /// <returns></returns>
        public bool IsNetworkSystemReady()
        {
            return (m_IsInitializedManagerSystem && m_IsInitializedNetWork);
        }

        /// <summary>
        /// �v���C���[����ݒ肷��
        /// </summary>
        /// <param name="aPlayerName"></param>
        public void SetMyPlayerName(string aPlayerName)
        {
            Debug.Log("�v���C���[����ύX = "+aPlayerName);

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
        /// ���g�̃��[����ID���擾����
        /// </summary>
        /// <returns>���g�̃��[����ID</returns>
        public int GetMyRoomID()
        {
#if USE_STEAM_WORKS
            return -1;
#else
            return m_RoomID;
#endif
        }

        /// <summary>
        /// �v���C���[�����擾����
        /// </summary>
        /// <returns>�ݒ�v���C���[��</returns>
        public string GetMyPlayerName()
        {
#if USE_STEAM_WORKS
            return SteamFriends.GetPersonaName();
#else
            return PhotonNetwork.NickName;
#endif
        }

        /// <summary>
        /// ���[�����쐬����
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
            // ���ʂ��A���Ă���܂ő҂�
            await NetworkWait();
            await JoinRoomWait();

            return m_LastError;
        }

        /// <summary>
        /// ���[���ɓ�������
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
            // ���ʂ��A���Ă���܂ő҂�
            await NetworkWait();
            await JoinRoomWait();

            return m_LastError;
        }

        /// <summary>
        /// ���[�����̗L���ȃv���C���[�����擾����
        /// </summary>
        /// <returns>�L���v���C���[��</returns>
        public int GetInRoomMemberNum()
        {
            for(int i = 0; i < m_NetInfo.infolist.Count; i++)
            {
                if (m_NetInfo.infolist[i].id == -1) return i;
            }

            return 0;
        }

        /// <summary>
        /// ���[�����̔C�ӂ̃v���C���[�����擾����
        /// </summary>
        /// <param name="aIndex"></param>
        /// <returns>�w��v���C���[��</returns>
        public string GetMemberNameByIndex(int aIndex = 0)
        {
            if (aIndex >= m_NetInfo.infolist.Count) return "";

            return m_NetInfo.infolist[aIndex].name;
        }

        /// <summary>
        /// �J�E���g�_�E���X�^�[�g�ʒm
        /// </summary>
        /// <param name="aStartTime"></param>
        public void CountStartNotification(int aStartTime)
        {
            CreateSendData(ENETWORK_COMMAND.CMD_COUNTDOWN, RpcTarget.All, aStartTime);
        }

        /// <summary>
        /// �J�E���g�X�^�[�g���Ԑݒ�
        /// </summary>
        /// <param name="aVersatileCount"></param>
        public void SetVersatileCount(long aVersatileCount)
        {
            m_VersatileCount = aVersatileCount;
        }

        /// <summary>
        /// �J�E���g���Ԏ擾
        /// </summary>
        /// <returns>�c��J�E���g�l</returns>
        public long GetVersatileCount()
        {
            return m_VersatileCount;
        }

        /// <summary>
        /// �J�E���g���ԏ�����
        /// </summary>
        public void ResetVersatileCount()
        {
            m_VersatileCount = 0;
        }
        // ----------------------------------------------------------------

#if USE_STEAM_WORKS
#else
        /// <summary>
        /// Photon��p�R�[���o�b�N:���[�����쐬����
        /// </summary>
        public override void OnCreatedRoom()
        {
            // �v���C���[�f�[�^���쐬����
            if (!IsMasterClient()) return;

            m_RoomID                        = 0;
            stNetData _info                 = m_NetInfo.infolist[m_RoomID];
            _info.id                        = 0;

            m_NetInfo.infolist[m_RoomID]    = _info;
        }

        /// <summary>
        /// Photon��p�R�[���o�b�N:���[���ɓ�������
        /// </summary>
        public override void OnJoinedRoom()
        {
            if (IsMasterClient())  return;
        }

        /// <summary>
        /// Photon��p�R�[���o�b�N:���[���ɒN������������
        /// </summary>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            if (!IsMasterClient()) return;

            // ���������v���C���[�����쐬����
            stNetData _newPlayer        = new stNetData();
            int _newID                  = GetInvalidIndexByInfoList();
            _newPlayer.name             = newPlayer.NickName;
            _newPlayer.id               = _newID;

            m_NetInfo.infolist[_newID]  = _newPlayer;

            // �X�V�����v���C���[���X�g�𑗐M����
            CreateSendData(ENETWORK_COMMAND.CMD_UPDATEPLAYER_LIST, RpcTarget.Others, m_NetInfo);
        }

        /// <summary>
        /// Photon��p�R�[���o�b�N:���[���쐬�Ɏ��s����
        /// </summary>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            base.OnCreateRoomFailed(returnCode, message);

            m_LastError = ENETWORK_ERROR_CODE.NET_ERR_CREATEROOM;
        }

        /// <summary>
        /// Photon��p�R�[���o�b�N:���[���ւ̓����Ɏ��s����
        /// </summary>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            base.OnJoinRoomFailed(returnCode, message);

            m_LastError = ENETWORK_ERROR_CODE.NET_ERR_JOINROOM;
        }
#endif

        /// <summary>
        /// ��M�����l�b�g���[�N�R�}���h���L���[�ɓ����
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
