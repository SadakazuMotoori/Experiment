using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

using StandardAssets.Characters.Physics;

using KdGame.Net;
namespace Character
{
    [RequireComponent(typeof(MainObjectParameter))]
    [RequireComponent(typeof(OpenCharacterController))]
    [RequireComponent(typeof(AttackHitManager))]
    public class CharacterBrain : MonoBehaviour, Attack.IDamageApplicable
    {
        [SerializeField] float _moveSpeed = 1.0f;
        [SerializeField] float _attackSpeed = 1.0f;

        [SerializeField] TargetSeacher _charaSearcher;
        //        [SerializeField] UnityEngine.Animations.Rigging.MultiAimConstraint _lookAtConstraint;

        [SerializeField] GameObject _winObject;

        [SerializeField] UnityEngine.Animations.Rigging.Rig _animRig;

        // �q�b�g�Ǘ�
        AttackHitManager _attackHitMgr;

        // �I�u�W�F�N�g�̊�{�p�����[�^
        MainObjectParameter _mainObjParam;

//        [SerializeField] FlexibleValue.Value _test;

        // 
//        [SerializeField] Animator _animator;
        [SerializeField] GameStateMachine.StateMachineManager _stateMgr;

        // ���S���ɖ���������GameObject���X�g
        [SerializeField] List<GameObject> _disableOnDied;

        // ����
        InputProvider _inputProvider;
        // �L�����N�^�[�̈ړ��֌W����
        OpenCharacterController _charaCtrl;

        // ���݂̑��x
        Vector3 _velocity;
        public void AddForce(Vector3 force)
        {
            _velocity += force;
        }

        // �d��
        float _gravity = 0;

        // ���C��
        float _friction = 0.85f;

        public enum NowStateTypes
        {
            Move = 0,
            Attack,
            Stagger,
            Died,
            Inaction,
        }
        public NowStateTypes NowStateType { get; private set; }

        void Awake()
        {
            _mainObjParam = GetComponent<MainObjectParameter>();
            _charaCtrl = GetComponent<OpenCharacterController>();
            _attackHitMgr = GetComponent<AttackHitManager>();

            _inputProvider = GetComponentInChildren<InputProvider>();
            Debug.Assert(_inputProvider != null, "InputProvider���Z�b�g����ĂȂ��I");
            //        _input = GetComponent<PlayerInput>();

            // ��������Ă��Ȃ��ƁAgameObject��L��/�����ɂ����Ƃ��ɃX�e�[�g�}�V�������Z�b�g����Ă��낢��܂���
//            _animator.keepAnimatorControllerStateOnDisable = true;
//            _stateMgr = _animator.GetComponent<GameStateMachine.StateMachineManager>();

            /*
            // Animator�̑S�X�e�[�g�ݒ�
            var smbList = _animator.GetBehaviours<GameStateMachine.SMB_StateMachineNode>();
            foreach (var smb in smbList)
            {
                smb.GetStateNode<ASBase>().Initialize(this);
            }
            */
        }

        // Start is called before the first frame update
        void Start()
        {
            //        Destroy(gameObject, 3);
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.timeScale <= 0) return;

            // �X�e�[�g�}�V�������쒆�̂�
            if (_stateMgr.enabled == false) return;

            // �q�b�g�X�g�b�v����
            if (_mainObjParam.HitStopTimer <= 0)
            {
//                _stateMgr.Animator.enabled = true;
                _stateMgr.Animator.speed = 1.0f;

                // �d��
                _velocity.y += _gravity * Time.deltaTime;
                if (_charaCtrl.isGrounded)
                {
                    _velocity.y = 0;
                }

                // �ړ�
                _charaCtrl.Move(_velocity * Time.deltaTime);
//                Debug.Log(_charaCtrl.isGrounded);
            }
            // �q�b�g�X�g�b�v��
            else
            {
                _stateMgr.Animator.speed = 0.0f;
            }
        }

        void FixedUpdate()
        {
            // �X�e�[�g�}�V�������쒆�̂�
            if (_stateMgr.enabled)
            {
                // ���C
                _velocity.x *= _friction;
                _velocity.z *= _friction;
            }
        }

        // 
        public void DoWin()
        {
            // ���O���~
            _animRig.weight = 0;
            // �X�e�[�g�}�V���̏������~
            _stateMgr.enabled = false;
            // ���G��
            _mainObjParam.IsInvincible = true;
            // �����I�u�W�F�N�g��L����(���o)
            _winObject.SetActive(true);
        }

