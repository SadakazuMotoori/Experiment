using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    // ����
    public virtual Vector2 GetAxisL() => Vector2.zero;
    // �E��
    public virtual Vector2 GetAxisR() => Vector2.zero;

    public virtual bool GetButtonAttack() => false;
}
