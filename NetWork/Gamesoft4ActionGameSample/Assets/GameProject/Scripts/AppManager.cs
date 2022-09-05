using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

[DefaultExecutionOrder(-100)]
public class AppManager : MonoBehaviour
{

    /*
    // BeforeSceneLoad��Awake���O�Ɏ��s�����
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

        // 60fps�Œ�
        //        Application.targetFrameRate = 60;

        // ���񃍁[�h
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

        // �V�[���؂�ւ�
        //        var handle = UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(key);
        //        await handle.Task;
        await AssetBundleManager.Instance.ChangeSceneAsync(key);


        // �g�p���Ă��Ȃ��A�Z�b�g�����
        await Resources.UnloadUnusedAssets();
        // GC�̃��������
        System.GC.Collect();

        await UniTask.Delay(500);

        WindowSystem.WindowManager.Instance.ShowNowLoading(false);
    }
}
