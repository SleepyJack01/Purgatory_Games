using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBeacon : MonoBehaviour
{
    [SerializeField] private Light beacon;
    [SerializeField] private float interval = 1;

    float timer;

    private void Awake() 
    {
        beacon = GetComponentInChildren<Light>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            beacon.enabled = !beacon.enabled;
            timer -= interval;
        }
    }
}
