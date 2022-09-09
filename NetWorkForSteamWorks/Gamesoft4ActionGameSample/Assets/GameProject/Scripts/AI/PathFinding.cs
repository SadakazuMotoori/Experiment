using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

namespace GameAI
{
    //=======================================================
    /// <summary>
    /// 経路探索
    /// </summary>
    //=======================================================
    public class PathFinding : MonoBehaviour
    {
        // NavMeshエージェント：実際に経路探索を行うもの
        NavMeshAgent _navAgent;
//        public NavMeshAgent NavAgent => _navAgent;

        // 到着したか
        public bool IsArrived
        {
            get
            {
                // 経路探索が完了していない場合は未到達
                if (_navAgent.pathPending) return false;
                // 停止中は到達
                if (_navAgent.isStopped) return true;
                // 停止距離以内に来たら、到着したとする
                return _navAgent.remainingDistance <= _navAgent.stoppingDistance;
            }
        }

        // 経路探索を停止する
        public void Stop()
        {
            _navAgent.isStopped = true;
        }

        // ワープ移動させる
        public void WarpPosition(Vector3 pos)
        {
            _navAgent.Warp(pos);
        }

        // 目的地の設定
        public void SetDestination(Vector3 position)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas) == false)
            {
                return;
            }

            // 停止を解除
            _navAgent.isStopped = false;
            // 目的地設定
            _navAgent.SetDestination(position);
        }

        // 次の目標座標を取得
        public Vector3 NextPosition => _navAgent.steeringTarget;

        // 
        public Vector3 DesiredVelocity => _navAgent.desiredVelocity;

        // 目的地への残り距離を取得　コーナーがある場合はinfinityが帰る
        public float RemainingDistance => _navAgent.remainingDistance;

        // 目的地への残り距離を取得(停止距離を引く)　コーナーがある場合はinfinityが帰る
        public float RemainingDistanceWithStoppingDistance => Mathf.Max(0.0f, _navAgent.remainingDistance - _navAgent.stoppingDistance);

        void Awake()
        {
            _navAgent = GetComponent<NavMeshAgent>();

            // Transformが自動で更新されないようにする
//            _navAgent.speed = 0;
            _navAgent.angularSpeed = 0;
            _navAgent.acceleration = 0;
            _navAgent.updatePosition = false;
            _navAgent.updateRotation = false;

            _navAgent.isStopped = true;
        }

        void Update()
        {
            // 経路探索が終わっていない場合は、何もしない
            if (_navAgent.pathPending) return;

            // 到達している場合は、停止
            if (IsArrived)
            {
                _navAgent.isStopped = true;
            }

            // 動作中なら、nextPositionを移動させないようにする
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
