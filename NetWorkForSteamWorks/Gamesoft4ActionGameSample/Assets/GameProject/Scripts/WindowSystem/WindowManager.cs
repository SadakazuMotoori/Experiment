using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using UniRx;
using UniRx.Triggers;

namespace WindowSystem
{
    public class WindowManager : MonoBehaviour
    {
        [SerializeField] Transform _contents;

        [SerializeField] FadeInOutContoller _fadeInOut;
        public FadeInOutContoller FadeInOut => _fadeInOut;

        [SerializeField] GameObject _nowLoading;
        public void ShowNowLoading(bool enable)
        {
            _nowLoading.SetActive(enable);
        }

        public static WindowManager Instance { get; private set; }

        void Awake()
        {
            // すでに存在する
            if (Instance != null)
            {
                // contentsの中だけ移動して削除する
                foreach(Transform content in _contents)
                {
                    content.SetParent(Instance._contents, false);
                }

                // 削除
                Destroy(gameObject);
                return;
            }
            Instance = this;
//            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // 子の数が変更されたら、入力を切り替える
            this.ObserveEveryValueChanged(count => _contents.transform.childCount)
                .Subscribe(_ =>
                {
                    if (_contents.transform.childCount == 0)
                    {
                        PlayerInputManager.Instance.ChangeActionMap("GamePlay");
                    }
                    else
                    {
                        PlayerInputManager.Instance.ChangeActionMap("UI");
                    }
                });

            // フェード機能有効
            _fadeInOut.gameObject.SetActive(true);
            _fadeInOut.FadeIn(1.0f).Forget();
        }

        public async UniTask<WindowType> CreateWindow<WindowType>(string name, System.Func<WindowType, UniTask> initProc)
        {
            // ウィンドウ作成
            GameObject obj = await UnityEngine.AddressableAssets.Addressables.InstantiateAsync(name, parent: _contents);

            // ウィンドウ初期設定
            obj.SetActive(false);

            var wnd = obj.GetComponent<WindowType>();
            if (initProc != null)
            {
                await initProc(wnd);
            }

            obj.SetActive(true);

            return wnd;
        }

        // 
        public void DeleteAllWindow()
        {
            foreach(Transform child in transform)
            {
                var wnd = child.GetComponent<WindowBase>();
                if (wnd != null)
                {
                    wnd.CloseWindow().Forget();
                }
            }
        }

    }
}