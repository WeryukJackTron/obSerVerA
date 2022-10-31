using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SideBarScript : MonoBehaviour
{
    public GameObject[] farms;
    public GameObject farmids;
    public Sprite hide, show;
    public static SideBarScript instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (showing)
        {
            for (int i = 0; i < farms.Length; i++)
            {
                if(!farms[i].transform.GetChild(0).GetChild(0).gameObject.activeSelf)
                    farms[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(showing);
            }
        }
    }

    bool showing = false;
    public void ShowHideFarmIDs()
    {
        if(farmids.transform.GetChild(0).GetComponent<Image>().sprite == hide)
        {
            /*farmids.transform.GetComponent<SpriteRenderer>().color = Color.black;
            farmids.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "off";*/
            farmids.transform.GetChild(0).GetComponent<Image>().sprite = show;
            showing = false;
        }
        else
        {
            farmids.transform.GetChild(0).GetComponent<Image>().sprite = hide;
            //farmids.transform.GetComponent<SpriteRenderer>().color = Color.red;
            //farmids.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "on";
            showing = true;
        }
        if (!showing)
            for(int i = 0; i < farms.Length; i++)
            {
                farms[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(showing);
            }
    }

}
