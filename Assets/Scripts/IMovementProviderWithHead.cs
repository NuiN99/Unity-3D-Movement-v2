using UnityEngine;

namespace NuiN.Movement
{
    public interface IMovementProviderWithHead : IMovementProvider
    {
        public Quaternion GetHeadRotation();
    }
}