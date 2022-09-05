using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverCharacterPosition : MonoBehaviour
{
    [SerializeField] Transform _recoverPos;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<MainObjectParameter>() == null) return;

        other.gameObject.SetActive(false);
        other.transform.position = _recoverPos.position;
        other.gameObject.SetActive(true);
    }
}
