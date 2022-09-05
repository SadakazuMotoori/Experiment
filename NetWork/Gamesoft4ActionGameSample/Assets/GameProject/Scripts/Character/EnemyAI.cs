using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAI
{
    [System.Serializable]
    public class EnemyAIStateIdle : AIStateNodeBase
    {
        [SerializeField] float _chaseMaxDistance = 2.0f;

        float _lastAttackTime = 0;

        float _lastMoveTime = 0;

        public override void OnEnter()
        {
            base.OnEnter();

        }

        public override void OnUpdate()
        {
            // 
            if (Time.time > _lastMoveTime + 5)
            {
                _lastMoveTime = Time.time;

                Vector3 nextPos = StateBaseAI.transform.position;
                nextPos.x += Random.Range(-3, 3);
                nextPos.z += Random.Range(-3, 3);

                StateBaseAI.PathFinding.SetDestination(nextPos);
            }

            // ‹ß‚­‚Ì“G‚ðUŒ‚
            var node = StateBaseAI.ObjSeacher.GetClosestTarget();
            if (node.HasValue)
            {
                if (node.Value.Distance > _chaseMaxDistance || Time.time > _lastAttackTime + 3)
                {
                    _lastAttackTime = Time.time;
                    _aiData.Target = node.Value.Target.MainObjParam;
//                    StateMgr.Blackboard.SetValue("Target", node.Value.Target.ObjParam);
//                    StateBaseAI.BB.Set("Target", node.Value.Target.ObjParam);
                    Animator.SetTrigger("GoChase");
                    return;
                }
            }

        }
    }

    [System.Serializable]
    public class EnemyAIStateChase : AIStateNodeBase
    {
        float _timeStartTime = 0;

        public override void OnEnter()
        {
            base.OnEnter();

            _timeStartTime = Time.time;
        }

        public override void OnUpdate()
        {
            //            Debug.Log("Chase");

            //            var closestObj = StateBaseAI.EyeSight.GetClosestObject(false);
            var closestObj = _aiData.Target;// StateMgr.Blackboard.GetValue<ObjectParameter>("Target");
            if (closestObj != null)
            {
                StateBaseAI.PathFinding.SetDestination(closestObj.transform.position);
            }

            // ˆê’èŽžŠÔ‚Å‚ ‚«‚ç‚ß
            if (Time.time >= _timeStartTime + 10)
            {
                Animator.SetTrigger("GoIdle");
                return;
            }

            /*
            Vector3 dir = StateBaseAI.PathFinding.NextPosition - StateBaseAI.transform.position;
            dir.y = 0;
            dir.Normalize();

            float dist = StateBaseAI.PathFinding.RemainingDistanceWithStoppingDistance;
            if (dist < 3.0f)
            {
                float rate = dist / 3.0f;
                dir *= Mathf.Lerp( 0.3f, 1.0f, rate); ;
            }

            StateBaseAI.AxisL = new Vector2(dir.x, dir.z);
            */

            if (StateBaseAI.PathFinding.IsArrived)
            {
                Animator.SetTrigger("GoAttack");
            }

        }
    }

    [System.Serializable]
    public class EnemyAIStateAttack : AIStateNodeBase
    {
        Character.CharacterBrain _brain;

        public override void OnEnter()
        {
            base.OnEnter();

            StateBaseAI.ButtonAttack = true;

            _brain = StateBaseAI.GetComponentInParent<Character.CharacterBrain>();
        }

        public override void OnUpdate()
        {
            StateBaseAI.ButtonAttack = false;

            if (_brain.NowStateType != Character.CharacterBrain.NowStateTypes.Attack)
            {
                Animator.SetTrigger("GoIdle");
                return;
            }

        }
    }
}