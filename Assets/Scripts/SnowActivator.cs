using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowActivator : MonoBehaviour
{
    [SerializeField] private GameObject playerSnow;
    [SerializeField] private GameObject windowSnow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if snow is not active activate it.
            if (!playerSnow.activeSelf)
            {
                playerSnow.SetActive(true);
                windowSnow.SetActive(false);
            }
            else
            {
                // if snow is active deactivate it.
                playerSnow.SetActive(false);
                windowSnow.SetActive(true);
            }
        }
    }
}
