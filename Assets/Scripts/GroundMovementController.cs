using NuiN.NExtensions;
using TNRD;
using UnityEngine;

namespace NuiN.Movement
{
    public class GroundMovementController : Movement
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] SerializableInterface<IMovementProvider> movementProvider;
        [SerializeField] GroundFloater groundChecker;

        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 0.375f;
        [SerializeField] float runSpeedMult = 1.5f;
        [SerializeField] float maxAirVelocityMagnitude = 6.2f;
        [SerializeField] float groundSpeedMult = 2.85f;
        [SerializeField] float groundDrag = 15f;
        [SerializeField] float airDrag = 0.002f;
        [SerializeField] float airNoInputCounteractMult = 0.01f;
        
        [Header("Rotation Settings")]
        [SerializeField] float walkingRotateSpeed = 99999f;
        [SerializeField] float runningRotateSpeed = 99999f;
        
        [Header("Jump Settings")] 
        [SerializeField] SimpleTimer jumpDelay = new(0.2f);
        [SerializeField] float jumpForce = 6f;
        [SerializeField] int maxAirJumps = 1;

        [Header("Down Force")] 
        [SerializeField] float gravity = 20f;
        [SerializeField] float downForceMult = 0.15f;
        [SerializeField] float downForceStartUpVelocity = 3f;
        
        int _curAirJumps;
        bool _jumping;

        void Reset()
        {
            movementProvider.Value = this.GetInHierarchy<IMovementProvider>();
            groundChecker = this.GetInHierarchy<GroundFloater>();
            rb = this.GetInHierarchy<Rigidbody>();
        }

        void OnValidate()
        {
            movementProvider.Value ??= this.GetInHierarchy<IMovementProvider>();
            groundChecker ??= this.GetInHierarchy<GroundFloater>();
            if (rb == null) rb = this.GetInHierarchy<Rigidbody>();
        }

        void OnEnable()
        {
            movementProvider.Value.OnJump += Jump;
            groundChecker.OnFinishedJump += SetJumpingFalse;
        }
        void OnDisable()
        {
            movementProvider.Value.OnJump -= Jump;
            groundChecker.OnFinishedJump -= SetJumpingFalse;
        }

        void Start()
        {
            rb.useGravity = false;
        }

        void FixedUpdate()
        {
            if (!groundChecker.Grounded)
            {
                rb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
                if (rb.velocity.y <= downForceStartUpVelocity)
                {
                    rb.velocity += Vector3.down * downForceMult;
                }
            }
            
            if (Constrained) return;
            
            Rotate();
            Move();
        }

        void Move()
        {
            Vector3 direction = movementProvider.Value.GetDirection().With(y: 0);

            bool inputtingDirection = direction != Vector3.zero;

            bool sprinting = movementProvider.Value.Sprinting;

            float speed = (sprinting ? moveSpeed * runSpeedMult : moveSpeed);
    
            Vector3 moveVector = direction * speed;
            Vector3 groundVelocity = rb.velocity.With(y: 0);
            Vector3 nextFrameVelocity = groundVelocity + moveVector;

            if (!groundChecker.Grounded || _jumping)
            {
                rb.drag = airDrag;
                float maxAirVel = sprinting ? maxAirVelocityMagnitude * runSpeedMult : maxAirVelocityMagnitude;

                // only allow movement in a direction that doesnt increase forward velocity past the max air vel
                if (nextFrameVelocity.magnitude >= maxAirVel && nextFrameVelocity.magnitude >= groundVelocity.magnitude)
                {
                    moveVector = Vector3.ProjectOnPlane(moveVector, groundVelocity.normalized);
                }

                if (!inputtingDirection)
                {
                    moveVector = -rb.velocity * airNoInputCounteractMult;
                }
            }
            else
            {
                moveVector *= groundSpeedMult;
                rb.drag = groundDrag;
                _curAirJumps = 0;
            }
            
            rb.velocity += moveVector.With(y: 0);
        }

        void Rotate()
        {
            Quaternion rotation = movementProvider.Value.GetRotation();
            float rotateSpeed = movementProvider.Value.Sprinting ? runningRotateSpeed : walkingRotateSpeed;

            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
        }

        void Jump()
        {
            if (!jumpDelay.Complete()) return;

            // set jumping true to immediately switch to air drag in movement logic
            _jumping = true;
            
            groundChecker.Jump();

            Vector3 vel = rb.velocity;
            
            if (groundChecker.Grounded)
            {
                _curAirJumps = 0;
                rb.velocity = vel.With(y: jumpForce);
                return;
            }

            if (_curAirJumps >= maxAirJumps) return;
            _curAirJumps++;

            // only sets y velocity when y velocity is less than potential jump force. Otherwise it would set y vel to a lower value when going faster
            if (vel.y <= jumpForce)
            {
                rb.velocity = vel.With(y: jumpForce);
            }
            else
            {
                rb.velocity += Vector3.up * jumpForce;
            }
        }

        void SetJumpingFalse()
        {
            _jumping = false;
        }
    }
}