using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class addDay : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    public int day;

    void Start()
    {
        //day = 1;
    }

    void Update()
    {
        dayText.text = (GameContext.sCurrentDay - day).ToString();
    }

    public void AddDay()
    {
        day++;
    }
}