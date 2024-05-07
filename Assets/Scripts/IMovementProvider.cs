using System;
using UnityEngine;

namespace NuiN.Movement
{
    public interface IMovementProvider
    {
        public bool Sprinting { get; }
        
        Vector3 GetDirection();
        Quaternion GetRotation();

        event Action OnJump;
    }
}