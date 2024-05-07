using System;
using UnityEngine;

namespace NuiN.Movement
{
    public class DebugMovementProvider : MonoBehaviour, IMovementProvider
    {
        public bool Sprinting { get; private set; }
        public event Action OnJump;

        Vector3 IMovementProvider.GetDirection()
        {
            return new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        Quaternion IMovementProvider.GetRotation()
        {
            return default;
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space)) OnJump?.Invoke();
            Sprinting = Input.GetKey(KeyCode.LeftShift);
        }

    }
}