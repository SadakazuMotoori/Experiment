using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

using DG.Tweening;

namespace WindowSystem
{
    public class Window_Title : WindowBase
    {
        [SerializeField] RectTransform _startLogo;

        Tween _startLogoTween;

        void Start()
        {
            Execute().Forget();

            var sequence = DOTween.Sequence();
            sequence.Append(_startLogo.DOScale(1.1f, 0.5f).SetEase(Ease.InOutSine));
            sequence.Append(_startLogo.DOScale(1.0f, 0.5f).SetEase(Ease.InOutSine));
            sequence.SetLoops(-1);

            _startLogoTween = sequence;
        }

        async UniTask Execute()
        {
            var cancelToken = this.GetCancellationTokenOnDestroy();

            await WindowManager.Instance.FadeInOut.WaitForCompletion();

            var input = PlayerInputManager.Instance;

            while(cancelToken.IsCancellationRequested == false)
            {
                // 決定
                if (input.UI_GetButtonDecide())
                {
                    // ロゴアニメ
                    _startLogoTween.Kill();
                    _ = _startLogo.DOScale(1.2f, 1.0f).SetEase(Ease.OutElastic);

                    // フェードアウト
                    await WindowManager.Instance.FadeInOut.FadeOut(1.0f);

                    await CloseWindow();

                    await AppManager.Instance.ChangeScene("GameScene");

                    /*
                    // シーン切り替え
                    var sceneInstance = await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("GameScene");

                    // 使用していないアセットを解放
                    await Resources.UnloadUnusedAssets();
                    // GCのメモリ解放
                    System.GC.Collect();
                    */

                    // フェードイン
                    WindowManager.Instance.FadeInOut.FadeIn(5.0f).Forget();

                    break;
                }

                // 1フレーム待機
                await UniTask.DelayFrame(1);
            }

        }
    }
}