        // �����ړ�
        public void Move(Vector3 move)
        {
            _charaCtrl.Move(move);
        }

        // ���s�ړ�
        void Walk(Vector2 axis, bool doInterporate)
        {
            //        Vector2 axis = _input.currentActionMap["AxisL"].ReadValue<Vector2>();
            if (axis.sqrMagnitude >= 1)
            {
                axis.Normalize();
            }

            float axisPower = axis.magnitude;

            // ����
            if (axisPower > 0.01f)
            {
                Vector3 vLook = new Vector3(axis.x, 0, axis.y);
//                vLook.y = 0;
                Quaternion rota = Quaternion.LookRotation(vLook, Vector3.up);
                //        transform.rotation = rota;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, rota, 720 * Time.deltaTime);
            }

            // �ړ�
            if (doInterporate)
            {
                const float kDelta = 6.0f;
                float nowSpeed = _stateMgr.Animator.GetFloat("MoveSpeed");
                axisPower = Mathf.MoveTowards(nowSpeed, axisPower, kDelta * Time.deltaTime);
            }

            // �����`�ړ��̔䗦
            _stateMgr.Animator.SetFloat("MoveSpeed", axisPower);

            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.x = axis.x;
            forward.z = axis.y;
            forward.Normalize();

            forward *= axisPower * _moveSpeed;
//            _charaCtrl.Move(forward * Time.deltaTime);
            _velocity.x += forward.x * Time.deltaTime;
            _velocity.z += forward.z * Time.deltaTime;

        }

        /// <summary>
        /// �_���[�W���ʒm�����
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool Attack.IDamageApplicable.ApplyDamage(ref Attack.DamageParam param)
        {
            // ���S���̓X�L�b�v
            if (_mainObjParam.IsDied) return false;

            // ���G���̓X�L�b�v
            if (_mainObjParam.IsInvincible) return false;

            // 
            _mainObjParam.Hp -= param.DamageValue;
            if (_mainObjParam.Hp <= 0)
            {
                _mainObjParam.IsDied = true;
            }
            else
            {
                _stateMgr.Animator.SetTrigger("DoStagger");

            }

            // 
            _velocity += param.Blow;

            async void DelaySetHitStop(float hitStopDuration)
            {
                await UniTask.DelayFrame(1);
                _mainObjParam.HitStopTimer = hitStopDuration;
            }

            // �q�b�g�X�g�b�v(�񓯊����s)
            DelaySetHitStop(param.HitStopDuration);

            return true;
        }

        //====================================================
        // �X�e�[�g�}�V���p�m�[�h
        //====================================================
        [System.Serializable]
        public class ASBase : GameStateMachine.StateNodeBase
        {
            protected CharacterBrain _brain;

            public override void OnEnter()
            {
                base.OnEnter();

                _brain = StateMgr.Blackboard.GetValue<CharacterBrain>("Brain");
            }

        }

        [System.Serializable]
        public class ASStand : ASBase
        {
            //        public ASMove(CharacterBrain brain) : base(brain) { }

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Move;
                //                _brain.Walk(Vector2.zero, false);

                // 
                Animator.SetLayerWeight(1, 0.0f);

                Animator.ResetTrigger("DoAttack");

                Animator.SetBool("IsMoving", false);
            }

            public override void OnExit()
            {
                //                Debug.Log("Move����o��");
            }

