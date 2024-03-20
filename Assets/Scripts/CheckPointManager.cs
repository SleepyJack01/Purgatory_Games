using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public int CurrentCheckPointIndex {get; private set;}

    private void Awake()
    {
        Transform[] checkPointDetectors = GetComponentsInChildren<Transform>();
        int index = 0;

        foreach (Transform checkPointDetector in checkPointDetectors)
        {
            if (checkPointDetector != transform)
            {
                CheckPointDetector detector = checkPointDetector.GetComponent<CheckPointDetector>();
                detector.SetCheckPointManager(this);
                detector.SetCheckPointIndex(index);
                index++;
            }
        }
    }

    public void CheckPointReached(CheckPointDetector checkPointDetector)
    {
        CurrentCheckPointIndex = checkPointDetector.GetCheckPointIndex();
        Debug.Log("CheckPointReached: " + CurrentCheckPointIndex);
    }
}
