using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitManager : ObjectMonoBehaviour
{
    Dictionary<MainObjectParameter, float> _hitObjects = new();

    List<MainObjectParameter> _tempList = new(10);

    MainObjectParameter _mainObjParam;

    void Awake()
    {
        TryGetComponent(out _mainObjParam);
    }

    // 指定objが登録済みか？
    public bool Exist(MainObjectParameter obj)
    {
        return _hitObjects.ContainsKey(obj);
    }

    // ヒット登録
    public void Register(MainObjectParameter objID, float duration)
    {
        _hitObjects[objID] = duration;
    }

    // 登録リストをクリアする
    public void Reset()
    {
        _hitObjects.Clear();
    }

    public void Update()
    {
        // ヒットストップ中は何もしない
        if (_mainObjParam != null)
        {
            if (_mainObjParam.HitStopTimer > 0) return;
        }

        // Dicのキーを配列に移す
        _tempList.Clear();
        foreach (var key in _hitObjects.Keys)
        {
            _tempList.Add(key);
        }

        // 全内容を処理する
        foreach(var key in _tempList)
        {
            // 時間を減算
            float v = _hitObjects[key];
            v -= Time.deltaTime;
            _hitObjects[key] = v;

            // 時間が来たら、登録解除
            if (v <= 0)
            {
                _hitObjects.Remove(key);
            }
        }
    }
}
