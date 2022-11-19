using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        [SerializeField] private Slider _punchSlider;
        private float _punchSliderValue;
        private bool _isReadyToBreak = false;
        private bool _chargingPunch = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();
        
        // 1. create a reference to our input actions
        private PlayerInputActions _inputActions;
        
        int _piecesCount;
        private void Awake()
        {
            _punchSlider.gameObject.SetActive(false);
            // 2. create a new instance of our input actions
            _inputActions = new PlayerInputActions();
            _inputActions.Enable();
            // 3. subscribe to the action we want to use
            _inputActions.Player.Actions.started += Charging_started;
            _inputActions.Player.Actions.canceled += Charging_canceled; 
            _inputActions.Player.Actions.performed += Charging_completed;
        }
        
        private void Charging_canceled(InputAction.CallbackContext obj)
        {
            _chargingPunch = false;
            _punchSlider.value = 0;
        }
        
        private void Charging_completed(InputAction.CallbackContext obj)
        {
            Debug.Log("CHARGED");
            _punchSlider.gameObject.SetActive(false);
            _chargingPunch = false;
            _isReadyToBreak = true;
            _piecesCount = _brakeOff.Count;
            _wholeCrate.SetActive(false);
            _brokenCrate.SetActive(true);

            for (int i = 0; i < _piecesCount; i++)
            {
                Debug.Log("breakoff = " + _brakeOff[i]);
                _brakeOff[i].isKinematic = false;
                _brakeOff[i].constraints = RigidbodyConstraints.None;
                _brakeOff[i].AddExplosionForce(6, transform.position, 10,1,ForceMode.Impulse);
            }
        }
        private void Charging_started(InputAction.CallbackContext obj)
        {
            // if (_isReadyToBreak && zone.GetZoneID() == 6)
            // {
                _chargingPunch = true;
                //turn on slider
                //_punchSlider.gameObject.SetActive(true);
                StartCoroutine(ChargePunch());
            //}

            
        }

        public void TurnOnSlider()
        {
            _punchSlider.gameObject.SetActive(true);
        }

        IEnumerator ChargePunch()
        {
            while (_chargingPunch == true & _punchSlider.value <= 1)
            {
                _punchSlider.value += Time.deltaTime / 3f;
                yield return null;
            }
        }
        
        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            
            if (_isReadyToBreak == false && _brakeOff.Count >0)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }
            if (_isReadyToBreak && zone.GetZoneID() == 6)
            //if (_isReadyToBreak) //Crate zone            
            {
                if (_brakeOff.Count > 0)
                {
                    BreakPart();
                    StartCoroutine(PunchDelay());
                }
                else if(_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                    Debug.Log("Completely Busted");
                }
            }
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);

        }



        public void BreakPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);            
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
        }
    }
}
