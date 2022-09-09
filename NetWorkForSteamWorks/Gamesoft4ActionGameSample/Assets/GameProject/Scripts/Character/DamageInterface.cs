using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attack
{

    public struct DamageParam
    {
        // �ʒm�f�[�^
        public int DamageValue;
        public float HitStopDuration;
        public Vector3 Blow;
    }

    /// <summary>
    /// �_���[�W�K�p�C���^�[�t�F�[�X
    /// </summary>
    public interface IDamageApplicable
    {
        bool ApplyDamage(ref DamageParam param);
    }
}
