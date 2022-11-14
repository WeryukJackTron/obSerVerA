using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoScript : MonoBehaviour
{
    public int maxMessages = 10;
    public GameObject infoPanel;
    public GameObject textObject;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    public TextMeshProUGUI myText;
    string farmID;

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

    public void GetInfo()
    {
        farmID = G_Variable.SelectedObject.transform.parent.gameObject.name;
        PrintInfo("A  quarantine  zone  is  currently  being  set up  at  farm  " + farmID + ".\n ");
        //Debug.Log("The ID of the farm is" + farmID);
    }

    void PrintInfo(string text)
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
}

[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
}
