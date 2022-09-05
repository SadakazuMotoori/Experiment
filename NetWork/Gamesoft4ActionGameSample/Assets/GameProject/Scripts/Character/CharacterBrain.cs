using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

using StandardAssets.Characters.Physics;

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

        // ヒット管理
        AttackHitManager _attackHitMgr;

        // オブジェクトの基本パラメータ
        MainObjectParameter _mainObjParam;

//        [SerializeField] FlexibleValue.Value _test;

        // 
//        [SerializeField] Animator _animator;
        [SerializeField] GameStateMachine.StateMachineManager _stateMgr;

        // 死亡時に無効化するGameObjectリスト
        [SerializeField] List<GameObject> _disableOnDied;

        // 入力
        InputProvider _inputProvider;
        // キャラクターの移動関係制御
        OpenCharacterController _charaCtrl;

        // 現在の速度
        Vector3 _velocity;
        public void AddForce(Vector3 force)
        {
            _velocity += force;
        }

        // 重力
        float _gravity = 0;

        // 摩擦力
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
            Debug.Assert(_inputProvider != null, "InputProviderがセットされてない！");
            //        _input = GetComponent<PlayerInput>();

            // これをしていないと、gameObjectを有効/無効にしたときにステートマシンがリセットされていろいろまずい
//            _animator.keepAnimatorControllerStateOnDisable = true;
//            _stateMgr = _animator.GetComponent<GameStateMachine.StateMachineManager>();

            /*
            // Animatorの全ステート設定
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

            // ステートマシンが動作中のみ
            if (_stateMgr.enabled == false) return;

            // ヒットストップ無し
            if (_mainObjParam.HitStopTimer <= 0)
            {
//                _stateMgr.Animator.enabled = true;
                _stateMgr.Animator.speed = 1.0f;

                // 重力
                _velocity.y += _gravity * Time.deltaTime;
                if (_charaCtrl.isGrounded)
                {
                    _velocity.y = 0;
                }

                // 移動
                _charaCtrl.Move(_velocity * Time.deltaTime);
//                Debug.Log(_charaCtrl.isGrounded);
            }
            // ヒットストップ中
            else
            {
                _stateMgr.Animator.speed = 0.0f;
            }
        }

        void FixedUpdate()
        {
            // ステートマシンが動作中のみ
            if (_stateMgr.enabled)
            {
                // 摩擦
                _velocity.x *= _friction;
                _velocity.z *= _friction;
            }
        }

        // 
        public void DoWin()
        {
            // リグを停止
            _animRig.weight = 0;
            // ステートマシンの処理を停止
            _stateMgr.enabled = false;
            // 無敵化
            _mainObjParam.IsInvincible = true;
            // 勝利オブジェクトを有効化(演出)
            _winObject.SetActive(true);
        }

        // 強制移動
        public void Move(Vector3 move)
        {
            _charaCtrl.Move(move);
        }

        // 歩行移動
        void Walk(Vector2 axis, bool doInterporate)
        {
            //        Vector2 axis = _input.currentActionMap["AxisL"].ReadValue<Vector2>();
            if (axis.sqrMagnitude >= 1)
            {
                axis.Normalize();
            }

            float axisPower = axis.magnitude;

            // 向き
            if (axisPower > 0.01f)
            {
                Vector3 vLook = new Vector3(axis.x, 0, axis.y);
//                vLook.y = 0;
                Quaternion rota = Quaternion.LookRotation(vLook, Vector3.up);
                //        transform.rotation = rota;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, rota, 720 * Time.deltaTime);
            }

            // 移動
            if (doInterporate)
            {
                const float kDelta = 6.0f;
                float nowSpeed = _stateMgr.Animator.GetFloat("MoveSpeed");
                axisPower = Mathf.MoveTowards(nowSpeed, axisPower, kDelta * Time.deltaTime);
            }

            // 立ち～移動の比率
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
        /// ダメージが通知される
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        bool Attack.IDamageApplicable.ApplyDamage(ref Attack.DamageParam param)
        {
            // 死亡時はスキップ
            if (_mainObjParam.IsDied) return false;

            // 無敵時はスキップ
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

            // ヒットストップ(非同期実行)
            DelaySetHitStop(param.HitStopDuration);

            return true;
        }

        //====================================================
        // ステートマシン用ノード
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
                //                Debug.Log("Moveから出た");
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

                // 摩擦
                if (_brain._charaCtrl.isGrounded)
                {
                    //                    _brain.ApplyFriction(0.00001f);
                    _brain._friction = 0.85f;
                }
                else
                {
                    _brain._friction = 0.98f;
                }

                // 重力
                _brain._gravity = -9.8f;

                if (_brain._inputProvider.GetButtonAttack())
                {
                    Animator.SetTrigger("DoAttack");
                    return;
                }

                // 死亡判定
                if (_brain._mainObjParam.IsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }

            }
        }
        // 歩き
        [System.Serializable]
        public class ASMove : ASBase
        {
            //        public ASMove(CharacterBrain brain) : base(brain) { }

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Move;
//                _brain.Walk(Vector2.zero, false);

//                Debug.Log("Moveになった");
            }

            public override void OnExit()
            {
//                Debug.Log("Moveから出た");
            }

            public override void OnUpdate()
            {
                Vector2 axis = _brain._inputProvider.GetAxisL();

                if (axis.magnitude < 0.01f)
                {
                    Animator.SetBool("IsMoving", false);
                    return;
                }

                // 摩擦
                if (_brain._charaCtrl.isGrounded)
                {
                    //                    _brain.ApplyFriction(0.00001f);
                    _brain._friction = 0.85f;
                }
                else
                {
                    _brain._friction = 0.98f;
                }

                // 重力
                _brain._gravity = -9.8f;

                // 移動
                if (_brain._charaCtrl.isGrounded)
                {
                    _brain.Walk(axis, true);
                }

                if (_brain._inputProvider.GetButtonAttack())
                {
                    Animator.SetTrigger("DoAttack");
                    return;
                }

                // 死亡判定
                if (_brain._mainObjParam.IsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }
            }
        }
        // 攻撃
        [System.Serializable]
        public class ASAttack : ASBase
        {

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Attack;

                _brain._attackHitMgr.Reset();

                Animator.SetFloat("AttackSpeed", _brain._attackSpeed);
                //                Debug.Log("Attackになった");

                // 相手の方へ向く
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

                // 死亡判定
                if (_brain._mainObjParam.IsDied)
                {
                    Animator.SetTrigger("DoDied");
                    return;
                }

                // 重力
                _brain._gravity = 0;

                // 摩擦
                _brain._friction = 0.85f;
            }

            public override void OnExit()
            {
//                Debug.Log("Attackから出た");
            }
        }

        // ダメージよろけ
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
                // 摩擦
                _brain._friction = 0.85f;

                // 重力
                _brain._gravity = -9.8f;
            }
        }

        // 死亡
        [System.Serializable]
        public class ASDied : ASBase
        {
            float _remainingTime = 0;

            public override void OnEnter()
            {
                base.OnEnter();

                _brain.NowStateType = NowStateTypes.Died;

                // 指定GameObject無効化
                foreach (var obj in _brain._disableOnDied)
                {
                    if (obj == null) continue;
                    obj.SetActive(false);
                }

                _brain.GetComponent<CapsuleCollider>().enabled = false;

                // 指定時間で消える
                _remainingTime = 5.0f;

                Debug.Log("戦闘不能！");
            }

            public override void OnUpdate()
            {
                // 摩擦
                _brain._friction = 0.85f;

                _remainingTime -= Time.deltaTime;
                if(_remainingTime <= 0)
                {
                    Destroy(_brain.gameObject);
                }
            }
        }

        /// <summary>
        /// 行動不可
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