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
                // ����
                if (input.UI_GetButtonDecide())
                {
                    // ���S�A�j��
                    _startLogoTween.Kill();
                    _ = _startLogo.DOScale(1.2f, 1.0f).SetEase(Ease.OutElastic);

                    // �t�F�[�h�A�E�g
                    await WindowManager.Instance.FadeInOut.FadeOut(1.0f);

                    await CloseWindow();

                    await AppManager.Instance.ChangeScene("LobbyScene");
 //                   await AppManager.Instance.ChangeScene("GameScene");

                    /*
                    // �V�[���؂�ւ�
                    var sceneInstance = await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("GameScene");

                    // �g�p���Ă��Ȃ��A�Z�b�g�����
                    await Resources.UnloadUnusedAssets();
                    // GC�̃��������
                    System.GC.Collect();
                    */

                    // �t�F�[�h�C��
                    WindowManager.Instance.FadeInOut.FadeIn(5.0f).Forget();

                    break;
                }

                // 1�t���[���ҋ@
                await UniTask.DelayFrame(1);
            }

        }
    }
}