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

    // �w��obj���o�^�ς݂��H
    public bool Exist(MainObjectParameter obj)
    {
        return _hitObjects.ContainsKey(obj);
    }

    // �q�b�g�o�^
    public void Register(MainObjectParameter objID, float duration)
    {
        _hitObjects[objID] = duration;
    }

    // �o�^���X�g���N���A����
    public void Reset()
    {
        _hitObjects.Clear();
    }

    public void Update()
    {
        // �q�b�g�X�g�b�v���͉������Ȃ�
        if (_mainObjParam != null)
        {
            if (_mainObjParam.HitStopTimer > 0) return;
        }

        // Dic�̃L�[��z��Ɉڂ�
        _tempList.Clear();
        foreach (var key in _hitObjects.Keys)
        {
            _tempList.Add(key);
        }

        // �S���e����������
        foreach(var key in _tempList)
        {
            // ���Ԃ����Z
            float v = _hitObjects[key];
            v -= Time.deltaTime;
            _hitObjects[key] = v;

            // ���Ԃ�������A�o�^����
            if (v <= 0)
            {
                _hitObjects.Remove(key);
            }
        }
    }
}
