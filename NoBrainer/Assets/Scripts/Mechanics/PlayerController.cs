using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer.Gameplay;
using static Platformer.Core.Simulation;
using Platformer.Model;
using Platformer.Core;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This is the main class used to implement control of the player.
    /// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        public AudioClip jumpAudio;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        public float maxSpeed = 5;
        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        public float jumpTakeOffSpeed = 6;

        public float maxFallSpeed = 10;

        public JumpState jumpState = JumpState.Grounded;
        private bool stopJump;
        private int jumpCount = 0;
        public int nbJumps = 2;
        /*internal new*/
        public Collider2D collider2d;
        /*internal new*/
        public AudioSource audioSource;
        public Health health;
        public bool controlEnabled = true;

        bool jump;
        Vector2 move;
        SpriteRenderer spriteRenderer;
        internal Animator animator;
        readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();
        public float jumpHangTimeThreshold = 0.3f;
        public float jumpHangGravityMult = 0.5f;
        public float jumpHangMaxSpeedMult = 1.3f;
        public float coyoteTime = 0.15f;
        private float lastOnGroundTime = 0;
        private float wallJumpCooldown = 0;
        private float lastWalljumpTime = 0;
        public float wallJumpTime = 0.15f;
        public float wallJumpNonControlTime = 0.4f;
        private bool canWallJump = false;

        public Bounds Bounds => collider2d.bounds;

        void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        protected override void Update()
        {
            if (controlEnabled)
            {
                if ((Time.time - lastWalljumpTime) > wallJumpNonControlTime || canWallJump)
                {
                    move.x = Input.GetAxis("Horizontal");
                }
                lastOnGroundTime -= Time.deltaTime;
                wallJumpCooldown -= Time.deltaTime;
                if (((lastOnGroundTime > 0 && jumpState != JumpState.Jumping && jumpState != JumpState.InFlight) || jumpCount > 0 || (wallJumpCooldown > 0 && canWallJump && lastOnGroundTime < 0)) && Input.GetButtonDown("Jump"))
                {
                    jumpState = JumpState.PrepareToJump;
                }
                else if (Input.GetButtonUp("Jump"))
                {
                    stopJump = true;
                    Schedule<PlayerStopJump>().player = this;
                }
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    break;
            }
        }

        protected override void ComputeVelocity()
        {
            if (jump && (lastOnGroundTime > 0 || jumpCount > 0 || (wallJumpCooldown > 0 && canWallJump && lastOnGroundTime < 0)))
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
                if ((wallJumpCooldown > 0 && canWallJump && lastOnGroundTime < 0))
                {
                    canWallJump = false;
                    lastWalljumpTime = Time.time;
                    move.x *= -1;
                }
                else
                {
                    jumpCount -= 1;
                }
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed,
        }

        protected override void FixedUpdate()
        {
            //if already falling, fall faster than the jump speed, otherwise use normal gravity.

            if (jumpState == JumpState.InFlight && Mathf.Abs(velocity.y) < jumpHangTimeThreshold)
            {
                velocity += jumpHangGravityMult * gravityModifier * Physics2D.gravity * Time.deltaTime;
                targetVelocity *= jumpHangMaxSpeedMult;
            }
            else if (velocity.y < 0)
            {
                velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            }
            else
                velocity += Physics2D.gravity * Time.deltaTime;

            velocity.y = Mathf.Max(-maxFallSpeed, velocity.y);
            velocity.x = targetVelocity.x;


            IsGrounded = false;

            var deltaPosition = velocity * Time.deltaTime;

            var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);

            var move = moveAlongGround * deltaPosition.x;

            PerformMovement(move, false);

            move = Vector2.up * deltaPosition.y;

            PerformMovement(move, true);

        }

        void PerformMovement(Vector2 move, bool yMovement)
        {
            var distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                //check if we hit anything in current direction of travel
                var count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                for (var i = 0; i < count; i++)
                {
                    var currentNormal = hitBuffer[i].normal;
                    //is this surface flat enough to land on?
                    if (currentNormal.y > minGroundNormalY)
                    {
                        jumpCount = nbJumps;
                        lastOnGroundTime = coyoteTime;
                        canWallJump = true;
                        IsGrounded = true;
                        // if moving up, change the groundNormal to new surface normal.
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    else
                    {
                        wallJumpCooldown = wallJumpTime;
                    }
                    if (IsGrounded)
                    {
                        //how much of our velocity aligns with surface normal?
                        var projection = Vector2.Dot(velocity, currentNormal);
                        if (projection < 0)
                        {
                            //slower velocity if moving against the normal (up a hill).
                            velocity = velocity - projection * currentNormal;
                        }
                    }
                    else
                    {
                        //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                        velocity.x *= 0;
                        // velocity.y = Mathf.Min(velocity.y, 0);
                    }
                    //remove shellDistance from actual move distance.
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            body.position = body.position + move.normalized * distance;
        }
    }
}