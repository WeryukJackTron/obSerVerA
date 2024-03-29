using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoScript : MonoBehaviour
{
    public GameObject settings;

    public int maxMessages = 10;

    public GameObject QuarantinePanel;
    public GameObject VetPanel;
    public GameObject InfectedPanel;
    public GameObject CleanPanel;

    public GameObject infoPanel;
    public GameObject textObject;

    [SerializeField]
    List<Message> messageList = new List<Message>();
    List<Message> quarantineList = new List<Message>();
    List<Message> vetList = new List<Message>();
    List<Message> infectedList = new List<Message>();
    List<Message> cleanList = new List<Message>();


    public TextMeshProUGUI myText;
    public GameObject grid;
    static string farmID;

    public static InfoScript instance;

    public static class G_Variable
    {
        public static GameObject SelectedObject;
    }

    public void onButtonClick()
    {
        G_Variable.SelectedObject = gameObject;
        Debug.Log(G_Variable.SelectedObject.name);
    }

    private void Start()
    {
        instance = this;
        settings.SetActive(false);
        //gameObject.SetActive(false);
    }

    public static void setIDFarm(string id)
    {
        farmID = id;
    }

    public void GetInfo()
    {
        //farmID = G_Variable.SelectedObject.transform.parent.gameObject.name;
        PrintQuarantines("- A  quarantine  zone  is  currently  being  set up  at  farm  " + farmID + ".");
        //Debug.Log("The ID of the farm is" + farmID);
    }

    public void PrintQuarantines(string text)
    {
        if (quarantineList.Count >= maxMessages)
        {
            Destroy(quarantineList[0].textObject.gameObject);
            quarantineList.Remove(quarantineList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, QuarantinePanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;

        quarantineList.Add(newMessage);
    }

    public void PrintVets(string text)
    {
        if (vetList.Count >= maxMessages)
        {
            Destroy(vetList[0].textObject.gameObject);
            vetList.Remove(vetList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, VetPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;

        vetList.Add(newMessage);
    }

    public void PrintInfected(string text)
    {
        if (infectedList.Count >= maxMessages)
        {
            Destroy(infectedList[0].textObject.gameObject);
            infectedList.Remove(infectedList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, InfectedPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;

        infectedList.Add(newMessage);
    }

    public void PrintClean(string text)
    {
        if (cleanList.Count >= maxMessages)
        {
            Destroy(cleanList[0].textObject.gameObject);
            cleanList.Remove(cleanList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, CleanPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;

        cleanList.Add(newMessage);
    }


    public void PrintInfo(string text)
    {
        //farmID = G_Variable.SelectedObject.name;
        //Debug.Log(farmID);

        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        newMessage.text = text;

        GameObject newText = Instantiate(textObject, infoPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);

        /*if (G_Variable.SelectedObject.transform.GetChild(4).gameObject.activeSelf)
        {
            myText.text = "farm" + farmID;
        }*/
    }

    public void ClearInfo()
    {
        messageList.Clear();
        for(int i = infoPanel.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(infoPanel.transform.GetChild(i).gameObject);
        }
    }

    int page = 1;
    public void updateFarmStatus()
    {
        int start = 0;
        switch (page)
        {
            case 1:
                start = 1;
                break;
            case 2:
                start = 51;
                break;
            case 3:
                start = 101;
                break;
        }
        for(int i = 0; i < grid.transform.childCount; i++)
        {
            grid.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = start.ToString();
            if (ModelHandler.sInfectedVisibleFarms.Contains((ushort)start))
            {
                grid.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = Color.red;
            }
            else if (ModelHandler.sUnderInvestigationFarms.Contains((ushort)start))
            {
                grid.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = Color.cyan;
            }
            else
            {
                grid.transform.GetChild(i).GetComponent<TextMeshProUGUI>().color = Color.white;
            }
            start++;
        }
    }

    public void nextPage()
    {
        Debug.Log("next");
        if(page < 3)
        {
            page++;
            updateFarmStatus();
        }
    }

    public void prevPage()
    {
        Debug.Log("prev");
        if(page > 1)
        {
            page--;
            updateFarmStatus();
        }
    }

    public void backtoMainmenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}

[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
}
