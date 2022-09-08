using Attack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MainObjectParameter))]
public class StageObjectBrain : MonoBehaviour, Attack.IDamageApplicable
{
    MainObjectParameter _mainObjParam;
    Rigidbody _rigidBody;

    public bool ApplyDamage(ref DamageParam param)
    {
//        transform.position += param.Blow * 0.1f;

        if (_rigidBody.isKinematic == false)
        {
            _rigidBody.AddForce(param.Blow, ForceMode.Impulse);
        }

        return true;
    }

    void Awake()
    {
        TryGetComponent(out _mainObjParam);
        TryGetComponent(out _rigidBody);
    }
}
