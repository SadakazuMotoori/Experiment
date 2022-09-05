using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

namespace GameAI
{
    //=======================================================
    /// <summary>
    /// �o�H�T��
    /// </summary>
    //=======================================================
    public class PathFinding : MonoBehaviour
    {
        // NavMesh�G�[�W�F���g�F���ۂɌo�H�T�����s������
        NavMeshAgent _navAgent;
//        public NavMeshAgent NavAgent => _navAgent;

        // ����������
        public bool IsArrived
        {
            get
            {
                // �o�H�T�����������Ă��Ȃ��ꍇ�͖����B
                if (_navAgent.pathPending) return false;
                // ��~���͓��B
                if (_navAgent.isStopped) return true;
                // ��~�����ȓ��ɗ�����A���������Ƃ���
                return _navAgent.remainingDistance <= _navAgent.stoppingDistance;
            }
        }

        // �o�H�T�����~����
        public void Stop()
        {
            _navAgent.isStopped = true;
        }

        // ���[�v�ړ�������
        public void WarpPosition(Vector3 pos)
        {
            _navAgent.Warp(pos);
        }

        // �ړI�n�̐ݒ�
        public void SetDestination(Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas) == false)
            {
                return;
            }

            // ��~������
            _navAgent.isStopped = false;
            // �ړI�n�ݒ�
            _navAgent.SetDestination(position);
        }

        // ���̖ڕW���W���擾
        public Vector3 NextPosition => _navAgent.steeringTarget;

        // 
        public Vector3 DesiredVelocity => _navAgent.desiredVelocity;

        // �ړI�n�ւ̎c�苗�����擾�@�R�[�i�[������ꍇ��infinity���A��
        public float RemainingDistance => _navAgent.remainingDistance;

        // �ړI�n�ւ̎c�苗�����擾(��~����������)�@�R�[�i�[������ꍇ��infinity���A��
        public float RemainingDistanceWithStoppingDistance => Mathf.Max(0.0f, _navAgent.remainingDistance - _navAgent.stoppingDistance);

        void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();

            // Transform�������ōX�V����Ȃ��悤�ɂ���
//            _navAgent.speed = 0;
            _navAgent.angularSpeed = 0;
            _navAgent.acceleration = 0;
            _navAgent.updatePosition = false;
            _navAgent.updateRotation = false;

            _navAgent.isStopped = true;
        }

        void Update()
        {
            // �o�H�T�����I����Ă��Ȃ��ꍇ�́A�������Ȃ�
            if (_navAgent.pathPending) return;

            // ���B���Ă���ꍇ�́A��~
            if (IsArrived)
            {
                _navAgent.isStopped = true;
            }

            // ���쒆�Ȃ�AnextPosition���ړ������Ȃ��悤�ɂ���
            if (_navAgent.isStopped == false)
            {
                _navAgent.nextPosition = transform.position;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_navAgent)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(NextPosition, 0.3f);

                Gizmos.color = Color.red;

                int i = 0;
                Vector3 prevPos = new Vector3();
                foreach (Vector3 pos in _navAgent.path.corners)
                {
                    Gizmos.DrawWireSphere(pos, 0.2f);

                    if (i == 0)
                    {
                        prevPos = pos;
                    }
                    else
                    {
                        Gizmos.DrawLine(prevPos, pos);
                        prevPos = pos;
                    }
                    i++;
                }

                //            Gizmos.DrawWireSphere(_nextPosition, 0.2f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_navAgent.nextPosition, _navAgent.nextPosition + _navAgent.velocity);

            }
        }
#endif
    }
}
