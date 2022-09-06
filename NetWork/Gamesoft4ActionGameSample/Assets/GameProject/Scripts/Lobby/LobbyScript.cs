using System.Collections;
using System.Collections.Generic;
using KdGame.Net;
using UnityEngine;

public class LobbyScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();
        if(_netMan)
        {
            AppManager.Instance.GetNetworkManager().InitializeNetworkManager();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
