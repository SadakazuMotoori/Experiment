using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    //======================================================
    /// <summary>
    /// AI入力
    /// </summary>
    //======================================================
    public class StateBaseAIInputProvider : InputProvider
    {
        // 経路探索
        [SerializeField] PathFinding _pathFinding;
        public PathFinding PathFinding => _pathFinding;

        // 視界
//        [SerializeField] EyeSight _eyeSight;
//        public EyeSight EyeSight => _eyeSight;

        [SerializeField] TargetSeacher _objSeacher;
        public TargetSeacher ObjSeacher => _objSeacher;

        /*
        // 汎用パラメータ
        Blackboard _blackboard = new();
        public Blackboard BB => _blackboard;
        */

        // ステートマシン
        Animator _animator;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;

            /*
            // Animatorの全ステート設定
            var smbList = _animator.GetBehaviours<GameStateMachine.SMB_StateMachineNode>();
            foreach (var smb in smbList)
            {
                smb.GetStateNode<AIStateNodeBase>().Initialize(this);
            }
            */
        }

        void Update()
        {
            if (_pathFinding.IsArrived == false)
            {
                //                Vector3 dir = _pathFinding.NextPosition - transform.position;
                Vector3 dir = _pathFinding.DesiredVelocity;
                dir.y = 0;
                dir.Normalize();

                float dist = _pathFinding.RemainingDistanceWithStoppingDistance;
                if (dist < 3.0f)
                {
                    float rate = dist / 3.0f;
                    dir *= Mathf.Lerp(0.3f, 1.0f, rate); ;
                }

                AxisL = new Vector2(dir.x, dir.z);
            }
            else
            {
                AxisL = Vector3.zero;
            }
        }


        public Vector2 AxisL { get; set; }
        public override Vector2 GetAxisL() => AxisL;

        public bool ButtonAttack { get; set; }
        public override bool GetButtonAttack() => ButtonAttack;
    }

    /// <summary>
    /// SMB_AIStateNodeに接続するクラス
    /// </summary>
    [System.Serializable]
    public class AIStateNodeBase : GameStateMachine.StateNodeBase
    {
        StateBaseAIInputProvider _stateBaseAI;
        public StateBaseAIInputProvider StateBaseAI => _stateBaseAI;

        public override void OnEnter()
        {
            base.OnEnter();

            _stateBaseAI = StateMgr.Blackboard.GetValue<StateBaseAIInputProvider>("AIInput");

            // AI用データをBlackboardに追加
            _aiData = StateMgr.Blackboard.GetValue<AIData>("AIData");
            if (_aiData == null)
            {
                _aiData = new AIData();
                StateMgr.Blackboard.SetValue("AIData", _aiData);
            }
        }

        protected class AIData
        {
            public MainObjectParameter Target;
        }
        // (共有)AIデータ
        protected AIData _aiData;
    }

    /*

    public class Blackboard
    {
        Dictionary<string, object> _parameters = new();

        public void Set<T>(string key, T value)
        {
            _parameters[key] = value;
        }

        public T Get<T>(string key)
        {
            if(_parameters.TryGetValue(key, out var value))
            {
                return (T)value;
            }
            return default;
        }
    }
    */
}
