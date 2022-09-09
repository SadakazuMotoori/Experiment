using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Cysharp.Threading.Tasks;

namespace WindowSystem
{

    public class Window_YesNo : WindowBase
    {
        [SerializeField] TMPro.TextMeshProUGUI _textMessage;
        [SerializeField] UnityEngine.UI.Button _btn_Yes;
        [SerializeField] UnityEngine.UI.Button _btn_No;

        [SerializeField] UICursorController _cursorCtrl;

//        bool? _isYes = null;

        public async UniTask Initialize(string message)
        {
            _textMessage.text = message;

            // Ç»Ç…Ç©ÇΩÇ≠Ç≥ÇÒì«Ç›çûÇﬁ
            await UniTask.Delay(200);

            /*
            _btn_Yes.onClick.AddListener(() =>
            {
                Debug.Log("1");
                _isYes = true;
            //            Destroy(gameObject);
            });

                _btn_No.onClick.AddListener(() =>
                {
                    Debug.Log("2");
                    _isYes = false;
                //            Destroy(gameObject);
            });
            */
        }


        // Start is called before the first frame update
        void Start()
        {
            _frame.localScale = new Vector3(0.0f, 0.0f, 0.0f);

            _frame.DOScale(1.0f, 0.1f);
        }

        public async UniTask<bool> Execute()
        {
            var input = PlayerInputManager.Instance;

            while (true)
            {
                /*
                if (_isYes.HasValue)
                {
                    await CloseWindow();
                    return _isYes.Value;
                }
                */

                if (input.UI_GetButtonLeft())
                {
                    _cursorCtrl.MoveSelection(-1);
                }
                if (input.UI_GetButtonRight())
                {
                    _cursorCtrl.MoveSelection(+1);
                }

                if (input.UI_GetButtonDecide())
                {
                    var trans = _cursorCtrl.GetNowSelected();
                    if (trans != null)
                    {
                        await CloseWindow();

                        if (trans.name == "Btn_Yes") return true;
                        else return false;
                    }
                    break;
                }

                // 1ÉtÉåÅ[ÉÄë“ã@
                await UniTask.DelayFrame(1);
            }

            return false;
        }

    }
}