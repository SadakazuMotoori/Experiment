using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Cysharp.Threading.Tasks;

public class FadeInOutContoller : MonoBehaviour
{
    UnityEngine.UI.Image _image;

    // 現在実行中のTween
    Tween _nowTween;

    void Awake()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
    }

    public async UniTask FadeIn(float duration)
    {
        // 現在補間中なら停止
        _nowTween.Kill();
        // 補間開始
        _nowTween = _image.DOFade(0.0f, duration);
        // 終了待ち
        await _nowTween;
    }

    public async UniTask FadeOut(float duration)
    {
        // 現在補間中なら停止
        _nowTween.Kill();
        // 補間開始
        _nowTween = _image.DOFade(1.0f, duration);
        // 終了待ち
        await _nowTween;
    }

    public async UniTask WaitForCompletion()
    {
        if (_nowTween.IsActive() == false) return;

        await _nowTween;
    }
}
