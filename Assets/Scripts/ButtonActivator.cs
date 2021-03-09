using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivator : MonoBehaviour
{

    public GameObject[] targetPanels;


    public void SetPanelActivity(int index, int value)
    {
        targetPanels[index].SetActive(value == 1);
    }
}
