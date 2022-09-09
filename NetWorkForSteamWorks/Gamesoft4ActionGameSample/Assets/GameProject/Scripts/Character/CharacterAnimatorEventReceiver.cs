using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{

    public class CharacterAnimatorEventReceiver : MonoBehaviour
    {
        // 
        [SerializeField] CharacterBrain _brain;
        // ��������I�u�W�F�N�g�̃e���v���[�g��������GameObject
        [SerializeField] Transform _templateObjGroup;

        Animator _animator;

        List<GameObject> _attackObjectList = new();

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void OnAnimatorMove()
        {
            _brain.Move(_animator.deltaPosition);
        }

        public void OnCallChangeFace(string name)
        {
            const int layerNo = 1;

            if (string.IsNullOrEmpty(name) == false)
            {
                _animator.SetLayerWeight(layerNo, 1.0f);
                _animator.CrossFade(name, 0, layerNo);
            }
        }

        /// <summary>
        /// �A�j���C�x���g�F�U������o��
        /// </summary>
        /// <param name="arg"></param>
        public void AnimEvent_Attack(string arg)
        {
            Transform found = _templateObjGroup.Find(arg);
            if (found != null)
            {
                // ����
                var obj = Instantiate(found.gameObject, _brain.transform);
                _attackObjectList.Add(obj);
            }
        }

        public void AnimEvent_ClearAttackObject()
        {
            foreach (var obj in _attackObjectList)
            {
                if (obj == null) continue;
                Destroy(obj);
            }
            _attackObjectList.Clear();
        }

        public void AnimEvent_AddForce(string strVec)
        {
            string[] param = strVec.Split(',');


            Vector3 v = new();
            v.x = float.Parse(param[0]);
            v.y = float.Parse(param[1]);
            v.z = float.Parse(param[2]);

            v = _brain.transform.rotation * v;

            _brain.AddForce(v);
        }
    }
}