using System.Collections;
using System.Collections.Generic;
using TMPro;
using KdGame.Net;
using UnityEngine;

public class WaitingRoomScript : MonoBehaviour
{
    public Canvas   m_Canvas;
    private int     m_NowMemberCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        // ŽQ‰ÁŽÒˆê——
        int _memberNum = NetworkManager.Instance.GetMemberNum();

        for (int i = 0; i < _memberNum; i++)
        {
            foreach(Transform child in m_Canvas.transform)
            {
                if (child.name == "Member")
                {
                    TMP_Text _text;
                    _text = child.GetComponent<TMP_Text>();

                    string _nowText = _text.text;
                    _nowText += NetworkManager.Instance.GetMemberName(i) + "\n";
                    _text.text = _nowText;
                }
            }
        }
        m_NowMemberCount = _memberNum;
    }

    // Update is called once per frame
    void Update()
    {
        // ŽQ‰ÁŽÒˆê——
        int _memberNum = NetworkManager.Instance.GetMemberNum();

        if (m_NowMemberCount != _memberNum)
        {
            foreach (Transform child in m_Canvas.transform)
            {
                if (child.name == "Member")
                {
                    TMP_Text _text;
                    _text = child.GetComponent<TMP_Text>();
                    _text.text = "";

                    break;
                }
            }

            for (int i = 0; i < _memberNum; i++)
            {
                foreach (Transform child in m_Canvas.transform)
                {
                    if (child.name == "Member")
                    {
                        TMP_Text _text;
                        _text = child.GetComponent<TMP_Text>();

                        string _nowText = _text.text;
                        _nowText += NetworkManager.Instance.GetMemberName(i) + "\n";
                        _text.text = _nowText;
                    }
                }
            }
            m_NowMemberCount = _memberNum;
        }
    }
}
