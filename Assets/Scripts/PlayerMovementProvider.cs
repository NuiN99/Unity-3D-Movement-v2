using System;
using NuiN.NExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NuiN.Movement
{
    public class PlayerMovementProvider : MonoBehaviour, IMovementProvider
    {
        public event Action OnJump;
        public bool Sprinting => useActions ? sprintAction.IsPressed() : sprintActionReference.action.IsPressed();
        
        const string AXIS_X = "Horizontal";
        const string AXIS_Y = "Vertical";
    
        const string MOUSE_X = "Mouse X";
        const string MOUSE_Y = "Mouse Y";
    
        Vector2 _rotation;

        [SerializeField] bool useActions;
        
        [SerializeField, ShowIf(nameof(useActions), true)] InputAction sprintAction;
        [SerializeField, ShowIf(nameof(useActions), true)] InputAction jumpAction;
        
        [SerializeField, ShowIf(nameof(useActions), false)] InputActionReference sprintActionReference;
        [SerializeField, ShowIf(nameof(useActions), false)] InputActionReference jumpActionReference;
    
        [SerializeField] float lookSensitivity = 20f;
        [Range(0f, 90f)][SerializeField] float yRotationLimit = 88f;

        Quaternion _headRotation;

        void Awake()
        {
            sprintAction?.Enable();
            if (sprintActionReference != null) sprintActionReference.action?.Enable();
            
            jumpAction?.Enable();
            if (jumpActionReference != null) jumpActionReference.action?.Enable();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }

        void OnEnable()
        {
            if (useActions) jumpAction.performed += HandleJump;
            else jumpActionReference.action.performed += HandleJump; 
        }
        void OnDisable()
        {
            if (useActions) jumpAction.performed -= HandleJump;
            else jumpActionReference.action.performed -= HandleJump;
        }

        void HandleJump(InputAction.CallbackContext context)
        {
            OnJump?.Invoke();
        }

        public Vector3 GetDirection()
        {
            float x = Input.GetAxisRaw(AXIS_X);
            float z = Input.GetAxisRaw(AXIS_Y);

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 desiredMoveDirection = forward * z + right * x;

            return desiredMoveDirection.normalized;
        }
    
        public Quaternion GetRotation()
        {
            _rotation.x += Input.GetAxis(MOUSE_X) * lookSensitivity;
            _rotation.y += Input.GetAxis(MOUSE_Y) * lookSensitivity;
            _rotation.y = Mathf.Clamp(_rotation.y, -yRotationLimit, yRotationLimit);

            var xQuat = Quaternion.AngleAxis(_rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(_rotation.y, Vector3.left);
        
            _headRotation = yQuat;

            return xQuat;
        }

        public Quaternion GetHeadRotation()
        {
            return _headRotation;
        }
    }
}