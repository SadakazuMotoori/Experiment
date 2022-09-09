using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    PlayerInput _input;
    public PlayerInput Input => _input;

    InputActionMap _actionMap_GamePlay;
    InputActionMap _actionMap_UI;

    void Awake()
    {
        if (Instance != null) return;
        Instance = this;
        _input = GetComponent<PlayerInput>();
        _actionMap_GamePlay = _input.actions.FindActionMap("GamePlay");
        _actionMap_UI = _input.actions.FindActionMap("UI");
    }

    public void ChangeActionMap(string name)
    {
        _input.SwitchCurrentActionMap(name);

        Debug.Log("InputAction変更：" + name);
    }

    //========================================
    // プレイヤー関係
    //========================================
    public Vector2 GamePlay_GetAxisL() => _actionMap_GamePlay["AxisL"].ReadValue<Vector2>();
    public bool GamePlay_GetButtonAttack() => _actionMap_GamePlay["Attack"].WasPressedThisFrame();
    public bool GamePlay_GetButtonMenu() => _actionMap_GamePlay["Menu"].WasPressedThisFrame();

    //========================================
    // UI
    //========================================
    public bool UI_GetButtonUp() => _actionMap_UI["Up"].WasPressedThisFrame();
    public bool UI_GetButtonDown() => _actionMap_UI["Down"].WasPressedThisFrame();
    public bool UI_GetButtonLeft() => _actionMap_UI["Left"].WasPressedThisFrame();
    public bool UI_GetButtonRight() => _actionMap_UI["Right"].WasPressedThisFrame();
    public bool UI_GetButtonDecide() => _actionMap_UI["Decide"].WasPressedThisFrame();
}
