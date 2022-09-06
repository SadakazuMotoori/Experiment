using System.Collections;
using System.Collections.Generic;
using KdGame.Net;
using UnityEngine;
using TMPro;

public class LobbyScript : MonoBehaviour
{
    TMP_InputField m_InputField;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();
        if(_netMan)
        {
            AppManager.Instance.GetNetworkManager().InitializeNetworkManager();
        }

        m_InputField = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnChangePlayerName()
    {
        NetworkManager.Instance.SetPlayerName(m_InputField.text);
    }

    public void OnClickCreateRoom()
    {
        Debug.Log("CreateRoomâüÇ≥ÇÍÇΩ!");  // ÉçÉOÇèoóÕ

        NetworkManager.Instance.InitializeNetworkStatus("TestRoom");
    }
}
