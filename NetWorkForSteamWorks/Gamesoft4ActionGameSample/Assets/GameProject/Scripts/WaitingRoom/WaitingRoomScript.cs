using UnityEngine;
using System;

using KdGame.Net;
using TMPro;


public class WaitingRoomScript : MonoBehaviour
{
    DateTime m_UnixEpoch            = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public Canvas   m_Canvas;
    private int     m_NowMemberCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();

        int _memberNum = _netMan.GetInRoomMemberNum();
        for (int i = 0; i < _memberNum; i++)
        {
            foreach (Transform child in m_Canvas.transform)
            {
                if (child.name == "Member")
                {
                    TMP_Text _text;
                    _text = child.GetComponent<TMP_Text>();

                    string _nowText = _text.text;
                    _nowText += NetworkManager.Instance.GetMemberNameByIndex(i) + "\n";
                    _text.text = _nowText;
                }

                if (!_netMan.IsMasterClient())
                {
                    if (child.name == "Button")
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
        m_NowMemberCount = _memberNum;
    }

    // Update is called once per frame
    async void Update()
    {
        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();
        int _memberNum = _netMan.GetInRoomMemberNum();

        long _count = _netMan.GetVersatileCount();
        if (_count == 0)
        {
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
                            _nowText += NetworkManager.Instance.GetMemberNameByIndex(i) + "\n";
                            _text.text = _nowText;
                        }
                    }
                }
                m_NowMemberCount = _memberNum;
            }
        }
        else
        {
            long time = (_count - (long)(DateTime.Now - m_UnixEpoch).TotalSeconds);

            if (time < 0)
            {
                _netMan.ResetVersatileCount();
                Destroy(this);
                await AppManager.Instance.ChangeScene("GameScene");
            }
            else
            {
                foreach (Transform child in m_Canvas.transform)
                {
                    if (child.name == "Time")
                    {
                        TMP_Text _text;
                        _text = child.GetComponent<TMP_Text>();
                        _text.text = time.ToString();
                    }
                }
            }
        }
    }

    public void OnClickGoGame()
    {
        Debug.Log("GO‰Ÿ‚³‚ê‚½!");

        NetworkManager _netMan = AppManager.Instance.GetNetworkManager();

        foreach (Transform child in m_Canvas.transform)
        {
            if (child.name == "Button")
            {
                Destroy(child.gameObject);
                break;
            }
        }

        _netMan.CountStartNotification(10);
    }
}
