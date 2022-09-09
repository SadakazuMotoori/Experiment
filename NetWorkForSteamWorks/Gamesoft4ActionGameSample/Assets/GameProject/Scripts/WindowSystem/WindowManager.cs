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
            // ���łɑ��݂���
            if (Instance != null)
            {
                // contents�̒������ړ����č폜����
                foreach(Transform content in _contents)
                {
                    content.SetParent(Instance._contents, false);
                }

                // �폜
                Destroy(gameObject);
                return;
            }
            Instance = this;
//            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            // �q�̐����ύX���ꂽ��A���͂�؂�ւ���
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

            // �t�F�[�h�@�\�L��
            _fadeInOut.gameObject.SetActive(true);
            _fadeInOut.FadeIn(1.0f).Forget();
        }

        public async UniTask<WindowType> CreateWindow<WindowType>(string name, System.Func<WindowType, UniTask> initProc)
        {
            // �E�B���h�E�쐬
            GameObject obj = await UnityEngine.AddressableAssets.Addressables.InstantiateAsync(name, parent: _contents);

            // �E�B���h�E�����ݒ�
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