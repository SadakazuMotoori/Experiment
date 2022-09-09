using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStateMachine
{

    //===================================================
    /// <summary>
    /// AnimatorのStateから、キャラクターなどの独自ステートクラスを動かす
    /// </summary>
    //===================================================
    public class SMB_StateMachineNode : StateMachineBehaviour
    {
        // 独自ステートクラス
        [SerializeReference, SubclassSelector] StateNodeBase _state;
        // 設定されている独自ステートを、指定型で取得
        public T GetStateNode<T>() where T : StateNodeBase
        {
            return _state as T;
        }

//        StateMachineManager _stateMgr;

        // ※SetTriggerしても、すぐには遷移しなく再びOnStateUpdateのUpdateが呼ばれてしまうことはマズいので、その制御用。
//        bool _transitioned = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(_state.StateMgr == null)
            {
                var mgr = animator.GetComponent<StateMachineManager>();
                _state.Initialize(animator, mgr);
            }

            // 遷移済みをリセット
            //             _transitioned = false;

            //            _state.OnEnter();
            _state.StateMgr.ChangeState(_state);
        }

        /*
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 遷移済みの場合はスキップ
            if (_transitioned) return;

            // 遷移中の場合は、カレントステートのみ実行
            var currentInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentInfo.shortNameHash != stateInfo.shortNameHash) return;

            // 更新処理
            string trigger = _state.OnUpdate();

            // ステート変更指示あり
            if (string.IsNullOrEmpty(trigger) == false)
            {
                animator.SetTrigger(trigger);
                _transitioned = true;
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _state.OnExit();
        }
        */
        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }

}
