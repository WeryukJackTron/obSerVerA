using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class nextDayButton : MonoBehaviour
{
    public static List<int> farmid = new List<int>();
    public TextMeshProUGUI dayUI;
    public GameObject calledSVAPanel;

    // Start is called before the first frame update
    void Start()
    {
        dayUI.text = GameContext.sCurrentDay.ToString();
    }

    public void onClick()
    {
        // Get day counter
        int dayNum = (int)GameContext.sCurrentDay;// += 1;
        Debug.Log("Day " + dayNum);
        if (dayNum == 30)
            SceneManager.LoadScene("End");
        dayUI.text = (dayNum + 1).ToString();
        // Call function to advance the model by one day
        ModelHandler.Run();
        if (ModelHandler.HasLost())
            SceneManager.LoadScene("End");

        List<ushort> calling = ModelHandler.GetWhoCalled();
        if(calling.Count > 0)
        {
            List<int> callingList = new List<int>();

            foreach (ushort call in calling)
            {
                SideBarScript.Farms[call - 1].transform.GetChild(1).gameObject.SetActive(true);
                GameContext.sCalledSVA.Add(call);

                callingList.Add((int)call);
            }

            string callingStr = string.Join(", ", callingList.ToArray());

            calledSVAPanel.SetActive(true);

            GameObject subjectText = calledSVAPanel.transform.GetChild(2).gameObject;

            if (calling.Count > 1)
            {
                subjectText.GetComponent<TMPro.TextMeshProUGUI>().text = "The   following   farms   are   reporting   symptoms:";
            }
            else
            {
                subjectText.GetComponent<TMPro.TextMeshProUGUI>().text = "The   following   farm   is   reporting   symptoms:";
            }

            GameObject callingStrText = calledSVAPanel.transform.GetChild(3).gameObject;
            callingStrText.GetComponent<TMPro.TextMeshProUGUI>().text = callingStr;
        }

        GameContext.busyVets = 0;
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
        {
            if (GameContext.sFarmsInfo[i].Vet && ModelHandler.IsFarmInfected((ushort)(i+1)))
            {
                GameContext.sFarmsInfo[i].Infected = true;
            }
            GameContext.sFarmsInfo[i].Vet = false;
        }

        for(int i = 0; i < SideBarScript.Farms.Length; i++)
        {
            if (SideBarScript.Farms[i].transform.GetChild(4).gameObject.activeSelf)
            {
                ModelHandler.sUnderInvestigationFarms.Remove((ushort)(int.Parse(SideBarScript.Farms[i].name)));
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
        Debug.Log(ModelHandler.sUnderInvestigationFarms.Count);
    }

}
