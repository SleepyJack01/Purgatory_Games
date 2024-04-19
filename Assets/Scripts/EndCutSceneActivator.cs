using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCutSceneActivator : MonoBehaviour
{
    [SerializeField] private GameObject EndGameCutScene;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if EndGameCutScene is not active activate it.
            if (!EndGameCutScene.activeSelf)
            {
                EndGameCutScene.SetActive(true);
            }
            
        }
    }
}
