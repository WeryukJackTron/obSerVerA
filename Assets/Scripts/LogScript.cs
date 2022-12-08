using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    public List<TextMeshProUGUI> checkbox;
    public GameObject checkboxSelf;
    public TextMeshProUGUI farmidLog;

    public static LogScript instance;

    InfoScript infoScript; // Added by Petter
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        infoScript = GameObject.FindGameObjectWithTag("Info").GetComponent<InfoScript>(); // Added by Petter
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
            transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
        if(checkboxSelf.activeSelf && checkboxSelf.transform.GetChild(0).gameObject.activeSelf)
            checkboxSelf.transform.GetComponentInChildren<TextMeshProUGUI>().text = "";

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
            if (GameContext.busyVets < GameContext.maxVets)
            {
                if (checkbox[i].text != "" && checkboxes[i] != 0)
                {
                    foreach (GameObject gameObject in SideBarScript.Farms)
                    {
                        if (int.Parse(gameObject.name) != checkboxes[i])
                            continue;
                        Transform transform = gameObject.transform;
                        if (transform.GetComponent<SpriteRenderer>().sprite != FarmInitScript.Infected && !ModelHandler.sUnderInvestigationFarms.Contains((ushort)int.Parse(gameObject.name)))
                        {
                            GameContext.busyVets++;
                            Debug.Log(GameContext.busyVets + " Vets");
                            transform.GetChild(4).gameObject.SetActive(true);
                            GameContext.sVetsSend++;
                            infoScript.PrintVets("- A vet has been sent to " + gameObject.name + "."); // Added by Petter
                            ModelHandler.sUnderInvestigationFarms.Add((ushort)int.Parse(transform.gameObject.name));
                        }
                        break;
                    }
                    GameContext.sFarmsInfo[checkboxes[i] - 1].Vet = true;
                    //if (SideBarScript.instance.farms[checkboxes[i] - 1].transform.GetComponent<SpriteRenderer>().sprite != FarmsScript.Infected)
                    //    SideBarScript.instance.farms[checkboxes[i] - 1].transform.GetChild(4).gameObject.SetActive(true);
                }
            }
        }
        if (GameContext.busyVets < GameContext.maxVets)
        {
            if (!ModelHandler.sInfectedVisibleFarms.Contains((ushort)int.Parse(farmidLog.text)) && !ModelHandler.sUnderInvestigationFarms.Contains((ushort)int.Parse(farmidLog.text)))
            {
                GameContext.busyVets++;
                Debug.Log(GameContext.busyVets + " Vets");
                SideBarScript.Farms[int.Parse(farmidLog.text) - 1].transform.GetChild(4).gameObject.SetActive(true);
                GameContext.sVetsSend++;
                infoScript.PrintVets("- A vet has been sent to " + farmidLog.text + ".");
                ModelHandler.sUnderInvestigationFarms.Add((ushort)int.Parse(farmidLog.text));
            }
        }
        resetCheckbox();
        GameContext.Map.SetActive(true);
        GameContext.Log.SetActive(false);
        TestScript.instance.ShowHideCheckBoxes();
    }
}
