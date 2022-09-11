using System.Collections;
using System.Collections.Generic;
using KdGame.Net;
using UnityEngine;
using TMPro;

public class LobbyScript : MonoBehaviour
{
    public Canvas m_Canvas;
    TMP_InputField m_InputField;

    void Awake()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();
        if (_netMan)
        {
            _netMan.InitializeManagerSystem();
        }

        m_InputField = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
    }

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();
        if(_netMan)
        {
            _netMan.InitializeNetWorkSystem();
        }
    }

    // Update is called once per frame
    void Update()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();

        if(!_netMan.IsNetworkSystemReady())
        {
            Debug.Log("�l�b�g���[�N�V�X�e�������������ł��I");
            return;
        }
    }

    public void OnChangePlayerName()
    {
        NetworkManager.Instance.SetMyPlayerName(m_InputField.text);
    }

    public async void OnClickCreateRoom()
    {
        Debug.Log("CreateRoom�����ꂽ!");

        foreach (Transform child in m_Canvas.transform)
        {
            if (child.name == "Button01" || child.name == "Button02")
            {
                child.gameObject.SetActive(false);
            }
        }

        NetworkManager.ENETWORK_ERROR_CODE _err;
        _err = await NetworkManager.Instance.CreateRoom("TestRoom");

        if(_err == NetworkManager.ENETWORK_ERROR_CODE.NET_ERR_NORE)
        {
            Debug.Log("���[���쐬�����I");
            await AppManager.Instance.ChangeScene("WaitRoomScene");
        }
        else
        {
            Debug.Log("���[���쐬���s�I");
            foreach (Transform child in m_Canvas.transform)
            {
                if (child.name == "Button01" || child.name == "Button02")
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }

    public async void OnClickJoinRoom()
    {
        Debug.Log("JoinRoom�����ꂽ!");

        foreach (Transform child in m_Canvas.transform)
        {
            if (child.name == "Button01" || child.name == "Button02")
            {
                child.gameObject.SetActive(false);
            }
        }

        NetworkManager.ENETWORK_ERROR_CODE _err;
        _err = await NetworkManager.Instance.JoinRoom("TestRoom");

        if (_err == NetworkManager.ENETWORK_ERROR_CODE.NET_ERR_NORE)
        {
            Debug.Log("���[���쐬�����I");
            await AppManager.Instance.ChangeScene("WaitRoomScene");
        }
        else
        {
            Debug.Log("���[���쐬���s�I");
            foreach (Transform child in m_Canvas.transform)
            {
                if (child.name == "Button01" || child.name == "Button02")
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
    }
}
