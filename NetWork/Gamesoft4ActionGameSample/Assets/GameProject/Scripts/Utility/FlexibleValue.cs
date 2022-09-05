using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FlexibleValue
{
    /// <summary>
    /// 値データの基本クラス
    /// シリアライズ可能にするため、これを継承して値の実態を用意する
    /// </summary>
    [System.Serializable]
    public class FVBase
    {
        // 値を取得
        public virtual object GetValue() { return null; }
        // 値をセット
        public virtual void SetValue(object value) { }
    }

    // ※ジェネリックは使用しない
    // ※・InvokeMemberがややこしくなる
    // ※・SerializeReflectionのシリアライズができなくなる

    // int
    [System.Serializable]
    public class FVInt : FVBase
    {
        // 値本体
        [SerializeField] int _value = 0;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is int v) { _value = v; } }

        public static System.Type GetValueType() => typeof(int);
    }

    // float
    [System.Serializable]
    public class FVFloat : FVBase
    {
        // 値本体
        [SerializeField] float _value = 0;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is float v) { _value = v; } }
        public static System.Type GetValueType() => typeof(float);
    }

    // string
    [System.Serializable]
    public class FVString : FVBase
    {
        // 値本体
        [SerializeField] string _value = "";

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is string v) { _value = v; } }
        public static System.Type GetValueType() => typeof(string);
    }

    // vector3
    [System.Serializable]
    public class FVVector3 : FVBase
    {
        // 値本体
        [SerializeField] Vector3 _value = new();

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is Vector3 v) { _value = v; } }
        public static System.Type GetValueType() => typeof(Vector3);
    }

    // UnityEngine.Object
    [System.Serializable]
    public class FVObject : FVBase
    {
        // 値本体
        [SerializeField] Object _value = null;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is Object v) { _value = v; } }
        public static System.Type GetValueType() => typeof(Object);
    }

    // その他、class系
    [System.Serializable]
    public class FVClass : FVBase
    {
        // 値本体
        [SerializeField] object _value = null;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is object v) { _value = v; } }

        public static System.Type GetValueType() => typeof(object);

    }

    //=======================================================
    /// <summary>
    /// 
    /// </summary>
    //=======================================================
    [System.Serializable]
    public class Value
    {
        // 汎用型
        [SerializeReference, SubclassSelector] FVBase _fv = new FVInt();

        // FV系の型情報
        #region FVBase派生の型情報取得や作成
        static List<System.Type> s_valueClassTypes = null;

        public FVBase CreateFrom<T>()
        {
            // FVBaseの派生クラス情報を取得(初回のみ)
            if (s_valueClassTypes == null)
            {
                s_valueClassTypes = Assembly.GetAssembly(typeof(FVBase)).GetTypes().Where(t =>
                {
                    return t.IsSubclassOf(typeof(FVBase)) && !t.IsAbstract;
                }).ToList();
            }

            System.Type targetType = typeof(T);
            // FVBaseの派生クラスの、どれに相当するか判定
            foreach (var type in s_valueClassTypes)
            {
                // 内部のデータ型を取得
                var t = (System.Type)type.InvokeMember("GetValueType", BindingFlags.InvokeMethod, null, null, null);

                // 完全一致か？
                if (targetType == t)
                {
                    return (FVBase)System.Activator.CreateInstance(type);
                }
                // 派生クラスか？
                else if (targetType.IsSubclassOf(t))
                {
                    return (FVBase)System.Activator.CreateInstance(type);
                }
            }
            return null;
        }
        #endregion

        // データを指定型で取得する　型が違う場合はdefault値が返る
        public T GetValue<T>()
        {
            if (_fv.GetValue() is T v) return v;
            return default;
        }

        // 指定型のデータをセットする　型が違う場合は、自動でその型に変更される
        public void SetValue<T>(T value)
        {
            // 未作成なら、作成
            if (_fv == null)
            {
                _fv = CreateFrom<T>();
            }
            // 型チェック
            else if(_fv.GetValue() is T == false)
            {
                _fv = CreateFrom<T>();
            }

            // 値をセット
            _fv.SetValue(value);

        }

    }

}