using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableEnable : MonoBehaviour
{
    public GameObject infoPanel;
    public float sec = 1f;
    public GameObject infoButton;
    public Sprite hide, show;
    bool showing = false;
    public GameObject next, prev;

    public void onButtonClick()
    {
        if (infoButton.transform.GetChild(0).GetComponent<Image>().sprite == hide)
        {
            infoButton.transform.GetChild(0).GetComponent<Image>().sprite = show;
            showing = false;
        }
        else
        {
            infoButton.transform.GetChild(0).GetComponent<Image>().sprite = hide;
            showing = true;
        }

        if (infoPanel.activeInHierarchy == true)
        {
            infoPanel.SetActive(false);
            StartCoroutine(delayButtons(false));
        }
        else
        {
            infoPanel.SetActive(true);
            StartCoroutine(delayButtons(true));
            InfoScript.instance.updateFarmStatus();
        }
    }

    public IEnumerator delayButtons(bool status)
    {
        if(status)
            yield return new WaitForSeconds(1.1f);
        if (infoPanel.activeSelf || !status)
        {
            next.SetActive(status);
            prev.SetActive(status);
        }
        yield return null;
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
