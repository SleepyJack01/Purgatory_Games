using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Transform[] respawnPoints;
    [SerializeField] private Animator uiAnimator;
    private CheckPointManager checkPointManager;
    private CharacterController characterController;
    public bool isDead = false;
    

    private void Start()
    {
        checkPointManager = FindObjectOfType<CheckPointManager>();
        characterController = GetComponent<CharacterController>();
        transform.position = respawnPoints[0].position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        Debug.Log("Respawning...");
        uiAnimator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        isDead = true;
        characterController.enabled = false;
        yield return new WaitForSeconds(0.2f);
        if (checkPointManager == null)
        {
            Debug.Log("CheckPointManager not found");
            yield break;
        }
        else
        {
            int currentCheckPointIndex = checkPointManager.CurrentCheckPointIndex;
            if (currentCheckPointIndex >= respawnPoints.Length)
            {
                Debug.Log("CurrentCheckPointIndex is out of bounds");
                yield break;
            }
            transform.position = respawnPoints[currentCheckPointIndex].position;
            Debug.Log("Respawned at checkpoint " + currentCheckPointIndex);
            isDead = false;
            characterController.enabled = true;
            uiAnimator.SetTrigger("FadeIn");
        }
    }
}
