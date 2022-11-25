using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnable : MonoBehaviour
{
    public GameObject infoPanel;

    public void onButtonClick()
    {
        if (infoPanel.activeInHierarchy == true)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(true);
        }
    }
}
