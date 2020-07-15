using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderCheck : MonoBehaviour
{
    private Slider SongposSlider;

    void Start()
    {
        SongposSlider = GetComponent<Slider>();
        SongposSlider.onValueChanged.AddListener(delegate { SongposChange(); });

    }

    // Update is called once per frame
    void SongposChange()
    {
        
    }
}
