using UnityEngine;

namespace NuiN.Movement
{
    public class XRGroundMovementController : GroundMovementController
    {
        [SerializeField] Transform head;
        [SerializeField] XRPlayerMovementProvider playerMovementProvider; 
        
        protected override void Rotate()
        {
            transform.RotateAround(new Vector3(head.position.x, 0, head.position.z), Vector3.up,  playerMovementProvider.LookSensitivity * playerMovementProvider.RotationAxis);
        }
    }
}