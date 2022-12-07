using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class EndScript : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Subtitle;

    public TextMeshProUGUI VetsNum;
    public TextMeshProUGUI ZonesNum;

    // Start is called before the first frame update
    void Start()
    {
        VetsNum.SetText(GameContext.sVetsSend.ToString());
        ZonesNum.SetText(GameContext.sZonePlaced.ToString());
        if (ModelHandler.HasWon())
        {
            Title.SetText("You Won");
            Subtitle.SetText("Congratulations you manage to quarantine all infected farms");
        }
        else//You obviously lost
        {
            Title.SetText("You Lost");
            if (GameContext.sCurrentDay >= 30)
                Subtitle.SetText("Unfortunately you didn't manage to restrict the infection in time");
            else
                Subtitle.SetText("The virus went out of control, half or more of the farms are infected");
        }
    }
}
