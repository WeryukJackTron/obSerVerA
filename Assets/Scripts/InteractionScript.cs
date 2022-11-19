using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionScript : MonoBehaviour
{
    public int idfarm = 0, interaction_type = 0;
    public bool zoned = false;

    public static InteractionScript instance;

    public void Start()
    {
        instance = this;
    }

    public void setFarmId(int id)
    {
        idfarm = id;
        if (idfarm != 0 && interaction_type != 0)
            UIinteraction();
    }

    public void setInteractionType(int type)
    {
        interaction_type = type;
        if (idfarm != 0 && interaction_type != 0)
            UIinteraction();
    }

    public void UIinteraction()
    {
        if(interaction_type == 2 && GameContext.sFarmsInfo[idfarm - 1].Zone == false)
        {
            InfoScript.setIDFarm(idfarm.ToString());
            InfoScript.instance.GetInfo();
            foreach(GameObject gameObject in SideBarScript.Farms)
            {
                if (int.Parse(gameObject.name) != idfarm)
                    continue;

                gameObject.transform.GetChild(4).gameObject.SetActive(true);
                gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;

                GameContext.sFarmsInfo[idfarm - 1].Vet = true;
            }
            //SideBarScript.instance.farms[idfarm - 1].transform.GetChild(4).gameObject.SetActive(true);
            //SideBarScript.instance.farms[idfarm - 1].transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;
            nextDayButton.farmid.Add(idfarm);
        }
        else if(interaction_type == 1)
        {
            GameContext.Log.SetActive(true);
            GameContext.Log.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = idfarm.ToString();
            GameContext.Log.GetComponent<TestScript>().UpdateLog();
            GameContext.Log.GetComponent<TestScript>().checkFarm();
            GameContext.Map.SetActive(false);
        }
        idfarm = interaction_type = 0;
    }
}
