using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FlexibleValue
{
    /// <summary>
    /// �l�f�[�^�̊�{�N���X
    /// �V���A���C�Y�\�ɂ��邽�߁A������p�����Ēl�̎��Ԃ�p�ӂ���
    /// </summary>
    [System.Serializable]
    public class FVBase
    {
        // �l���擾
        public virtual object GetValue() { return null; }
        // �l���Z�b�g
        public virtual void SetValue(object value) { }
    }

    // ���W�F�l���b�N�͎g�p���Ȃ�
    // ���EInvokeMember����₱�����Ȃ�
    // ���ESerializeReflection�̃V���A���C�Y���ł��Ȃ��Ȃ�

    // int
    [System.Serializable]
    public class FVInt : FVBase
    {
        // �l�{��
        [SerializeField] int _value = 0;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is int v) { _value = v; } }

        public static System.Type GetValueType() => typeof(int);
    }

    // float
    [System.Serializable]
    public class FVFloat : FVBase
    {
        // �l�{��
        [SerializeField] float _value = 0;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is float v) { _value = v; } }
        public static System.Type GetValueType() => typeof(float);
    }

    // string
    [System.Serializable]
    public class FVString : FVBase
    {
        // �l�{��
        [SerializeField] string _value = "";

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is string v) { _value = v; } }
        public static System.Type GetValueType() => typeof(string);
    }

    // vector3
    [System.Serializable]
    public class FVVector3 : FVBase
    {
        // �l�{��
        [SerializeField] Vector3 _value = new();

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is Vector3 v) { _value = v; } }
        public static System.Type GetValueType() => typeof(Vector3);
    }

    // UnityEngine.Object
    [System.Serializable]
    public class FVObject : FVBase
    {
        // �l�{��
        [SerializeField] Object _value = null;

        public override object GetValue() { return _value; }
        public override void SetValue(object value) { if (value is Object v) { _value = v; } }
        public static System.Type GetValueType() => typeof(Object);
    }

    // ���̑��Aclass�n
    [System.Serializable]
    public class FVClass : FVBase
    {
        // �l�{��
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
        // �ėp�^
        [SerializeReference, SubclassSelector] FVBase _fv = new FVInt();

        // FV�n�̌^���
        #region FVBase�h���̌^���擾��쐬
        static List<System.Type> s_valueClassTypes = null;

        public FVBase CreateFrom<T>()
        {
            // FVBase�̔h���N���X�����擾(����̂�)
            if (s_valueClassTypes == null)
            {
                s_valueClassTypes = Assembly.GetAssembly(typeof(FVBase)).GetTypes().Where(t =>
                {
                    return t.IsSubclassOf(typeof(FVBase)) && !t.IsAbstract;
                }).ToList();
            }

            System.Type targetType = typeof(T);
            // FVBase�̔h���N���X�́A�ǂ�ɑ������邩����
            foreach (var type in s_valueClassTypes)
            {
                // �����̃f�[�^�^���擾
                var t = (System.Type)type.InvokeMember("GetValueType", BindingFlags.InvokeMethod, null, null, null);

                // ���S��v���H
                if (targetType == t)
                {
                    return (FVBase)System.Activator.CreateInstance(type);
                }
                // �h���N���X���H
                else if (targetType.IsSubclassOf(t))
                {
                    return (FVBase)System.Activator.CreateInstance(type);
                }
            }
            return null;
        }
        #endregion

        // �f�[�^���w��^�Ŏ擾����@�^���Ⴄ�ꍇ��default�l���Ԃ�
        public T GetValue<T>()
        {
            if (_fv.GetValue() is T v) return v;
            return default;
        }

        // �w��^�̃f�[�^���Z�b�g����@�^���Ⴄ�ꍇ�́A�����ł��̌^�ɕύX�����
        public void SetValue<T>(T value)
        {
            // ���쐬�Ȃ�A�쐬
            if (_fv == null)
            {
                _fv = CreateFrom<T>();
            }
            // �^�`�F�b�N
            else if(_fv.GetValue() is T == false)
            {
                _fv = CreateFrom<T>();
            }

            // �l���Z�b�g
            _fv.SetValue(value);

        }

    }

}