using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private GameObject player;
    [SerializeField] private Animator uiAnimator;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject firstSelectedButton;
    public bool isPaused = false;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnControlsChangeSchemeChanaged()
    {
        switch (playerInput.currentControlScheme)
        {
            case "Keyboard and Mouse":
                eventSystem.SetSelectedGameObject(null);
                break;
            case "Gamepad":
                eventSystem.SetSelectedGameObject(firstSelectedButton);
                break;
            default:
                Debug.LogWarning("Unknown control scheme: " + playerInput.currentControlScheme);
                break;
        }
    }

    public void PauseGame()
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
        playerInput.SwitchCurrentActionMap("UI");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        menu.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        playerInput.SwitchCurrentActionMap("PlayerControls");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
        menu.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        isPaused = false;
    }

    public void RestartScene()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name));
    }

    public void QuitToMainMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        StartCoroutine(LoadScene("Main Menu"));
    }

    public void DebugButton1()
    {
        SpawnPlayerAtCheckPoint(1);
    }

    public void DebugButton2()
    {
        SpawnPlayerAtCheckPoint(2);
    }

    public void DebugButton3()
    {
        SpawnPlayerAtCheckPoint(3);
    }

    public void DebugButton4()
    {
        SpawnPlayerAtCheckPoint(4);
    }

    public void DebugButton5()
    {
        SpawnPlayerAtCheckPoint(5);
    }

    IEnumerator LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        uiAnimator.SetTrigger("FadeOut");
        playerInput.SwitchCurrentActionMap("PlayerControls");
        menu.SetActive(false);

        yield return new WaitForSeconds(1f);
        
        eventSystem.SetSelectedGameObject(null);
        isPaused = false;
        SceneManager.LoadScene(sceneName);
    }


    public void SpawnPlayerAtCheckPoint(int checkPointIndex)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput.SwitchCurrentActionMap("PlayerControls");
        menu.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        Time.timeScale = 1;
        isPaused = false;
        Debug.Log("Spawning player at checkpoint: " + checkPointIndex);
        CharacterController characterController = player.GetComponent<CharacterController>();
        characterController.enabled = false;
        player.transform.position = playerManager.respawnPoints[checkPointIndex].position;
        characterController.enabled = true;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.timeScale == 0)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}
