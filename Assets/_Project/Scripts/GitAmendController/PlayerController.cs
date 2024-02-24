using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using Utilities;

namespace Platformer
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")] 
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Child] Animator animator;
        [SerializeField, Anywhere] CinemachineFreeLook freeLookVCam;
        [SerializeField, Anywhere] InputReader input;
        
        [Header("Movement Settings")]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;
        
        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float jumpMaxHeight = 0f;
        [SerializeField] float gravityMultiplier = 3f;

        Transform mainCam;
        
        float currentSpeed;
        float velocity;
        float jumpVelocity;

        Vector3 movement;

        List<Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        
        
        static readonly int SPEED_STRING = Animator.StringToHash("Speed"); 

        void Awake()
        {
            mainCam = Camera.main.transform;
            freeLookVCam.Follow = transform;
            freeLookVCam.LookAt = transform;
            freeLookVCam.OnTargetObjectWarped(transform, transform.position - freeLookVCam.transform.position - Vector3.forward);

            rb.freezeRotation = true;

            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            timers = new List<Timer>(2) { jumpTimer, jumpCooldownTimer };

            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        void Start() => input.EnablePlayerActions();

        void OnEnable()
        {
            input.Jump += OnJump;
        }

        void OnDisable()
        {
            input.Jump -= OnJump;
        }
        
        void OnJump(bool performed)
        {
            if (performed && groundChecker.IsGrounded && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning)
            {
                jumpTimer.Start();
            } 
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }

        void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
            HandleTimers();
            UpdateAnimator();
        }
    
        void FixedUpdate()
        {
            HandleJump();
            HandleMovement();
        }
        
        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        void HandleJump()
        {
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = 0f;
                jumpTimer.Stop();
                return;
            }

            if (jumpTimer.IsRunning)
            {
                // Progress for initial jump burst velocity
                float launchPoint = 0.9f;
                if (jumpTimer.Progress > launchPoint)
                {
                    // Calculate the velocity required to reach the jump height using v = sqrt(2gh)
                    jumpVelocity = Mathf.Sqrt(2f * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                }
                else
                {
                    jumpVelocity += (1f - jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
                }
            }
            else
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity, rb.velocity.z);
        }

        void UpdateAnimator()
        {
            animator.SetFloat(SPEED_STRING, currentSpeed);
        }

        void HandleMovement()
        {
            Vector3 adjustedDirection = Quaternion.AngleAxis(mainCam.eulerAngles.y, Vector3.up) * movement;
            if (adjustedDirection.magnitude > 0f)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            } 
            else
            {
                SmoothSpeed(0f);
                
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            Vector3 velocity = adjustedDirection * (moveSpeed * Time.fixedDeltaTime);
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
        }

        void HandleRotation(Vector3 adjustedDirection)
        {
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}