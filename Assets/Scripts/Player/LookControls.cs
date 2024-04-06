using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] Transform neck;
    [SerializeField] Transform head;
    [SerializeField] Transform eyes;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform camPostion;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PauseMenuHandler pauseMenuHandler;

    [Header("Sensitivity Settings")]
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField] float controllerSensitivity = 100f;
    private float sensitivity;

    [Header("Camera Settings")]
    [SerializeField] float cameraClamp = 90f;
    [SerializeField] float freeLookMaxAngle = 120f;
    [SerializeField] private float freeLookTiltAmount = 0.2f;
    [SerializeField] private float slideTiltAmount = -7f;
    [SerializeField] private float wallRunTiltAmount = 9f;

    // Look controls
    private Vector2 lookInput;
    private Vector2 LastMousePos;
    private Vector2 totalMouseDelta = Vector2.zero;

    // Rotation controls
    private float lookX;
    private float lookY;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        playerInput.onControlsChanged += ctx => SetSensitivity();
        playerMovement = GetComponent<PlayerMovement>();
        pauseMenuHandler = FindObjectOfType<PauseMenuHandler>();
    }

    void Start()
    {
        SetSensitivity();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CalculateInput();
    }

    void LateUpdate()
    {
        ApplyCameraRotation();
    }

    // Sets the sensitivity based on the current control scheme
    public void SetSensitivity()
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard and Mouse":
                sensitivity = mouseSensitivity;
                break;
            case "Gamepad":
                sensitivity = controllerSensitivity;
                break;
            default:
                Debug.LogWarning("Unknown control scheme: " + playerInput.currentControlScheme);
                break;
        }
    }

    // Calculates the input based on the current control scheme
    void CalculateInput()
    {
        if (playerInput.currentControlScheme == "Keyboard and Mouse")
        {
            Vector2 currentMousePos = lookInput;
            Vector2 mouseDelta = currentMousePos - LastMousePos;
            LastMousePos = currentMousePos;

            totalMouseDelta += mouseDelta;

            lookX = totalMouseDelta.x * sensitivity;
            lookY = totalMouseDelta.y * sensitivity;
        }
        else if (playerInput.currentControlScheme == "Gamepad")
        {
            lookX = lookInput.x * sensitivity * Time.deltaTime;
            lookY = lookInput.y * sensitivity * Time.deltaTime;
        }
    }

    // Apply the rotation to the camera and player
    void ApplyCameraRotation()
    {

        if (playerMovement.isFreelooking)
        {
            yRotation += lookX;
            yRotation = Mathf.Clamp(yRotation, -freeLookMaxAngle, freeLookMaxAngle);
            neck.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
            if (playerMovement.isSliding)
            {
                eyes.transform.localRotation = Quaternion.Lerp(eyes.transform.localRotation, Quaternion.Euler(0f, 0f, slideTiltAmount), Time.deltaTime * 6f);
                // Lower the head to give the illusion of sliding
                head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, new Vector3(0, -0.4f, 0), Time.deltaTime * 6f);
            }
            else if (playerMovement.isWallRunning)
            {
                if (playerMovement.wallRunRight)
                {
                    eyes.transform.localRotation = Quaternion.Lerp(eyes.transform.localRotation, Quaternion.Euler(0f, 0f, wallRunTiltAmount), Time.deltaTime * 6f);
                    head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, new Vector3(-0.2f, -0.1f, 0f), Time.deltaTime * 6f);
                }
                else if (playerMovement.wallRunLeft)
                {
                    eyes.transform.localRotation = Quaternion.Lerp(eyes.transform.localRotation, Quaternion.Euler(0f, 0f, -wallRunTiltAmount), Time.deltaTime * 6f);
                    head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, new Vector3(0.2f, -0.1f, 0f), Time.deltaTime * 6f);
                }
            }
            else
            {
                eyes.transform.localRotation = Quaternion.Euler(0f, 0f, yRotation * freeLookTiltAmount);
            }
        }
        else
        {
            // Raise the head back to normal position
            head.transform.localPosition = Vector3.Lerp(head.transform.localPosition, Vector3.zero, Time.deltaTime * 6f);

            xRotation -= lookY;
            xRotation = Mathf.Clamp(xRotation, -cameraClamp, cameraClamp);

            neck.transform.localRotation = Quaternion.Lerp(neck.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 6f);
            eyes.transform.localRotation = Quaternion.Lerp(eyes.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 6f);
            yRotation = 0f;
            playerBody.Rotate(Vector3.up * lookX);
            head.transform.rotation = Quaternion.Euler(xRotation, neck.rotation.eulerAngles.y, 0f);
        }
    }

    // Gets the raw input from the input system
    public void OnLook(InputAction.CallbackContext context)
    {
        if (pauseMenuHandler.isPaused)
        {
            lookInput = Vector2.zero;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
    }
}
