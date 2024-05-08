using System;
using NuiN.NExtensions;
using UnityEngine;

namespace NuiN.Movement
{
    public class GroundFloater : MonoBehaviour
    {
        public bool Grounded { get; private set; }
        public bool InCoyoteTime { get; private set; }
        
        public event Action OnFinishedJump;
        
        [Header("Dependencies")]
        [SerializeField] Rigidbody rb;
        [SerializeField] CollisionProxy collisionProxy;

        [Header("Settings")]
        [SerializeField] protected float floatHeight = 1f;
        [SerializeField] float floatSpring = 2f;
        [SerializeField] float floatDamper = 0.1f;
        [SerializeField] SimpleTimer coyoteTime = new(0.15f, true);
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
        bool _usedCoyoteTimeThisJump;

        void Reset()
        {
            rb = this.GetInHierarchy<Rigidbody>();
            collisionProxy = this.GetInHierarchy<CollisionProxy>();
        }

        void OnValidate()
        {
            rb = rb == null ? this.GetInHierarchy<Rigidbody>() : rb;
            collisionProxy = collisionProxy == null ? this.GetInHierarchy<CollisionProxy>() : collisionProxy;
        }

        void OnEnable() => collisionProxy.CollisionStay += Colliding;
        void OnDisable() => collisionProxy.CollisionStay -= Colliding;

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

            if (!_usedCoyoteTimeThisJump && !_wasNotGrounded && !Grounded && !InCoyoteTime)
            {
                InCoyoteTime = true;
                _usedCoyoteTimeThisJump = false;
            }
            
            if (InCoyoteTime)
            {
                bool coyoteTimeComplete = coyoteTime.Complete();
                if (Grounded || coyoteTimeComplete)
                {
                    InCoyoteTime = false;
                    _usedCoyoteTimeThisJump = false;
                }
            }
            
            if (_wasNotGrounded && Grounded)
            {
                _wasNotGrounded = false;
                OnFinishedJump?.Invoke();
            }

            if (!Grounded || _jumping)
            {
                Grounded = false;
                return;
            }

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
            if (InCoyoteTime) _usedCoyoteTimeThisJump = true;
            else _usedCoyoteTimeThisJump = false;
            
            _jumping = true;
            _wasNotGrounded = true;
        }

        void Colliding(Collision other)
        {
            _usedCoyoteTimeThisJump = false;
            _jumping = false;
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