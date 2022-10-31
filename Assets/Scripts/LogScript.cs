using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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

    public void sendVets()
    {
        SideBarScript.instance.farms[0].transform.GetChild(4).gameObject.SetActive(true);
        SideBarScript.instance.farms[15].transform.GetChild(4).gameObject.SetActive(true);
        SideBarScript.instance.farms[20].transform.GetChild(4).gameObject.SetActive(true);
        resetCheckbox();
        FarmsScript.instance.Map.SetActive(true);
        FarmsScript.instance.Log.SetActive(false);
        TestScript.instance.ShowHideCheckBoxes();
    }
}
