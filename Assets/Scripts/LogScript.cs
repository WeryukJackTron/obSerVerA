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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resetCheckbox()
    {
        foreach(Transform transform in this.transform)
        {
            if (!transform.gameObject.activeSelf)
                break;
            transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        //For some reason doesn't work Unity complains that Child is out of bounds when use GetChild(1)
        // Just like ShowHideCheckBoxes in TestScript
        //for (int i = 0; i < this.transform.childCount; i++)
        //{
        //    this.transform.GetChild(i).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = "";
        //}
    }

    public static int[] checkboxes = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    public void sendVets()
    {
        for(int i = 0; i < checkbox.Count; i++)
        {
            if(checkbox[i].text != "" && checkboxes[i] != 0)
            {
                if (SideBarScript.instance.farms[checkboxes[i] - 1].transform.GetComponent<SpriteRenderer>().sprite != FarmsScript.Infected)
                    SideBarScript.instance.farms[checkboxes[i] - 1].transform.GetChild(4).gameObject.SetActive(true);
            }
        }
        resetCheckbox();
        GameContext.Map.SetActive(true);
        GameContext.Log.SetActive(false);
        TestScript.instance.ShowHideCheckBoxes();
    }
}
