using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

using KdGame.Net;
using Photon.Pun;
public class NetworkInputProvider : InputProvider
{
    Vector2 m_Key       = Vector2.zero;
    Vector2 m_PrevKey   = Vector2.zero;

    int     m_PlayerID  = -1;

    public override void SetPlayerID(int ID) { m_PlayerID = ID; }

    // ç∂é≤
    public override Vector2 GetAxisL()
    {
        //        return PlayerInputManager.Instance.Input.currentActionMap["AxisL"].ReadValue<Vector2>();

        Vector2 axis = (m_PlayerID == NetworkManager.Instance.GetMyID()) ? m_Key : Vector2.zero;
        Quaternion r = Quaternion.Euler(0, 0, -Camera.main.transform.eulerAngles.y);
        return r * axis;
    }

    // ç∂é≤
    public override void SetAxisL(Vector2 key)
    {
        m_Key = key;
    }

    // âEé≤
    public override Vector2 GetAxisR() => Vector2.zero;

    public override bool GetButtonAttack()
    {
        return PlayerInputManager.Instance.GamePlay_GetButtonAttack();
    }

    private void Update()
    {
        if (PlayerInputManager.Instance.GamePlay_GetButtonMenu())
        {
            OpenMenuWindow().Forget();
        }

        if (m_PlayerID == NetworkManager.Instance.GetMyID())
        {
            if (m_PrevKey != PlayerInputManager.Instance.GamePlay_GetAxisL())
            {
                m_Key       = PlayerInputManager.Instance.GamePlay_GetAxisL();
                m_PrevKey   = m_Key;

                NetworkManager.stSyncKey syncKeyInfo = new NetworkManager.stSyncKey();
                syncKeyInfo.playerid    = NetworkManager.Instance.GetMyID();
                syncKeyInfo.key         = m_PrevKey;
                NetworkManager.Instance.CreateSendData(NetworkManager.ENETWORK_COMMAND.CMD_SYNCKEY, RpcTarget.All, syncKeyInfo);
            }
        }
    }

    async UniTask OpenMenuWindow()
    {
        var wnd = await WindowSystem.WindowManager.Instance.CreateWindow<WindowSystem.Window_GameMenu>("Window_GameMenu",
            async wnd =>
            {
                await wnd.Initialize();
            });
        await wnd.Execute();
    }
}
