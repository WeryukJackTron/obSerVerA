using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nextDayButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public void onClick()
    {
        // Increase day counter
        int dayNum = GameObject.Find("Farms").GetComponent<spreadDisease>().dayNumber;// += 1;
        Debug.Log("Day " + dayNum);
        // Call function to read the current day's event log
        GameObject.Find("Farms").GetComponent<spreadDisease>().readCurrentDayEventLog();
/*
        Debug.Log("Previously infected farms:");
        var prev = (GameObject.Find("Farms").GetComponent<spreadDisease>().prevInfectedFarms);
        Debug.Log(string.Join(", ", prev));

        Debug.Log("Currently infected farms:");
        var curr = (GameObject.Find("Farms").GetComponent<spreadDisease>().currInfectedFarms);
        Debug.Log(string.Join(", ", curr));
*/
        for(int i = 0; i < SideBarScript.instance.farms.Length; i++)
        {
            if (SideBarScript.instance.farms[i].transform.GetChild(4).gameObject.activeSelf)
            {
                SideBarScript.instance.farms[i].transform.GetChild(4).gameObject.SetActive(false);
                if (!SideBarScript.instance.farms[i].transform.GetChild(3).gameObject.activeSelf)
                {
                    SideBarScript.instance.farms[i].transform.GetChild(2).gameObject.SetActive(true);
                }
            }
        }
    }

}
