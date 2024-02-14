using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform neck;
    [SerializeField] Transform head;
    [SerializeField] Transform eyes;
    [SerializeField] Animator cameraAnimator;
    private CharacterController controller;
    private PlayerInput playerInput;

    [Header("Input Varibles")]
    private Vector2 currentMovementInput;
    private Vector3 playerDirection;
    private Vector3 previousPosition;
    private float lastVerticalVelocity;
    private bool freeLookButtonPressed;
    
    [Header("Speed Settings")]
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float sprintSpeed = 12f;
    [SerializeField] float wallRunSpeed = 12f;
    [SerializeField] private float slideSpeed = 16f;
    [SerializeField] float lerpTime = 6f;
    [SerializeField] float airLerpTime = 1f;
    private float currentSpeed;
    float threshold = 0.01f;

    [Header("Gravity Settings")]
    [SerializeField] float gravity = -9.81f;

    [Header("Jump Settings")]
    [SerializeField] float jumpForce = 5f;
    private float verticalVelocity;
    private Vector3 verticalMove;
    

    [Header("Crouch Settings")]
    private bool canStand;
    private bool crouchPressed;
    private Vector3 crouchStandCheckPosition;

    [Header("Sliding Settings")]
    [SerializeField] private float slideTimerMax = 1.5f;
    private bool sprintButtonHeld = false;
    private float slideTimer = 0.0f;
    private Vector3 slideDirection;

    [Header("Wall Running Settings")]
    [SerializeField] LayerMask wallMask;
    [ SerializeField] private float wallRayDistance = 1f;
    [SerializeField] private float wallRunGravity = -1f;
    [SerializeField] private float wallRunTimerMax = 2f;
    [SerializeField] private float wallJumpForce = 5f;
    private float wallRunTimer = 0.0f;
    private Vector3 wallRunDirection;
    private float horizontalVelocity;
    [HideInInspector] public bool wallRunRight;
    [HideInInspector] public bool wallRunLeft;

    
    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 1.1f;
    private bool isGrounded
    {
        get
        {
            // Cast a ray downwards from the player's position
            Ray ray = new Ray(transform.position, -transform.up);
            // Check if the ray hits the ground within the specified distance
            return Physics.Raycast(ray, groundDistance, groundMask);
        }
    }

    [Header("Player Headbob Settings")]
    [SerializeField] bool enableHeadBob = true;
    private float headBobSprintSpeed = 22f;
    private float headBobWalkSpeed = 14f;
    private float headBobCrouchSpeed = 10f;
    private float headBobSprintIntensity = 0.2f;
    private float headBobWalkIntensity = 0.1f;
    private float headBobCrouchIntensity = 0.05f;
    private Vector2 headBobVector = Vector2.zero;
    private float headBobIndex = 0.0f;
    private float headBobCurrentIntensity = 0.0f;

    [Header("States")]
    private bool isMoving;
    private bool isWalking;
    private bool isSprinting;
    private bool isCrouching;
    private bool isAnimating;
    [HideInInspector] public bool isFreelooking;
    [HideInInspector] public bool isSliding;
    [HideInInspector] public bool isWallRunning;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        previousPosition = transform.position;
    }

    void Start()
    {
        isWalking = true;
        isSprinting = false;
        isCrouching = false;
    }

    void Update()
    {
        if (currentMovementInput == Vector2.zero)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        applyGravity();
        SpeedHandler();
        SlideHandler();
        CheckWall();
        WallRunHandler();
        JumpHandler();
        LandingHandler();
        CrouchHandler();
        MovementHandler();

        // Set slideDirection when a slide is initiated
        if (isSliding && slideTimer == slideTimerMax)
        {
            slideDirection = playerDirection;
        }

        Debug.Log(isAnimating);
    }

    void LateUpdate()
    {
        if (enableHeadBob)
        {
            HeadBobbingHandler();
        }  

        // Calculate vertical velocity
        Vector3 newPosition = transform.position;
        float verticalVelocity = (newPosition.y - previousPosition.y) / Time.deltaTime;
        lastVerticalVelocity = verticalVelocity;

        // Store the current position for the next frame
        previousPosition = newPosition;
    }

    void applyGravity()
    {
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else if (isWallRunning)
        {
            verticalVelocity = Mathf.Lerp(verticalVelocity, wallRunGravity, Time.deltaTime * lerpTime);
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            horizontalVelocity = Mathf.Lerp(horizontalVelocity, 0, Time.deltaTime * lerpTime);
        }
    }

    void SpeedHandler()
    {
        if (isCrouching && isGrounded && !isSliding)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, crouchSpeed, Time.deltaTime * lerpTime);
        }
        else if (isSliding)
        {
            currentSpeed = Mathf.Min((Mathf.Pow(slideTimer, 2) + 0.2f) * slideSpeed, 16);
        }
        else if (isWallRunning)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, wallRunSpeed, Time.deltaTime * lerpTime);
        }
        else if (isWalking && isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * lerpTime);
        }
        else if (isSprinting && isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, Time.deltaTime * lerpTime);
        }
    }

    void HeadBobbingHandler()
    {
        if (isCrouching)
        {
            headBobCurrentIntensity = headBobCrouchIntensity;
            headBobIndex += headBobCrouchSpeed * Time.deltaTime;
        }
         else if (isWallRunning)
        {
            headBobCurrentIntensity = headBobSprintIntensity;
            headBobIndex += headBobSprintSpeed * Time.deltaTime;
        }
        else if (isWalking)
        {
            headBobCurrentIntensity = headBobWalkIntensity;
            headBobIndex += headBobWalkSpeed * Time.deltaTime;
        }
        else  if (isSprinting)
        {
            headBobCurrentIntensity = headBobSprintIntensity;
            headBobIndex += headBobSprintSpeed * Time.deltaTime;
        }

        if (isGrounded && isMoving || isWallRunning)
        {
            headBobVector.y = Mathf.Sin(headBobIndex);
            headBobVector.x = Mathf.Sin(headBobIndex / 2);

            eyes.localPosition = Vector3.Lerp(eyes.localPosition, head.localPosition + new Vector3(headBobVector.x * headBobCurrentIntensity, headBobVector.y * headBobCurrentIntensity, 0), Time.deltaTime * lerpTime);
        }
        else
        {
            eyes.localPosition = Vector3.Lerp(eyes.localPosition, head.localPosition + Vector3.zero, Time.deltaTime * lerpTime);

            if (Vector3.Distance(eyes.localPosition, head.localPosition) <= 0.001f)
            {
                eyes.localPosition = head.localPosition + Vector3.zero;
            }
        }
    }

    void CrouchHandler()
    {
        // Calculate the position to check if player can stand up
        crouchStandCheckPosition = controller.bounds.center + Vector3.up * (controller.height / 2 + controller.radius);
        // Check if player can stand up
        canStand = !Physics.CheckSphere(crouchStandCheckPosition, controller.radius);
        
        if (crouchPressed || isSliding)
        {
            // If player is crouching, reduce player's height
            controller.height = Mathf.Lerp(controller.height, 1f, Time.deltaTime * lerpTime);

            if (isSprinting && isMoving)
            {
                isSliding = true;
                slideTimer = slideTimerMax;
                isFreelooking = true;
                
            }
        
            isWalking = false;
            isSprinting = false;
            isCrouching = true;
        }

        else if (canStand)
        {
            float newHeight = Mathf.Lerp(controller.height, 2f, Time.deltaTime * lerpTime);
            float heightDifference = newHeight - controller.height;
            controller.height = newHeight;
            if (heightDifference > 0)
            {
                controller.Move(new Vector3(0, heightDifference / 2, 0));
            }
            
            if (sprintButtonHeld)
            {
                isSprinting = true;
                isWalking = false;
            }
            else if (!sprintButtonHeld && !isSliding)
            {
                isWalking = true;
            }
            
            isCrouching = false;
        }   
    }

    void SlideHandler()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                isSliding = false;
                isFreelooking = false;

                if (isCrouching)
                {
                    crouchPressed = false;
                }

                if (sprintButtonHeld)
                {
                    isSprinting = true;
                    isWalking = false;
                }
            }
        }
    }

    private bool CheckWall()
    {
        // Ray to the right
        Ray rightRay = new Ray(transform.position, transform.right);
        if (Physics.Raycast(rightRay, wallRayDistance, wallMask))
        {
            wallRunRight = true;
            return true;
        }

        // Ray to the left
        Ray leftRay = new Ray(transform.position, -transform.right);
        if (Physics.Raycast(leftRay, wallRayDistance, wallMask))
        {
            wallRunLeft = true;
            return true;
        }

        wallRunRight = false;
        wallRunLeft = false;
        return false;
    }

    void WallRunHandler()
    {

        if (controller.velocity.magnitude > 9 && !isGrounded && CheckWall() && !isWallRunning && sprintButtonHeld)
        {
            wallRunTimer = wallRunTimerMax;
            isWallRunning = true;
            isFreelooking = true;
            wallRunDirection = playerDirection;  
        }
        else if (!CheckWall() || isGrounded || controller.velocity.magnitude < 9 || !sprintButtonHeld)
        {
            isWallRunning = false;

            if (!isSliding && !freeLookButtonPressed)
            {
                isFreelooking = false;
            }
        }

        if (isWallRunning)
        {
            wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0)
            {
                isWallRunning = false;
                isFreelooking = false;
            }
        }
    }

    void JumpHandler()
    {
        verticalMove = new Vector3(0, verticalVelocity, 0);
        controller.Move(verticalMove * Time.deltaTime);
    }

    void LandingHandler()
    {
        if (isGrounded && !isSliding)
        {
            if (lastVerticalVelocity < -10 && !isAnimating)
            {
                cameraAnimator.SetTrigger("CameraRollTrigger");
                isAnimating = true;
            }
            else if (lastVerticalVelocity < -4 && !isAnimating)
            {
                cameraAnimator.SetTrigger("CameraLandTrigger");
                isAnimating = true;
            }
            else
            {
                isAnimating = false;
            }
        }
        else
        {
            isAnimating = false;
        }
    }

    void MovementHandler()
    {
        if (isGrounded)
        {
            playerDirection = Vector3.Lerp(playerDirection,(transform.forward * currentMovementInput.y + transform.right * currentMovementInput.x), Time.deltaTime * lerpTime);
        }
        else if (isMoving)
        {
            playerDirection = Vector3.Lerp(playerDirection,(transform.forward * currentMovementInput.y + transform.right * currentMovementInput.x), Time.deltaTime * airLerpTime);
        }

        if (isSliding)
        {
            controller.Move(slideDirection * currentSpeed * Time.deltaTime);
        }
        else if (isWallRunning)
        {
            controller.Move(wallRunDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            if (playerDirection.magnitude >= threshold)
            {
                controller.Move(playerDirection * currentSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(Vector3.MoveTowards(playerDirection, Vector3.zero, Time.deltaTime * lerpTime) * currentSpeed * Time.deltaTime);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        sprintButtonHeld = context.performed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isCrouching)
        {
            verticalVelocity = jumpForce;
            if (!isAnimating)
            {
                cameraAnimator.SetTrigger("CameraJumpTrigger");
                isAnimating = true;
            }
        }
        else if (context.performed && isWallRunning)
        {
            verticalVelocity = jumpForce;
            if (wallRunRight)
            {
                playerDirection += -transform.right * wallJumpForce;
            }
            else if (wallRunLeft)
            {
                playerDirection += transform.right * wallJumpForce;
            }
            wallRunTimer = 0;
            isWallRunning = false;
            isFreelooking = false;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        crouchPressed = context.performed;
        if (!isSliding)
        {
            if (crouchPressed)
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }
    }

    public void OnFreelook(InputAction.CallbackContext context)
    {
        freeLookButtonPressed = context.performed;
        isFreelooking = context.performed;
    }
}