using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ダメージ適用者
/// </summary>
public class DamageApplicant : ObjectMonoBehaviour
{
    [SerializeField] MainObjectParameter _mainObjParam;
    [SerializeField] float _lifespan = 1.0f;
    [SerializeField] float _damageRate = 1.0f;
    [SerializeField] float _hitInterval = 0.1f;

    [Header("ヒットストップ")]
    [SerializeField] float _hitStopDuration = 0.5f;

    [Header("ヒットスロー")]
    [SerializeField] float _hitSlow = 1.0f;
    [SerializeField] float _hitSlowDuration = 0.5f;
    [SerializeField] float _hitSlowStartDelayTime = 0.1f;

    [SerializeField] Vector3 _blow;


    // Start is called before the first frame update
    void Start()
    {
//        TryGetComponent(out _mainObjParam);
    }

    void Update()
    {
        // ヒットストップ中は何もしない
        if (_mainObjParam != null)
        {
            if (_mainObjParam.HitStopTimer > 0) return;
        }

        // 寿命
        _lifespan -= Time.deltaTime;
        if (_lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var attackHitMgr = GetComponentInParent<AttackHitManager>();
        if (attackHitMgr == null) return;

        foreach (var contact in collision.contacts)
        {
            // 同じキャラは無視
            var otherObjParam = collision.rigidbody.GetComponent<MainObjectParameter>();
            if (_mainObjParam == otherObjParam) continue;
            // 同じチームも無視
            if (_mainObjParam.TeamID == otherObjParam.TeamID) continue;

            // ヒット管理チェック
            if (attackHitMgr.Exist(otherObjParam)) continue;

            // ヒット時間を登録
            attackHitMgr.Register(otherObjParam, _hitInterval);

            // ダメージ通知先
            var dmgApp = otherObjParam.GetComponent<Attack.IDamageApplicable>();
            if (dmgApp != null)
            {
                Attack.DamageParam param = new();
                param.DamageValue = (int)(_mainObjParam.Str * _damageRate);
                param.HitStopDuration = _hitStopDuration;

                param.Blow = transform.rotation * _blow;

                if (dmgApp.ApplyDamage(ref param))
                {
                    if (_hitSlow < 1.0f)
                    {
                        TimeScaleManager.Instance.AddTimeScale(_hitSlow, _hitSlowDuration, _hitSlowStartDelayTime);
                    }

                    // ヒットストップ
                    _mainObjParam.HitStopTimer = _hitStopDuration;

                    // エフェクト
                    var asset = AssetBundleManager.Instance.LoadAsset<GameObject>("VFX_Hit");
                    var newObj = Instantiate(asset, position: contact.point, rotation: transform.rotation);

//                    var handle = UnityEngine.AddressableAssets.Addressables.InstantiateAsync("VFX_Hit", position: contact.point, rotation: transform.rotation);
//                    var newObj = handle.WaitForCompletion();
                }
            }

        }


    }
}
