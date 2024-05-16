using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipDetector : MonoBehaviour
{
    
    private TooltipManager tooltipManager;
    private int index;

    void Start()
{
    tooltipManager = FindObjectOfType<TooltipManager>();
    if (tooltipManager == null)
    {
        Debug.LogError("TooltipManager not found in the scene.");
    }
}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tooltipManager.TooltipReached(this);
            tooltipManager.ShowTooltip();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tooltipManager.HideTooltip();
        }
    }

    public void SetTooltipManager(TooltipManager tooltipManager)
    {
        this.tooltipManager = tooltipManager;
    }

    public void SetTooltipIndex(int index)
    {
        this.index = index;
    }

    public int GetTooltipIndex()
    {
        return this.index;
    }
    
}
