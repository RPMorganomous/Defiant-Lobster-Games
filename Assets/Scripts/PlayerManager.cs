using System.Collections;
using System.Collections.Generic;
using Game.Scripts.LiveObjects;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    
    // 1. get a reference and start an instance of our input actions
    private static PlayerInputActions _inputActions;
    
    // Start is called before the first frame update
    void Start()
    {
        // 2. enable input action map (Player)
        InitializeInputs();
        
        // 3. register perform functions
        _inputActions.Player.Actions.performed += Action_performed;
        _inputActions.Player.Actions.canceled += Action_canceled;
        _inputActions.Player.Actions.started += Action_started;
    }

    private void Action_started(InputAction.CallbackContext obj)
    {
        InteractableZone._actionButtonPressed = true;
    }

    private void Action_canceled(InputAction.CallbackContext obj)
    {
        InteractableZone._actionButtonPressed = false;
        InteractableZone._inHoldState = false;
    }

    private void Action_performed(InputAction.CallbackContext obj)
    {
        InteractableZone._inHoldState = true;
    }

    // Update is called once per frame
    void Update()
    {
        var move = _inputActions.Player.Movement.ReadValue<Vector2>();
        _player.CalcutateMovement(move);
    }

    private void InitializeInputs()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Player.Enable();
    }
    
    public static void OnDisable()
    {
        _inputActions.Player.Disable();
    }
    
    public static void OnEnable()
    {
        _inputActions.Player.Enable();
    }
    
}
