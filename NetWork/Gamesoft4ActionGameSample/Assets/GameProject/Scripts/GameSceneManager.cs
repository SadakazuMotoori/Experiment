using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AddressableAssets;

using Cysharp.Threading.Tasks;

using KdGame.Net;
using Photon.Pun;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [SerializeField] Character.CharacterBrain _playerChara;

    float _time;

    List<Character.CharacterBrain> _characterList;

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
        if (Instance != null) return;

        Instance = this;

        _characterList = new List<Character.CharacterBrain>();
    }

    void Start()
    {
        _time = Time.time;

        if (PhotonNetwork.IsMasterClient)
        {
            // プレイヤー分キャラを作成する
            for (short i = 0; i < NetworkManager.Instance.GetMemberNum(); i++)
            {
                NetworkManager.stCreateCharacterParameter _createCharParam = new NetworkManager.stCreateCharacterParameter();
                _createCharParam.pos        = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
                _createCharParam.name       = NetworkManager.Instance.GetPlayerName(i);
                _createCharParam.teamid     = 0;
                _createCharParam.hp         = 100;
                _createCharParam.playerid   = i;

                NetworkManager.Instance.CreateSendData(NetworkManager.ENETWORK_COMMAND.CMD_CREATECHARACTER, RpcTarget.All, _createCharParam);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - _time > 1.0f)
        {
            _time = Time.time;
        }
    }

    public async void OnCreateCharacter(NetworkManager.stCreateCharacterParameter aCreateCharParam)
    {
        /*
        var handle = Addressables.LoadAssetAsync<GameObject>("Character");
        GameObject asset = handle.WaitForCompletion();

        GameObject newObj = Instantiate(asset);
        */

        var handle = Addressables.InstantiateAsync("UnityChanDotNet", position: aCreateCharParam.pos, rotation: Quaternion.identity, parent: transform);

//      GameObject newObj = handle.WaitForCompletion();
        GameObject newObj = await handle.Task;
        newObj.GetComponent<MainObjectParameter>()._name        = aCreateCharParam.name;
        newObj.GetComponent<MainObjectParameter>()._hp          = aCreateCharParam.hp;
        newObj.GetComponent<MainObjectParameter>()._playerID    = aCreateCharParam.playerid;
        newObj.GetComponent<MainObjectParameter>()._teamID      = aCreateCharParam.teamid;

        _characterList.Add(newObj.GetComponent<Character.CharacterBrain>());
//      newObj.transform.position = new Vector3(x, 0, 0);
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

    public Character.CharacterBrain GetCharacterBrain(int aIndex)
    {
        return _characterList[aIndex];
    }
}
