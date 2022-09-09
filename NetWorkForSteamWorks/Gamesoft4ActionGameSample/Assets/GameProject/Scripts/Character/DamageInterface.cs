using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attack
{

    public struct DamageParam
    {
        // 通知データ
        public int DamageValue;
        public float HitStopDuration;
        public Vector3 Blow;
    }

    /// <summary>
    /// ダメージ適用インターフェース
    /// </summary>
    public interface IDamageApplicable
    {
        bool ApplyDamage(ref DamageParam param);
    }
}
