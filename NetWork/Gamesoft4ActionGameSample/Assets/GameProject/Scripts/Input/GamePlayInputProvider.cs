using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;

public class GamePlayInputProvider : InputProvider
{
    // ç∂é≤
    public override Vector2 GetAxisL()
    {
//        return PlayerInputManager.Instance.Input.currentActionMap["AxisL"].ReadValue<Vector2>();

        Vector2 axis = PlayerInputManager.Instance.GamePlay_GetAxisL();
        Quaternion r = Quaternion.Euler(0, 0, -Camera.main.transform.eulerAngles.y);
        return r * axis;
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
