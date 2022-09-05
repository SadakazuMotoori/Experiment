using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStateMachine
{
    //===================================================
    /// <summary>
    /// ステートマシン用ノード基本クラス
    /// </summary>
    //===================================================
    [System.Serializable]
    public class StateNodeBase
    {
        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }

        StateMachineManager _stateMgr;
        public StateMachineManager StateMgr => _stateMgr;

        Animator _animator;
        public Animator Animator => _animator;

        public void Initialize(Animator animator, StateMachineManager mgr)
        {
            _animator = animator;
            _stateMgr = mgr;
        }
    }

    //===================================================
    /// <summary>
    /// 
    /// </summary>
    //===================================================
    [RequireComponent(typeof(Animator))]
    public class StateMachineManager : MonoBehaviour
    {
        // アニメーター
        Animator _animator;
        public Animator Animator => _animator;

        // 共通データ
        [Header("未指定ならGetComponentで検索される")]
        [SerializeField] Blackboard _blackboard;
        public Blackboard Blackboard => _blackboard;

        // 現在実行しているステートノード
        StateNodeBase _nowState;
        public StateNodeBase NowState => _nowState;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            Debug.Assert(_animator != null, "Animatorが存在しません！");
            _animator.keepAnimatorControllerStateOnDisable = true;

            if (_blackboard == null)
            {
                _blackboard = GetComponent<Blackboard>();
            }
            Debug.Assert(_blackboard != null, "Blackboardが存在しません！");
        }

        public void ChangeState(StateNodeBase state)
        {
            if(_nowState != null)
            {
                _nowState.OnExit();
            }

            _nowState = state;

            if (_nowState != null)
            {
                _nowState.OnEnter();
            }
        }

        void Update()
        {
            if (_nowState == null) return;
            if (_animator.enabled == false) return;

            _nowState.OnUpdate();
        }

        void FixedUpdate()
        {
            if (_nowState == null) return;
            if (_animator.enabled == false) return;

            _nowState.OnFixedUpdate();
        }
    }

    /*
    //===================================================
    /// <summary>
    /// 
    /// </summary>
    //===================================================
    public class Blackboard
    {
        Dictionary<string, object> _parameters = new();

        public void Set<T>(string key, T value)
        {
            _parameters[key] = value;
        }

        public T Get<T>(string key)
        {
            if (_parameters.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default;
        }
    }
    */
}