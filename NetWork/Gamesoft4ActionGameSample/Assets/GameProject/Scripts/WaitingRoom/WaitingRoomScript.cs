using System.Collections;
using System.Collections.Generic;
using TMPro;
using KdGame.Net;
using UnityEngine;

public class WaitingRoomScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // ŽQ‰ÁŽÒˆê——
        int _memberNum = NetworkManager.Instance.GetMemberNum();

        for(int i = 0; i < _memberNum; i++)
        {
            TextMeshPro _text;
            _text = GameObject.Find("Canvas").GetComponent<TextMeshPro>();

            if (_text)
            {
                string _nowText = _text.text;
                _nowText += NetworkManager.Instance.GetMemberName(i) + "/n";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
