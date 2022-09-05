using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Cysharp.Threading.Tasks;

public class FadeInOutContoller : MonoBehaviour
{
    UnityEngine.UI.Image _image;

    // ���ݎ��s����Tween
    Tween _nowTween;

    void Awake()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
    }

    public async UniTask FadeIn(float duration)
    {
        // ���ݕ�Ԓ��Ȃ��~
        _nowTween.Kill();
        // ��ԊJ�n
        _nowTween = _image.DOFade(0.0f, duration);
        // �I���҂�
        await _nowTween;
    }

    public async UniTask FadeOut(float duration)
    {
        // ���ݕ�Ԓ��Ȃ��~
        _nowTween.Kill();
        // ��ԊJ�n
        _nowTween = _image.DOFade(1.0f, duration);
        // �I���҂�
        await _nowTween;
    }

    public async UniTask WaitForCompletion()
    {
        if (_nowTween.IsActive() == false) return;

        await _nowTween;
    }
}
