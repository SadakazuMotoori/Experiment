using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Cysharp.Threading.Tasks;


public class AssetBundleManager : MonoBehaviour
{
    public static AssetBundleManager Instance { get; private set; }

    List<AsyncOperationHandle> _manageAssetHandles = new();

    void Awake()
    {
        if (Instance != null) return;
        Instance = this;

    }

    public T LoadAsset<T>(object key)
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        handle.WaitForCompletion();

        _manageAssetHandles.Add(handle);

        return handle.Result;
    }

    public async UniTask<T> LoadAssetAsync<T>(object key)
    {
        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task;

        _manageAssetHandles.Add(handle);

        return handle.Result;
    }

    public async UniTask<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> ChangeSceneAsync(object key)
    {
        var handle = Addressables.LoadSceneAsync(key);
        await handle.Task;

        return handle.Result;
    }

    public void ReleaseAllHandles()
    {
        foreach(AsyncOperationHandle handle in _manageAssetHandles)
        {
            Addressables.Release(handle);
        }
        _manageAssetHandles.Clear();
    }
}
