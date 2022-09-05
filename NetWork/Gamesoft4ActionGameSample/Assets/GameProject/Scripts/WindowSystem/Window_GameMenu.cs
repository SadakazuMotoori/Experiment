using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Cysharp.Threading.Tasks;

namespace WindowSystem
{

    public class Window_GameMenu : WindowBase
    {
        [SerializeField] UICursorController _cursorCtrl;

        public async UniTask Initialize()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            _frame.localScale = new Vector3(0.0f, 0.0f, 0.0f);

            _frame.DOScale(1.0f, 0.1f);
        }

        public async UniTask Execute()
        {
            var input = PlayerInputManager.Instance;

            while (true)
            {

                if (input.UI_GetButtonUp())
                {
                    _cursorCtrl.MoveSelection(-1);
                }
                if (input.UI_GetButtonDown())
                {
                    _cursorCtrl.MoveSelection(+1);
                }

                if (input.UI_GetButtonDecide())
                {
                    var trans = _cursorCtrl.GetNowSelected();
                    if (trans != null)
                    {
                        if (trans.name == "Menu01")
                        {
                            var wnd = await WindowSystem.WindowManager.Instance.CreateWindow<WindowSystem.Window_YesNo>("Window_YesNo",
                                async wnd =>
                                {
                                    await wnd.Initialize("ゲームソフト４コースです。\nこの時間は\nGameProgramming4の授業？");
                                });
                            if (await wnd.Execute() == false)
                            {
                                await CloseWindow();
                                return;
                            }
                        }
                        else
                        {
                            await CloseWindow();
                            return;
                        }

                    }
                }

                // 1フレーム待機
                await UniTask.DelayFrame(1);
            }
        }

    }
}