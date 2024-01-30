using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class ControlSchemeManager : MonoBehaviour
{
    public UnityEvent onControlsChanged;
    private PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerInput.onControlsChanged += (obj) => onControlsChanged?.Invoke();
    }

    public string GetCurrentControlScheme()
    {
        return playerInput.currentControlScheme;
    }
}
