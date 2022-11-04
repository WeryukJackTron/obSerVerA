using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    public List<TextMeshProUGUI> checkbox;
    public static LogScript instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        checkboxes = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetCheckbox()
    {
        for(int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public int[] checkboxes;
    public void sendVets()
    {
        for(int i = 0; i < checkbox.Count; i++)
        {
            if(checkbox[i].text != "" && checkboxes[i] != 0)
            {
                if (!spreadDisease.instance.currInfectedFarms.Contains((ushort)checkboxes[i]))
                    SideBarScript.instance.farms[checkboxes[i] - 1].transform.GetChild(4).gameObject.SetActive(true);
            }
        }
        resetCheckbox();
        FarmsScript.instance.Map.SetActive(true);
        FarmsScript.instance.Log.SetActive(false);
        TestScript.instance.ShowHideCheckBoxes();
    }
}
