using System;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NuiN.Movement
{
    public class XRPlayerMovementProvider : MonoBehaviour, IMovementProvider
    {
        public event Action OnJump;
        public bool Sprinting => toggleSprint ? _sprintToggled : sprintAction.action.IsPressed();
        public float LookSensitivity => lookSensitivity;
        public float RotationAxis => rotateAxisAction.action.ReadValue<Vector2>().x;

        [SerializeField] bool controllerRelative;
        [SerializeField, ShowIf(nameof(controllerRelative), false)] Transform playerHead;
        [SerializeField, ShowIf(nameof(controllerRelative), true)] Transform playerHand;
        
        [SerializeField] InputActionProperty moveAxisAction;
        [SerializeField] InputActionProperty rotateAxisAction;
        [SerializeField] InputActionProperty jumpAction;
        [SerializeField] InputActionProperty sprintAction;

        [SerializeField] bool toggleSprint;
        [SerializeField] float lookSensitivity = 20f;

        Vector2 _rotation;
        bool _sprintToggled;

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            
            moveAxisAction.Enable();
            rotateAxisAction.Enable();
            jumpAction.Enable();
            sprintAction.Enable();
        }

        void OnEnable()
        {
            jumpAction.AddHandler(HandleJump);
            sprintAction.AddHandler(HandleSprint);
        }

        void OnDisable()
        {
            jumpAction.RemoveHandler(HandleJump);
            sprintAction.RemoveHandler(HandleSprint);
        }

        void HandleJump(InputAction.CallbackContext context) => OnJump?.Invoke();
        void HandleSprint(InputAction.CallbackContext context) => _sprintToggled = !_sprintToggled;

        public Vector3 GetDirection()
        {
            Vector2 axis = moveAxisAction.action.ReadValue<Vector2>();

            Transform relativeTransform = controllerRelative ? playerHand : playerHead;
            Vector3 forward = relativeTransform.forward;
            Vector3 right = relativeTransform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * axis.y + right * axis.x;

            return desiredMoveDirection.normalized;
        }
    
        public Quaternion GetRotation()
        {
            Vector2 axis = rotateAxisAction.action.ReadValue<Vector2>();
            _rotation.x += axis.x * lookSensitivity;
            _rotation.y += axis.y * lookSensitivity;

            return Quaternion.AngleAxis(_rotation.x, Vector3.up);
        }
    }
}