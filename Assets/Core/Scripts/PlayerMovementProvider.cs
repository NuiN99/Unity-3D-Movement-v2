using System;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NuiN.Movement
{
    public class PlayerMovementProvider : MonoBehaviour, IMovementProvider
    {
        public event Action OnJump;
        public bool Sprinting => toggleSprint ? _sprintToggled : sprintAction.action.IsPressed();
        
        Vector2 _rotation;

        [SerializeField] InputActionProperty moveAxisAction;
        [SerializeField] InputActionProperty rotateAxisAction;
        [SerializeField] InputActionProperty jumpAction;
        [SerializeField] InputActionProperty sprintAction;

        [SerializeField] bool toggleSprint;
        [SerializeField] float lookSensitivity = 20f;
        [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

        Quaternion _headRotation;
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

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

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
            _rotation.y = Mathf.Clamp(_rotation.y, -yRotationLimit, yRotationLimit);

            var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);
        
            _headRotation = yQuat;

            return xQuat;
        }

        public Quaternion GetHeadRotation() => _headRotation;
    }
}