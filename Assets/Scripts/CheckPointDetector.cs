using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDetector : MonoBehaviour
{
    private CheckPointManager checkPointManager;
    private int index;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkPointManager.CheckPointReached(this);
        }
    }

    public void SetCheckPointManager(CheckPointManager checkPointManager)
    {
        this.checkPointManager = checkPointManager;
    }

    public void SetCheckPointIndex(int index)
    {
        this.index = index + 1;
    }

    public int GetCheckPointIndex()
    {
        return this.index;
    }
}
