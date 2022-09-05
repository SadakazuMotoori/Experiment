using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Cysharp.Threading.Tasks;

namespace WindowSystem
{
    public class WindowBase : MonoBehaviour
    {
        [SerializeField] protected RectTransform _frame;

        public virtual async UniTask CloseWindow()
        {
            await _frame.DOScale(0.0f, 0.1f);
            Destroy(gameObject);
        }
    }
}