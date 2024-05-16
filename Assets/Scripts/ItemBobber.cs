using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBobber : MonoBehaviour
{
    private Vector3 startingPosition;
    [SerializeField] Vector3 movementVector;
    [SerializeField] [Range(0,1)] float movementFactor;
    [SerializeField] float period = 2f;
    [SerializeField] float rotationSpeed = 30f;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime); // rotate the object slowly

        if (period <= Mathf.Epsilon) {return;}
        float cycles = Time.time / period; // continually growing over time

        const float tau = Mathf.PI * 2; // constant calue of 6.283
        float rawSinWave = Mathf.Sin(cycles * tau); //  going from -1 to 1

        movementFactor = (rawSinWave + 1f) /2f; // recalculated to go from 0 to 1 so its cleaner

        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPosition + offset;
    }
}
