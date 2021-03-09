using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MuteButton : MonoBehaviour
{
    public bool isMuted;


    public Slider soundSlider;
    public TextMeshProUGUI soundLabel;

    public Sprite[] buttonSprites;

    public void changeMuteStatus()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            GetComponent<Image>().sprite = buttonSprites[0];
            soundSlider.interactable = false;
            soundLabel.alpha = .3f;
        }
        else
        {
            GetComponent<Image>().sprite = buttonSprites[1];
            soundSlider.interactable = true;
            soundLabel.alpha = 1f;
        }
    }

}
