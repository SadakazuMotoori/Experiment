using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameStateMachine
{

    //===================================================
    /// <summary>
    /// Animator��State����A�L�����N�^�[�Ȃǂ̓Ǝ��X�e�[�g�N���X�𓮂���
    /// </summary>
    //===================================================
    public class SMB_StateMachineNode : StateMachineBehaviour
    {
        // �Ǝ��X�e�[�g�N���X
        [SerializeReference, SubclassSelector] StateNodeBase _state;
        // �ݒ肳��Ă���Ǝ��X�e�[�g���A�w��^�Ŏ擾
        public T GetStateNode<T>() where T : StateNodeBase
        {
            return _state as T;
        }

//        StateMachineManager _stateMgr;

        // ��SetTrigger���Ă��A�����ɂ͑J�ڂ��Ȃ��Ă�OnStateUpdate��Update���Ă΂�Ă��܂����Ƃ̓}�Y���̂ŁA���̐���p�B
//        bool _transitioned = false;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(_state.StateMgr == null)
            {
                var mgr = animator.GetComponent<StateMachineManager>();
                _state.Initialize(animator, mgr);
            }

            // �J�ڍς݂����Z�b�g
            //             _transitioned = false;

            //            _state.OnEnter();
            _state.StateMgr.ChangeState(_state);
        }

        /*
        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // �J�ڍς݂̏ꍇ�̓X�L�b�v
            if (_transitioned) return;

            // �J�ڒ��̏ꍇ�́A�J�����g�X�e�[�g�̂ݎ��s
            var currentInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (currentInfo.shortNameHash != stateInfo.shortNameHash) return;

            // �X�V����
            string trigger = _state.OnUpdate();

            // �X�e�[�g�ύX�w������
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
