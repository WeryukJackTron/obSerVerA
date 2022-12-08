using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SideBarScript : MonoBehaviour
{
    public static GameObject[] Farms;
    public GameObject farmids;
    public Sprite hide, show;
    public static SideBarScript instance;
    public TextMeshProUGUI totalFarmInfected;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        StartCoroutine(firstFarm());
    }

    public IEnumerator firstFarm()
    {
        yield return new WaitForSeconds(1.0f);
        List<ushort> infected = ModelHandler.GetInfected();
        int exclamation = Random.Range(1, infected.Count);
        ushort id = infected[exclamation];
        Farms[id - 1].transform.GetChild(1).gameObject.SetActive(true);
        GameContext.sCalledSVA.Add(id);
    }

    // Update is called once per frame
    void Update()
    {
        totalFarmInfected.text = ModelHandler.sInfectedVisibleFarms.Count + "/150";
        if (showing)
        {
            for (int i = 0; i < Farms.Length; i++)
            {
                if(!Farms[i].transform.GetChild(0).GetChild(0).gameObject.activeSelf)
                    Farms[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(showing);
            }
        }
    }

    bool showing = false;
    public void ShowHideFarmIDs()
    {
        Debug.Log("Called");
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
        {
            for (int i = 0; i < Farms.Length; i++)
            {
                Farms[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(showing);
            }
        }
    }

    public void GoToMap()
    {
        SceneManager.LoadScene("WorldMap");
    }

}
