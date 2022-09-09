using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReleaseFromParent : MonoBehaviour
{
    void Awake()
    {
        transform.SetParent(null);        
    }

}
