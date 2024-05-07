using System;
using NuiN.NExtensions;
using UnityEngine;

namespace NuiN.Movement
{
    public class GroundFloater : MonoBehaviour
    {
        public bool Grounded { get; private set; }
        public event Action OnFinishedJump;
        
        [Header("Dependencies")]
        [SerializeField] Rigidbody rb;

        [Header("Settings")]
        [SerializeField] float floatHeight = 0.75f;
        [SerializeField] float floatSpring = 2f;
        [SerializeField] float floatDamper = 1f;
        [SerializeField] bool affectOthers;
        [SerializeField] LayerMask groundMask = ~0;

        [Header("Debugging")] 
        [SerializeField] bool drawGizmos = true;
        [SerializeField, ShowIf(nameof(drawGizmos), true)] Color gizmosGroundColor = Color.green;
        [SerializeField, ShowIf(nameof(drawGizmos), true)] Color gizmosAirColor = Color.blue;

        Vector3 RayStart => transform.position - new Vector3(0, Height, 0);
        Vector3 RayDir => Vector3.down;
        float Height => transform.localScale.y;

        bool _jumping;
        bool _wasNotGrounded;

        void Reset() => rb = this.GetInHierarchy<Rigidbody>();
        void OnValidate() => rb = rb == null ? this.GetInHierarchy<Rigidbody>() : rb;

        void FixedUpdate()
        {
            if (rb.velocity.y < 0 && _jumping)
            {
                 _wasNotGrounded = true;
                _jumping = false;
            }

            Vector3 rayStart = RayStart;
            Vector3 rayDir = RayDir;

            Grounded = IsGrounded(rayStart, rayDir, out RaycastHit hit);
            if ( _wasNotGrounded && Grounded)
            {
                _wasNotGrounded = false;
                OnFinishedJump?.Invoke();
            }
            
            if (!Grounded || _jumping) return;

            Vector3 vel = rb.velocity;

            bool hitOtherRB = hit.collider.TryGetComponent(out Rigidbody otherRB);
            Vector3 otherVel = hitOtherRB ? otherRB.velocity : default;

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;
            float hitDist = Vector3.Distance(rayStart, hit.point);

            float x = hitDist - floatHeight;
            float springForce = (x * floatSpring) - (relVel * floatDamper);
            
            rb.AddForce(rayDir * springForce, ForceMode.VelocityChange);

            if (affectOthers && hitOtherRB)
            {
                otherRB.AddForceAtPosition(rayDir * -springForce, hit.point);
            }
        }

        bool IsGrounded(Vector3 rayStart, Vector3 rayDir, out RaycastHit hit)
        {
            return Physics.Raycast(rayStart, rayDir, out hit, floatHeight, groundMask);
        }

        public void Jump()
        {
            _jumping = true;
        }

        void OnDrawGizmos()
        {
            if (!drawGizmos || _jumping) return;

            Vector3 maxRayEnd = RayStart + RayDir * floatHeight;
            
            Gizmos.color = Grounded ? gizmosGroundColor : gizmosAirColor;
            Vector3 rayEnd = IsGrounded(RayStart, RayDir, out RaycastHit hit) ? hit.point : maxRayEnd;
            Gizmos.DrawLine(RayStart, rayEnd);

            if (Grounded)
            {
                Gizmos.color = gizmosAirColor;
                Gizmos.DrawLine(hit.point, maxRayEnd);
            }
            
            Gizmos.color = Color.white;
        }
    }
}