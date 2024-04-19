using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private PlayerInput playerInput;
    private PlayableDirector director;

    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject firstSelectedButton;

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

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        if (playerInput.currentControlScheme == "Gamepad")
        {
            eventSystem.SetSelectedGameObject(firstSelectedButton);
        }
        else
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    public void NewGame()
    {
        StartCoroutine(StartGame());
    }

    public void Quit()
    {
        StartCoroutine(QuitGame());
    }

    IEnumerator StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        director.Play();

        yield return new WaitForSeconds(3.5f);

        eventSystem.SetSelectedGameObject(null);
        SceneManager.LoadScene(1);
    }

    IEnumerator QuitGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        director.Play();

        yield return new WaitForSeconds(3.5f);

        eventSystem.SetSelectedGameObject(null);
        Application.Quit();
    }
}
