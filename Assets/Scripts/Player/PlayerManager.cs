using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public Transform[] respawnPoints;
    [SerializeField] private Animator uiAnimator;
    [SerializeField] private HeatBarHandler heatBarHandler;
    private CheckPointManager checkPointManager;
    private CharacterController characterController;
    public float maxHeat = 240;
    public float currentHeat;
    public bool isDead = false;
    public bool isHeated = true;
    

    private void Start()
    {
        checkPointManager = FindObjectOfType<CheckPointManager>();
        characterController = GetComponent<CharacterController>();
        transform.position = respawnPoints[0].position;

        currentHeat = maxHeat;
        heatBarHandler.SetMaxHeatBar(maxHeat);
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (!isHeated)
        {
            currentHeat -= Time.deltaTime * 2;
            if (currentHeat <= 0)
            {
                StartCoroutine(ColdRespawn());
            }
        }
        else
        {
            if (currentHeat < maxHeat)
            {
                currentHeat += Time.deltaTime * 16;
            }
        }

        heatBarHandler.SetHeatBar(currentHeat);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            StartCoroutine(Respawn());
        }
        if (other.CompareTag("Heat"))
        {
            isHeated = true;
            Debug.Log("Heated");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Heat"))
        {
            isHeated = false;
            Debug.Log("Not Heated");
        }
    }

    public IEnumerator Respawn()
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
            currentHeat = maxHeat;
            characterController.enabled = true;
            uiAnimator.SetTrigger("FadeIn");
        }
    }

    IEnumerator ColdRespawn()
    {
        Debug.Log("Respawning...");
        uiAnimator.SetTrigger("SlowFadeOut");
        yield return new WaitForSeconds(2);
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
            currentHeat = maxHeat;
            characterController.enabled = true;
            uiAnimator.SetTrigger("FadeIn");
        }
    }
}