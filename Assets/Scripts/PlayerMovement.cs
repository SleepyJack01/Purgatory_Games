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
    private CharacterController controller;
    private PlayerInput playerInput;

    [Header("Input Varibles")]
    private Vector2 currentMovementInput;
    private Vector3 playerDirection;
    private Vector3 lastVelocity;
    
    [Header("Speed Settings")]
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float sprintSpeed = 12f;
    [SerializeField] private float slideSpeed = 14f;
    [SerializeField] float lerpTime = 6f;
    [SerializeField] float airLerpTime = 1f;
    private float currentSpeed;
    float threshold = 0.01f;

    [Header("Gravity Settings")]
    [SerializeField] float gravity = -9.81f;

    [Header("Jump Settings")]
    private float verticalVelocity;
    [SerializeField] float jumpForce = 5f;

    [Header("Crouch Settings")]
    private bool canStand;
    private bool crouchPressed;
    private Vector3 crouchStandCheckPosition;

    [Header("Sliding Settings")]
    [SerializeField] private float slideTimerMax = 1.5f;
    private float slideTimer = 0.0f;
    private Vector2 slideDirection;
    
    
    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 1.1f;
    private bool isGrounded => Physics.CheckSphere(transform.position, groundDistance, groundMask);

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
    [HideInInspector] public bool isFreelooking;
    private bool isSliding;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
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
        CrouchHandler();
        SlideHandler();
        JumpHandler();
        MovementHandler();
    }

    void LateUpdate()
    {
        if (enableHeadBob)
        {
            HeadBobbingHandler();
        }  
    }

    void applyGravity()
    {
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    void SpeedHandler()
    {
        if (isCrouching && isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, crouchSpeed, Time.deltaTime * lerpTime);
        }
        else if (isSliding)
        {
            currentSpeed = (slideTimer + 0.2f) * slideSpeed;
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

        if (isGrounded && isMoving)
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
        
        if (crouchPressed)
        {
            // If player is crouching, reduce player's height
            controller.height = Mathf.Lerp(controller.height, 1f, Time.deltaTime * lerpTime);
            isCrouching = true;

            if (isSprinting && isMoving)
            {
                isSliding = true;
                slideTimer = slideTimerMax;
                slideDirection = (transform.forward * currentMovementInput.y + transform.right * currentMovementInput.x);

                isFreelooking = true;
                isWalking = false;
                isSprinting = false;
            }
        }
        else if (!crouchPressed && canStand && !isSliding)
        {
            float newHeight = Mathf.Lerp(controller.height, 2f, Time.deltaTime * lerpTime);
            float heightDifference = newHeight - controller.height;
            controller.height = newHeight;
            if (heightDifference > 0)
            {
                controller.Move(new Vector3(0, heightDifference / 2, 0));
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
            }
        }
    }

    void JumpHandler()
    {
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0);
        controller.Move(verticalMove * Time.deltaTime);
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

        if (playerDirection.magnitude >= threshold)
        {
            controller.Move(playerDirection * currentSpeed * Time.deltaTime);
        }
        else
        {
            controller.Move(Vector3.MoveTowards(playerDirection, Vector3.zero, Time.deltaTime * lerpTime) * currentSpeed * Time.deltaTime);
        }

        lastVelocity = controller.velocity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isWalking = false;
            isSprinting = true;
        }
        else if (context.canceled)
        {
            isWalking = true;
            isSprinting = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && !isCrouching)
        {
            verticalVelocity = jumpForce;
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouchPressed = true;
        }
        else if (context.canceled)
        {
            crouchPressed = false;
        }
    }

    public void OnFreelook(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isFreelooking = true;
        }
        else if (context.canceled)
        {
            isFreelooking = false;
        }
    }
}