using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
/// <summary>
/// ���L�f�[�^
/// </summary>
//==================================================
public class Blackboard : MonoBehaviour
{
    // �p�����[�^���X�g
    [SerializeField] SerializableDictionary<string, FlexibleValue.Value> _parameters;

    // �l���Z�b�g
    public void SetValue<T>(string key, T value)
    {
        // �擾
        FlexibleValue.Value fv;
        _parameters.TryGetValue(key, out fv);

        // �����ꍇ�͐V�K�쐬
        if(fv == null)
        {
            fv = new();
            _parameters[key] = fv;
        }

        // �l���Z�b�g
        fv.SetValue(value);
    }

    // �l���擾
    public T GetValue<T>(string key)
    {
        // �擾
        FlexibleValue.Value fv;
        _parameters.TryGetValue(key, out fv);

        // ���݂��Ȃ��ꍇ�́A�f�t�H���g�l��Ԃ�
        if (fv == null) return default;

        // �l���擾���Ԃ�
        return fv.GetValue<T>();
    }

}
