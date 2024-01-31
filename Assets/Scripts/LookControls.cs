using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookControls : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Camera cam;
    [SerializeField] Transform playerBody;
    [SerializeField] Transform camPostion;
    [SerializeField] PlayerInput playerInput;

    [Header("Sensitivity Settings")]
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField] float controllerSensitivity = 100f;
    private float sensitivity;

    [Header("Camera Settings")]
    [SerializeField] float cameraClamp = 90f;

    // Look controls
    private Vector2 lookInput;
    private Vector2 LastMousePos;
    private Vector2 totalMouseDelta = Vector2.zero;

    // Rotation controls
    private float lookX;
    private float lookY;
    private float xRotation = 0f;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += ctx => SetSensitivity();
    }

    void Start()
    {
        SetSensitivity();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CalculateInput();
    }

    void LateUpdate()
    {
        PositionCamera();
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

    // Attach the camera to the players "head"
    void PositionCamera()
    {
        cam.transform.position = camPostion.position;
    }

    // Apply the rotation to the camera and player
    void ApplyCameraRotation()
    {
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -cameraClamp, cameraClamp);
        
        playerBody.Rotate(Vector3.up * lookX);
        cam.transform.rotation = Quaternion.Euler(xRotation, camPostion.rotation.eulerAngles.y, 0f);
    }

    // Gets the raw input from the input system
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
