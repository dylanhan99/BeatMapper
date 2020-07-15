using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class btnFX : MonoBehaviour
{

    public AudioSource myFx;
    public AudioClip hoverFx;
    public AudioClip clickFx; 


    public void HoverSound()
    {
        myFx.PlayOneShot(hoverFx);
    }

    public void ClickSound()
    {
        myFx.PlayOneShot(clickFx);
    }

    public void OnButtonToggle()
    {
        Button button = GetComponent<Button>();
        ColorBlock colors = button.colors;
        Color tempColor = colors.normalColor;

        colors.normalColor = colors.pressedColor;
        colors.pressedColor = tempColor;
        button.colors = colors;

    }

}
