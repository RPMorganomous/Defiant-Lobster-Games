using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        //1. get a reference and start an instance of our Drone input actions
        private PlayerInputActions _inputActions;

        private void Start()
        {
            //2. enable input action map (Drone)
            InitializeInputs();
        }

        private void InitializeInputs()
        {
            _inputActions = new PlayerInputActions();
        }

        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;
        

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
                // 3. get the input action map (Drone)
                _inputActions.Drone.Enable();
                PlayerManager.OnDisable();
            }
        }

        private void ExitFlightMode()
        {            
            _droneCam.Priority = 9;
            _inFlightMode = false;
            UIManager.Instance.DroneView(false);
            // 3. get the input action map (Drone)
            _inputActions.Drone.Disable();
            PlayerManager.OnEnable();
            
        }

        private void Update()
        {
            if (_inFlightMode)
            {
                var tilt = _inputActions.Drone.DroneTilt.ReadValue<Vector2>();
                CalculateTilt(tilt);
                var rotate = _inputActions.Drone.DroneRotate.ReadValue<float>();
                CalculateMovementUpdate(rotate);

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    ExitFlightMode(); 
                    //_inputActions.Drone.Disable();
                }
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            var thrust = _inputActions.Drone.DroneThrust.ReadValue<float>();
            if (_inFlightMode)
                CalculateMovementFixedUpdate(thrust);
        }

        private void CalculateMovementUpdate(float rotate)
        {
            if (rotate < 0) // left arrow key pressed
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            if (rotate > 0 ) // right arrow key pressed
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
        }

        private void CalculateMovementFixedUpdate(float thrust)
        {
            
            if (thrust > 0) // space pressed
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            if (thrust < 0) // V pressed
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        public void CalculateTilt( Vector2 tilt)
        {
            // debug log vaues from inputactions
            Debug.Log("Tilt X: " + tilt.x);
            Debug.Log("Tilt Y: " + tilt.y);
            
            // check vector value for range and apply tilt
            if (tilt.x < 0) // A key pressed
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            else if (tilt.x > 0) // D key pressed
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            else if (tilt.y > 0) // W key pressed
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            else if (tilt.y < 0) // S key pressed
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            else 
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
