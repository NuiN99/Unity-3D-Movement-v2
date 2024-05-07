using UnityEngine;

namespace NuiN.Movement
{
    public abstract class Movement : MonoBehaviour
    {
        public bool Constrained { get; private set; }

        float _constraintDuration;
        
        public void ApplyConstraint(float duration)
        {
            if (_constraintDuration < duration) _constraintDuration = duration;
        }

        void Update()
        {
            _constraintDuration -= Time.deltaTime;
            Constrained = _constraintDuration > 0;
            
            DeltaTick();
        }

        protected abstract void DeltaTick();
    }
}