            public override void OnUpdate()
            {
                Vector2 axis = _brain._inputProvider.GetAxisL();
                if (axis.magnitude >= 0.01f)
                {
                    Animator.SetFloat("MoveSpeed", 0);
                    Animator.SetBool("IsMoving", true);
                    return;
                }

                // ���C
                if (_brain._charaCtrl.isGrounded)
                {
                    //                    _brain.ApplyFriction(0.00001f);
                    _brain._friction = 0.85f;
                }
                else
                {
                    _brain._friction = 0.98f;
                }

                // �d��
                _brain._gravity = -9.8f;

                if (_brain._inputProvider.GetButtonAttack())
                {
                    Animator.SetTrigger("DoAttack");
                    return;
                }

                // ���S����
                if (_brain._mainObjParam.IsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }

            }
        }
        // ����
        [System.Serializable]
        public class ASMove : ASBase
        {
            //        public ASMove(CharacterBrain brain) : base(brain) { }

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Move;
//                _brain.Walk(Vector2.zero, false);

//                Debug.Log("Move�ɂȂ���");
            }

            public override void OnExit()
            {
//                Debug.Log("Move����o��");
            }

            public override void OnUpdate()
            {
                if (_brain._mainObjParam._playerID != NetworkManager.Instance.GetMyID()) return;

                Vector2 axis = _brain._inputProvider.GetAxisL();

                if (axis.magnitude < 0.01f)
                {
                    Animator.SetBool("IsMoving", false);
                    return;
                }

                // ���C
                if (_brain._charaCtrl.isGrounded)
                {
                    //                    _brain.ApplyFriction(0.00001f);
                    _brain._friction = 0.85f;
                }
                else
                {
                    _brain._friction = 0.98f;
                }

                // �d��
                _brain._gravity = -9.8f;

                // �ړ�
                if (_brain._charaCtrl.isGrounded)
                {
                    _brain.Walk(axis, true);
                }

                if (_brain._inputProvider.GetButtonAttack())
                {
                    Animator.SetTrigger("DoAttack");
                    return;
                }

                // ���S����
                if (_brain._mainObjParam.IsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }
            }

            public void OnNetworkUpdate(Vector2 aAxis, bool aGetButtonAttack,bool aIsGrounded,  bool aIsDied)
            {
                if (_brain._mainObjParam._playerID != NetworkManager.Instance.GetMyID()) return;

                Vector2 axis = aAxis;

                if (axis.magnitude < 0.01f)
                {
                    Animator.SetBool("IsMoving", false);
                    return;
                }

                // ���C
                if (aIsGrounded)
                {
                    //                    _brain.ApplyFriction(0.00001f);
                    _brain._friction = 0.85f;
                }
                else
                {
                    _brain._friction = 0.98f;
                }

                // �d��
                _brain._gravity = -9.8f;

                // �ړ�
                if (aIsGrounded)
                {
                    _brain.Walk(axis, true);
                }

                if (aGetButtonAttack)
                {
                    Animator.SetTrigger("DoAttack");
                    return;
                }

                // ���S����
                if (aIsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }
            }
        }
        // �U��
        [System.Serializable]
        public class ASAttack : ASBase
        {

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Attack;

                _brain._attackHitMgr.Reset();

                Animator.SetFloat("AttackSpeed", _brain._attackSpeed);
                //                Debug.Log("Attack�ɂȂ���");

                // ����̕��֌���
                var closestTarget = _brain._charaSearcher.GetClosestTarget();
                if (closestTarget != null)
                {
                    var dir = closestTarget.Value.Target.transform.position - _brain.transform.position;
                    dir.y = 0;

                    _brain.transform.rotation = Quaternion.LookRotation(dir);
                }

            }
            public override void OnUpdate()
            {
                if (_brain._inputProvider.GetButtonAttack())
                {
                    Animator.SetTrigger("DoAttack");
                    return;
                }

                // ���S����
                if (_brain._mainObjParam.IsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }

                // �d��
                _brain._gravity = 0;

                // ���C
                _brain._friction = 0.85f;
            }

            public override void OnExit()
            {
//                Debug.Log("Attack����o��");
            }
        }

        // �_���[�W��낯
        [System.Serializable]
        public class ASStagger : ASBase
        {
            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Stagger;
            }

            public override void OnUpdate()
            {
                // ���C
                _brain._friction = 0.85f;

                // �d��
                _brain._gravity = -9.8f;
            }
        }

        // ���S
        [System.Serializable]
        public class ASDied : ASBase
        {
            float _remainingTime = 0;

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Died;

                // �w��GameObject������
                foreach (var obj in _brain._disableOnDied)
                {
                    if (obj == null) continue;
                    obj.SetActive(false);
                }

                _brain.GetComponent<CapsuleCollider>().enabled = false;

                // �w�莞�Ԃŏ�����
                _remainingTime = 5.0f;

                Debug.Log("�퓬�s�\�I");
            }

            public override void OnUpdate()
            {
                // ���C
                _brain._friction = 0.85f;

                _remainingTime -= Time.deltaTime;
                if(_remainingTime <= 0)
                {
                    Destroy(_brain.gameObject);
                }
            }
        }

        /// <summary>
        /// �s���s��
        /// </summary>
        [System.Serializable]
        public class ASInaction : ASBase
        {
            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Inaction;

            }

            public override void OnUpdate()
            {

            }
        }
    }
}