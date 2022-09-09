using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KdGame.Net;

using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(-100)]
public class AppManager : MonoBehaviour
{
    /*
    // BeforeSceneLoadでAwakeより前に実行される
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        GameObject App = new GameObject();
    }
    */

    public static AppManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 60fps固定
        //        Application.targetFrameRate = 60;

        // 初回ロード
//        UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>("default");
    }

//    private readonly UnityEngine.Profiling.CustomSampler _fooSampler = UnityEngine.Profiling.CustomSampler.Create($"AppManager.ChangeScene");

    public async UniTask ChangeScene(string key)
    {
        WindowSystem.WindowManager.Instance.ShowNowLoading(true);

        //        await UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>("default");

        await UniTask.Delay(500);

        // 
        AssetBundleManager.Instance.ReleaseAllHandles();

        // シーン切り替え
        //        var handle = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(key);
        //        await handle.Task;
        await AssetBundleManager.Instance.ChangeSceneAsync(key);

        // 使用していないアセットを解放
        await Resources.UnloadUnusedAssets();
        // GCのメモリ解放
        System.GC.Collect();

        await UniTask.Delay(500);

        WindowSystem.WindowManager.Instance.ShowNowLoading(false);
    }

    public NetworkManager GetNetworkManager()
    {
        return NetworkManager.Instance;
    }
}
