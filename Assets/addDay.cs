using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class addDay : MonoBehaviour
{

    public int day;
    public TextMeshProUGUI dayText;

    void Start()
    {
        day = 1;
    }

    void Update()
    {
        dayText.text = day.ToString();
    }

    public void AddDay()
    {
        day++;
    }
}