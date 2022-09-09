using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Cysharp.Threading.Tasks;

using UnityEngine.AddressableAssets;

public class EnemyPop : MonoBehaviour
{
    [SerializeField] int _maxPopup = 5;

    List<GameObject> _enemies = new();

//    System.IDisposable _updateDisposable;

    void Start()
    {
        /*
        _updateDisposable = Observable.Interval(System.TimeSpan.FromSeconds(5))
            .Subscribe(async _ =>
            {
                if (_maxPopup <= 0)
                {
                    var gameScene = GetComponentInParent<GameSceneManager>();
                    if (gameScene != null)
                    {
                        gameScene.GameClear();
                    }

                    // ����Interval���~����
                    _updateDisposable.Dispose();

                    return;
                }

                if (isActiveAndEnabled == false) return;

//                float x = Random.Range(-5, 5);
//                float z = Random.Range(-5, 5);

                var handle = Addressables.InstantiateAsync("UnityChan_Enemy", parent: transform);

                GameObject newObj = await handle.Task;

                _maxPopup--;

            }).AddTo(this);
        */
        UpdateAsync().Forget();
    }

    async UniTask UpdateAsync()
    {
        var cancelToken = this.GetCancellationTokenOnDestroy();

        await UniTask.Delay(3000);
        if (cancelToken.IsCancellationRequested) return;

        // �G�𐶂ݏo��
        while (_maxPopup >= 1)
        {

            var handle = Addressables.InstantiateAsync("UnityChan_Enemy", parent: transform);

            GameObject newObj = await handle.Task;
            _enemies.Add(newObj);

            _maxPopup--;

            await UniTask.Delay(5000);
            if (cancelToken.IsCancellationRequested) return;
        }

        await UniTask.Delay(3000);
        if (cancelToken.IsCancellationRequested) return;

        // �G�̐����`�F�b�N
        while (true)
        {
            bool isAlive = false;
            foreach (var obj in _enemies)
            {
                if(obj != null)
                {
                    // �����҂�����
                    isAlive = true;
                    break;
                }
            }

            if (isAlive == false) break;

            await UniTask.Delay(100);
            if (cancelToken.IsCancellationRequested) return;
        }

        // �Q�[���N���A�\
        var gameScene = GetComponentInParent<GameSceneManager>();
        if (gameScene != null)
        {
            gameScene.GameClear().Forget();
        }
    }
}
