using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
/// <summary>
/// 共有データ
/// </summary>
//==================================================
public class Blackboard : MonoBehaviour
{
    // パラメータリスト
    [SerializeField] SerializableDictionary<string, FlexibleValue.Value> _parameters;

    // 値をセット
    public void SetValue<T>(string key, T value)
    {
        // 取得
        FlexibleValue.Value fv;
        _parameters.TryGetValue(key, out fv);

        // 無い場合は新規作成
        if(fv == null)
        {
            fv = new();
            _parameters[key] = fv;
        }

        // 値をセット
        fv.SetValue(value);
    }

    // 値を取得
    public T GetValue<T>(string key)
    {
        // 取得
        FlexibleValue.Value fv;
        _parameters.TryGetValue(key, out fv);

        // 存在しない場合は、デフォルト値を返す
        if (fv == null) return default;

        // 値を取得し返す
        return fv.GetValue<T>();
    }

}
