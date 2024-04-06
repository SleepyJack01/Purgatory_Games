using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeatBarHandler : MonoBehaviour
{
    public Slider heatBarSlider;

    private void Awake() 
    {
       heatBarSlider = GetComponent<Slider>(); 
    }

    public void SetMaxHeatBar(float heat)
    {
        heatBarSlider.maxValue = heat;
        heatBarSlider.value = heat;
    }

    public void SetHeatBar(float heat)
    {
        heatBarSlider.value = heat;
    }
}
