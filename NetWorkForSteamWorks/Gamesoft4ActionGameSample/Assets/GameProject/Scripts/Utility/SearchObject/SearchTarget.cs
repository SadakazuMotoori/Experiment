using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchTarget : ObjectMonoBehaviour
{
    public enum Types
    {
        CharaCenter,
        Face,
    }

    // Ží—Þ
    [SerializeField] Types _type;
    public Types Type => _type;


    MainObjectParameter _mainObjParam;
    public MainObjectParameter MainObjParam => _mainObjParam;

    void Awake()
    {
        _mainObjParam = GetComponentInParent<MainObjectParameter>();
    }
}
