using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FarmTracker : MonoBehaviour
{
    public TextMeshProUGUI idFarm;
    public TextMeshProUGUI inspect, infection, quaratine;
    public GameObject board;
    public static int[] DayInfected = new int[150]; //First day that the farm gets infected
    public static int[] InspectDay = new int[150]; //The last day that the farm was inspected
    public static int[] QuarantSince = new int[150]; //The day when the farm got quarantined

    private void Update()
    {
        if (InspectDay[int.Parse(idFarm.text) - 1] != 0)
            this.gameObject.GetComponent<Button>().interactable = true;
        else
            this.gameObject.GetComponent<Button>().interactable = false;
    }

    public void showhideInfo()
    {
        if (board.activeSelf)
        {
            board.SetActive(false);
        }
        else
        {
            board.SetActive(true);
            refreshData();
        }
    }

    public void refreshData()
    {
        int i = int.Parse(idFarm.text) - 1;
        if (InspectDay[i] != 0)
            inspect.text = "This farm has been inspected <color=#00E4FF>" + (GameContext.sCurrentDay - InspectDay[i]) + "</color> Days ago";
        else
            inspect.text = "";
        if (DayInfected[i] != 0)
            infection.text = "This farm has been infected for: <color=#FF0000>" + (GameContext.sCurrentDay - DayInfected[i]) + "</color> Days";
        else
            infection.text = "";
        if (QuarantSince[i] != 0)
            quaratine.text = "This farm has been put on quarantine <color=#0DFF00>" + (GameContext.sCurrentDay - QuarantSince[i]) + "</color> days ago";
        else
            quaratine.text = "";
    }
}
