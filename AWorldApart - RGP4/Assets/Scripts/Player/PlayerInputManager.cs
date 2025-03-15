using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputManager : MonoBehaviour
{
    private PlayerInputs playerInputs;

    [SerializeField] private Vector2 movementAction;
    [SerializeField] public bool escape;
    [SerializeField] public bool jumpAction;
    [SerializeField] public bool mouseLeftClick;
    [SerializeField] public bool mouseRightClick;
    [SerializeField] public bool reload;
    private void Start()
    {
        playerInputs = new PlayerInputs();
        playerInputs.Enable();

        playerInputs.Player.Move.started += OnMove;
        playerInputs.Player.Move.canceled += OnMove;
        playerInputs.Player.Move.performed += OnMove;

        playerInputs.Player.Escape.started += OnEscape;
        playerInputs.Player.Escape.canceled += OnEscape;

        playerInputs.Player.Reload.started += OnReload;
        playerInputs.Player.Reload.canceled += OnReload;

        playerInputs.Player.Jump.started += OnJump;
        playerInputs.Player.Jump.canceled += OnJump;

        playerInputs.UI.Click.performed += OnClick;

        playerInputs.UI.RightClick.performed += OnRightClick;

    }
    private void OnClick(InputAction.CallbackContext callbackContext)
    {
        mouseLeftClick = callbackContext.ReadValueAsButton();
    }
    private void OnRightClick(InputAction.CallbackContext callbackContext)
    {
        mouseRightClick = callbackContext.ReadValueAsButton();
    }

    private void OnReload(InputAction.CallbackContext callbackContext)
    {
        reload = callbackContext.ReadValueAsButton();
    }
    private void OnJump(InputAction.CallbackContext callbackContext)
    {
        jumpAction = callbackContext.ReadValueAsButton();
    }
    private void OnEscape(InputAction.CallbackContext callbackContext)
    {
        escape = callbackContext.ReadValueAsButton();
    }

    public float GetHorizontal()
    {
        return movementAction.x;
    }

    public float GetVertical()
    {
        return movementAction.y;
    }

    private void OnMove(InputAction.CallbackContext callbackContext)
    {
        movementAction = callbackContext.ReadValue<Vector2>();
    }




}