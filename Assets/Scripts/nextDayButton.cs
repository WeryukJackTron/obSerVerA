using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class nextDayButton : MonoBehaviour
{
    public static List<int> farmid = new List<int>();

    // Start is called before the first frame update
    void Start()
    {

    }

    public void onClick()
    {
        // Get day counter
        int dayNum = (int)GameContext.sCurrentDay;// += 1;
        Debug.Log("Day " + dayNum);
        if (dayNum == 30)
            SceneManager.LoadScene("End");
        // Call function to advance the model by one day
        ModelHandler.Run();

        for(int i = 0; i < farmid.Count; i++)
            FarmsScript.instance.quarantine(farmid[i]);

        farmid.Clear();
        SideBarScript.instance.gameObject.transform.GetChild(3).GetChild(0).GetComponent<Button>().interactable = true;
        /*
                Debug.Log("Previously infected farms:");
                var prev = (GameObject.Find("Farms").GetComponent<spreadDisease>().prevInfectedFarms);
                Debug.Log(string.Join(", ", prev));

                Debug.Log("Currently infected farms:");
                var curr = (GameObject.Find("Farms").GetComponent<spreadDisease>().currInfectedFarms);
                Debug.Log(string.Join(", ", curr));
        */
        
        for (int i = 0; i < GameContext.sFarmsInfo.Length; i++)
            GameContext.sFarmsInfo[i].Vet = false;

        for(int i = 0; i < SideBarScript.Farms.Length; i++)
        {
            if (SideBarScript.Farms[i].transform.GetChild(4).gameObject.activeSelf)
            {
                SideBarScript.Farms[i].transform.GetChild(4).gameObject.SetActive(false);
                if (!ModelHandler.IsFarmInfected((ushort)int.Parse(SideBarScript.Farms[i].name)) && SideBarScript.Farms[i].transform.childCount < 6)
                {
                    SideBarScript.Farms[i].transform.GetChild(2).gameObject.SetActive(true);
                }
                else if(SideBarScript.Farms[i].transform.childCount < 6)
                {
                    SideBarScript.Farms[i].transform.GetChild(3).gameObject.SetActive(true);
                }
            }
        }
    }

}
