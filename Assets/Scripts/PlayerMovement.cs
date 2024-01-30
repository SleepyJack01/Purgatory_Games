using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector2 currentMovementInput;
    
    [Header("Speed Settings")]
    private float currentSpeed;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float sprintSpeed = 12f;
    [SerializeField] float lerpTime = 6f;
    [SerializeField] float airLerpTime = 1f;
    float threshold = 0.01f;
    private Vector3 playerDirection;
    private Vector3 lastVelocity;

    [Header("Gravity Settings")]
    [SerializeField] float gravity = -9.81f;

    [Header("Jump Settings")]
    private float verticalVelocity;
    [SerializeField] float jumpForce = 5f;

    [Header("Crouch Settings")]
    private bool canStand;
    private bool crouchPressed;
    private Vector3 crouchStandCheckPosition;
    
    [Header("Ground Check Settings")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 1.1f;
    private bool isGrounded => Physics.CheckSphere(transform.position, groundDistance, groundMask);

    [Header("States")]
    private bool isMoving;
    private bool isWalking;
    private bool isSprinting;
    private bool isCrouching;

    
    

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
        JumpHandler();
        MovementHandler();
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
        if (isCrouching)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, crouchSpeed, Time.deltaTime * lerpTime);
        }
        else if (isWalking)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, walkSpeed, Time.deltaTime * lerpTime);
        }
        else if (isSprinting)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, sprintSpeed, Time.deltaTime * lerpTime);
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
        }
        else if (!crouchPressed && canStand)
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
        if (context.performed && isGrounded)
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
}
