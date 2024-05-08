using System;
using UnityEngine;

namespace NuiN.Movement
{
    public interface IMovementProvider
    {
        bool Sprinting { get; }
        event Action OnJump;
        
        Vector3 GetDirection();
        Quaternion GetRotation();
        Quaternion GetHeadRotation() => default;
    }
}