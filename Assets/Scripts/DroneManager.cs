using System.Collections;
using System.Collections.Generic;
using Game.Scripts.LiveObjects;
using UnityEngine;

public class DroneManager : MonoBehaviour
{
[SerializeField] private Drone _drone;

//1. get a reference and start an instance of our drone imput actions
private DroneInputActions _inputActions;

    // Start is called before the first frame update
    void Start()
    {
        // 2. enable input action map (Drone)
        InitializeInputs();
        
        // 3. register perform functions
        _inputActions.Drone.Movement.performed += Movement_performed;
        
    }

    // Update is called once per frame
    void Update()
    {
        var tilt = _inputActions.Drone.Movement.ReadValue<Vector2>();
        _drone.CalculateTilt(tilt);
    }
    
    private void InitializeInputs()
    {
        _inputActions = new DroneInputActions();
        _inputActions.Drone.Enable();
        _inputActions.Drone.Disable();
    }
    private void Movement_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
// debug log to see values coming back

// set bools for tilt & lift
// no need for drone manager
// use single input manager for all game objects


    }
}
