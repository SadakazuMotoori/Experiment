using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;

using Cysharp.Threading.Tasks;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] Character.CharacterBrain _playerChara;

    float _time;

    public async UniTask GameClear()
    {
        _playerChara.DoWin();

        await UniTask.Delay(7000);

        // フェードアウト
        await WindowSystem.WindowManager.Instance.FadeInOut.FadeOut(1.0f);

        await AppManager.Instance.ChangeScene("TitleScene");
        /*
        // シーン切り替え
        await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync("TitleScene");

        // 使用していないアセットを解放
        await Resources.UnloadUnusedAssets();
        // GCのメモリ解放
        System.GC.Collect();
        */

        // フェードイン
        WindowSystem.WindowManager.Instance.FadeInOut.FadeIn(1.0f).Forget();
        
    }

    // Start is called before the first frame update
    void Awake()
    {
    }

    void Start()
    {
        _time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - _time > 1.0f)
        {
            _time = Time.time;

        }
    }

    public async void OnCreateCharacter()
    {
        /*
        var handle = Addressables.LoadAssetAsync<GameObject>("Character");
        GameObject asset = handle.WaitForCompletion();

        GameObject newObj = Instantiate(asset);
        */

        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);

        var handle = Addressables.InstantiateAsync("UnityChan_Enemy", position: new Vector3(x,0,z), rotation: Quaternion.identity, parent: transform);

        //        GameObject newObj = handle.WaitForCompletion();
        GameObject newObj = await handle.Task;
        newObj.GetComponent<MainObjectParameter>().TeamID = 0;

//        newObj.transform.position = new Vector3(x, 0, 0);
    }

    public async void OnCreateEnemy()
    {
        float x = Random.Range(-5, 5);
        float z = Random.Range(-5, 5);

        var handle = Addressables.InstantiateAsync("UnityChan_Enemy", position: new Vector3(x, 0, z), rotation: Quaternion.identity, parent: transform);

        GameObject newObj = await handle.Task;
    }

    public async void OnLoadScene()
    {
        Addressables.LoadSceneAsync("Lesson01_Addressables");
    }

}
