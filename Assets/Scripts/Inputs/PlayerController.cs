using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    [System.Serializable]
    public struct ControllerInput
    {
        public float X, Y;
        public int RawX, RawY;
    }
    
    public enum PlayerState { Idle, Run, Walk, Jump1, Jump2, Fall, WallJump, Dash, Climb, LadderClimb, Crouch, Attack1, Attack2, }

    public class PlayerController : MonoBehaviour
    {
        private InputAction jumpAction, dashAction, moveAction, attackAction;
        
        private PlayerState currentState;

        #region Components

        [Header("Components")]
        private PlayerInput playerInput;
        [SerializeField] private PlayerCollision col;
        [SerializeField] private Particles.GhostTrail ghostTrail;
        private Rigidbody2D rb;
        public ControllerInput inputs;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        #endregion
        
        private bool facingLeft, isPushingLeftWall, isPushingRightWall, canMove;

        #region Movement Properties

        [Header("Movement Properties")] 
        [SerializeField] private float movementSpeed = 4;
        [SerializeField] private float acceleration = 2;
        [SerializeField] private float currentMoveLerpSpeed = 100;

        #endregion

        #region Jump Properties

        [Header("Jump Properties")] 
        [SerializeField] private float jumpForce = 15;
        [SerializeField] private float fallMult = 7;
        [SerializeField] private float jumpVelFallOff = 8;
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private bool canDoubleJump = false;
    
        private bool hasJumped, hasDoubleJumped, hasLanded;
        private float timeLastJumped, prevVelocityY;
        private bool isWallJumping = false;

        #endregion

        #region Dash Properties

        [Header("Dash Properties")] 
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 1;
        [SerializeField] private float dashCooldown = 0.2f;
    
        private bool hasDashed, isDashing;
        private Vector2 dashDir;
        private float timeLastDashed, defaultGravityScale;

        #endregion
    
        #region Attack Properties
        
        [Header("Attack Values")]
        //[SerializeField] private int Damage = 0;
        [SerializeField] private LayerMask attackLayers;
    
        private bool isAttacking;
        private bool hasAttacked;
        
        private int attackComboNum = 0;
        private float attackComboInterval = 1f;
        private float timeLastAttack;
        private bool hasSecondAtkBuffer;
        
        #endregion

        #region Particles

        [Header("Particles")]
        [SerializeField] private ParticleSystem groundParticle;
        [SerializeField] private ParticleSystem dashParticle;
        
        #endregion

        #region Unity Functions

        private void Awake()
        {
            // assign components
            playerInput = GetComponent<PlayerInput>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponentInChildren<PlayerCollision>();
            // spriteRenderer = GetComponent<SpriteRenderer>();
            ghostTrail = GetComponentInChildren<Particles.GhostTrail>();
            
            // set default values
            canMove = true;
            defaultGravityScale = rb.gravityScale;
            //ghostTrail.enabled = false;
            
            // assign inputs
            jumpAction = playerInput.actions["Jump"];
            dashAction = playerInput.actions["Dash"];
            moveAction = playerInput.actions["Move"];
            attackAction = playerInput.actions["Attack"];
            AssignInputs();
        }
        

        private void FixedUpdate()
        {
            MoveUpdate();
            JumpUpdate();
            DashUpdate();
        }

        private void LateUpdate() { UpdateValues(); }

        #endregion
        

        void ChangeState(PlayerState state)
        {
            if(currentState == state) return;

            currentState = state;
            
            CustomEvents.Events.OnPlayerChangeState.Invoke(this, currentState);
        }

        private void UpdateValues()
        {
            if (!hasLanded && col.isGrounded)
            {
                hasLanded = true;
                hasDashed = false;
                hasJumped = false;
                currentMoveLerpSpeed = 100;
                prevVelocityY = 0;
                isWallJumping = false;
            }
            if (hasLanded && !col.isGrounded)
            {
                hasLanded = false;
                timeLastJumped = Time.time;
            }

            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -50, 15));
        }
        
        // Set Listeners to inputs
        private void AssignInputs()
        {

        }

        #region Inputs Handler
        
        public void Move(InputAction.CallbackContext context)
        {
            if(isWallJumping) return;
            
            inputs.RawX = (int) context.ReadValue<Vector2>().x;
            inputs.RawY = (int) context.ReadValue<Vector2>().y;
            inputs.X = context.ReadValue<Vector2>().x;
            inputs.Y = context.ReadValue<Vector2>().y;
            
            bool isPrevLeft = facingLeft;
            //facingLeft = _inputs.RawX != 1 && (_inputs.RawX == -1 || facingLeft);
            facingLeft = Mathf.Approximately(inputs.X, 0) ? facingLeft : inputs.X < 0;
        
            // if change of orientation
            if(isPrevLeft != facingLeft && col.isGrounded) groundParticle.Play();
            
            // spriteRenderer.flipX = facingLeft;

            isPushingLeftWall = col.onLeftWall && inputs.X < 0;
            isPushingRightWall = col.onRightWall && inputs.X > 0;
        }
        
        public void Jump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                if (col.isGrounded || Time.time < timeLastJumped + coyoteTime || canDoubleJump && !hasDoubleJumped)
                {
                    if (!hasJumped || hasJumped && !hasDoubleJumped)
                    {
                        prevVelocityY = Mathf.Clamp(rb.velocity.y,0,10);
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce + prevVelocityY);
                        hasDoubleJumped = hasJumped;
                        hasJumped = true;
                        //groundParticle.Play();
                    }
                }
                
                if (hasJumped && !hasLanded && (isPushingLeftWall || isPushingRightWall))
                {
                    rb.velocity =  new Vector2(col.wallSide * - 1f * (jumpForce/2f), jumpForce);
                    hasJumped = true;
                    isWallJumping = true;
                }
            }
            // if(hasLanded && col.isGrounded) return;
            // if(Mathf.Approximately(rb.velocity.y, 0)) return;
        }

        public void Dash(InputAction.CallbackContext context)
        {
            // early returns
            if(!context.started) return;
            if(isAttacking) return;
            if(isDashing) return;
            if(hasDashed) return;
            if(Time.time < timeLastAttack + dashCooldown) return;
            
            // dash
            dashDir = new Vector2(inputs.X, inputs.Y).normalized;
            
            if (dashDir == Vector2.zero) dashDir = facingLeft ? Vector2.left : Vector2.right;

            isDashing = true;
            hasDashed = true;
            
            timeLastDashed = Time.time;
            rb.gravityScale = 0;

            ghostTrail.enabled = true;

            dashParticle.transform.position = facingLeft
                ? (Vector2) transform.position + col.leftOffset
                : (Vector2) transform.position + col.rightOffset;

            Quaternion dashParticleRot = dashParticle.transform.rotation;
            
            dashParticle.transform.rotation = facingLeft
                ? new Quaternion(dashParticleRot.x, dashParticleRot.y, 270, dashParticleRot.w)
                : new Quaternion(dashParticleRot.x, dashParticleRot.y, 90, dashParticleRot.w);

            dashParticle.Play();
        }
        
        #endregion
        
        #region Movement Update

        private void MoveUpdate()
        {
            if(!canMove) return;
            if(isAttacking) return;
            if(isWallJumping) return;
            
            Vector2 velocity = rb.velocity;
            
            float newAcceleration = col.isGrounded ? acceleration : acceleration / 2f;

            switch (inputs.X)
            {
                case < 0:
                {
                    if (velocity.x > 0) inputs.X = 0;
                    inputs.X = Mathf.MoveTowards(inputs.X, -1, newAcceleration * Time.deltaTime);
                    break;
                }
                case > 0:
                {
                    if (velocity.x < 0) inputs.X = 0;
                    inputs.X = Mathf.MoveTowards(inputs.X, 1, newAcceleration * Time.deltaTime);
                    break;
                }
                default:
                    inputs.X = Mathf.MoveTowards(inputs.X, 0, newAcceleration * 2 * Time.deltaTime);
                    break;
            }

            
            Vector3 newVelocity = new Vector3(inputs.X * movementSpeed, velocity.y);
            rb.velocity = Vector2.MoveTowards(velocity, newVelocity, currentMoveLerpSpeed * Time.deltaTime);
        
            if(!hasLanded) return;
            if(isDashing) return;

            // animator.Play(Mathf.Approximately(rb.velocity.x, 0) ? "Idle" : "Run");
        }

        private void JumpUpdate()
        {
            if (jumpAction.IsPressed()) return;
            
            if (rb.velocity.y < jumpVelFallOff || rb.velocity.y > 0)
            {
                rb.velocity += Vector2.up * (fallMult * Physics2D.gravity.y * Time.deltaTime);
            }
        }

        private void DashUpdate()
        {
            if (!isDashing) return;
            rb.velocity = dashDir * dashSpeed;

            // end of dash
            if (!(Time.time >= timeLastDashed + dashDuration)) return;
        
            isDashing = false;
            ghostTrail.enabled = false;
            float yValue = Mathf.Clamp(rb.velocity.y, -10, 10);
            rb.velocity = new Vector2(rb.velocity.x, yValue);
            rb.gravityScale = defaultGravityScale;

            if (hasLanded) hasDashed = false;
        }
        
        #endregion
    }
}

