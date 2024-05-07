using TNRD;
using UnityEngine;

namespace NuiN.Movement
{
    public class HeadRotationController : MonoBehaviour
    {
        [SerializeField] Transform head;
        [SerializeField] SerializableInterface<IMovementProvider> movementProvider; 

        void LateUpdate()
        {
            head.localRotation = movementProvider.Value.GetHeadRotation();
        }
    }
}