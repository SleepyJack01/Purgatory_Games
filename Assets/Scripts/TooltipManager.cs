using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public int CurrentTooltipIndex {get; private set;}
    [SerializeField] GameObject tooltipMenu;
    [SerializeField] TextMeshProUGUI tooltipText;

    private void Awake()
    {
        TooltipDetector[] tooltipDetectors = GetComponentsInChildren<TooltipDetector>();
        int index = 0;

        foreach (TooltipDetector tooltipDetector in tooltipDetectors)
        {
            if (tooltipDetector != gameObject)
            {
                TooltipDetector detector = tooltipDetector.GetComponent<TooltipDetector>();
                if (detector != null)
                {
                    detector.SetTooltipManager(this);
                    detector.SetTooltipIndex(index);
                    index++;
                }
                else
                {
                    Debug.LogWarning("Child object " + tooltipDetector.name + " does not have a TooltipDetector component.");
                }
            }
        }
    }

    public void TooltipReached(TooltipDetector tooltipDetector)
    {
        CurrentTooltipIndex = tooltipDetector.GetTooltipIndex();
        Debug.Log("TooltipReached: " + CurrentTooltipIndex);
    }

    public void ShowTooltip()
    {
        tooltipMenu.SetActive(true);
        
        // Set the text of the tooltip depending on which tooltipDetector was reached using a switch statement
        switch (CurrentTooltipIndex)
        {
            case 0:
                tooltipText.text = "You can jump by pressing the space bar on your keyboard, or the A or LB button on your controller.";
                break;
            case 1:
                tooltipText.text = "To crouch under objects, press the ctrl key on your keyboard, or the B or RB button on your controller.";
                break;
            case 2:
                tooltipText.text = "You can grab onto ledges by jumping towards them";
                break;
            case 3:
                tooltipText.text = "You can sprint by holding the shift key on your keyboard, or pressing L3 on your controller. Press the crouch key while sprinting to slide under objects.";
                break;
            case 4:
                tooltipText.text = "You'll need to wallhop to reach the next platform. To wallhop, jump towards the wall and press the jump key again while in the air.";
                break;
            case 5:
                tooltipText.text = "You can wallrun by sprinting and jumping alongside a wall, press the jump key again while in the air to begin running along the wall.";
                break;
            case 6:
                tooltipText.text = "It's cold outside. You'll need to find a way to warm up before you freeze to death. Look for a fire to warm up at.";
                break;
            default:
                tooltipText.text = "If your seeing this, the tooltip text has not been set for this object. Bad developer!";
                break;
        }

    }

    public void HideTooltip()
    {
        tooltipMenu.SetActive(false);
    }


}
