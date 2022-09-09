using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Player _player;
    
    private PlayerInputActions _inputActions;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeInputs();
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
    
}
