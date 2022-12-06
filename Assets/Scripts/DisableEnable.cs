using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnable : MonoBehaviour
{
    public GameObject infoPanel;
    public float sec = 1f;

    public void onButtonClick()
    {   
        //StartCoroutine(AnimationFirst(sec));

        if (infoPanel.activeInHierarchy == true)
        {
            infoPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(true);
        }
    }
    /*
    public IEnumerator AnimationFirst(float seconds)
    {
        if (infoPanel.activeInHierarchy == true)
        {
            yield return new WaitForSeconds(seconds);
            infoPanel.SetActive(false);
        }
        else
        {
            infoPanel.SetActive(true);
        }
    }
    */
}
