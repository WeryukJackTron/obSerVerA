using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FarmsScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public GameObject idfarm;
    public GameObject zone;
    public GameObject Log, Map;
    public static FarmsScript instance;

    bool options = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        idfarm.SetActive(true);
        options = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        idfarm.SetActive(false);
        options = false;
    }

    public bool zoned = false;
    // Start is called before the first frame update
    void Start()
    {
        idfarm.GetComponent<TextMeshProUGUI>().text = this.gameObject.transform.parent.name;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
            Map.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<SelectScript>().deselectFarm();
        if (options && Input.GetMouseButtonDown(0))
        {
            if(this.transform.parent.GetChild(1).gameObject.activeSelf || this.transform.parent.GetChild(3).gameObject.activeSelf)
            {
                this.transform.parent.GetChild(1).gameObject.SetActive(false);
                this.transform.parent.GetChild(3).gameObject.SetActive(false);
                this.transform.parent.GetComponent<SpriteRenderer>().sprite = spreadDisease.instance.infected;
                GameObject myEventSystem = GameObject.Find("EventSystem");
                myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                StartCoroutine(DelayDeselectFarm());
            }
            else if (this.transform.parent.GetChild(2).gameObject.activeSelf)
            {
                this.transform.parent.GetChild(2).gameObject.SetActive(false);
                GameObject myEventSystem = GameObject.Find("EventSystem");
                myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                StartCoroutine(DelayDeselectFarm());
            }
            else if (!zoned && SelectScript.selected)
            {
                this.transform.parent.GetChild(4).gameObject.SetActive(true);
                zoned = true;
                nextDayButton.farmid.Add(int.Parse(this.transform.parent.name));
                GameObject myEventSystem = GameObject.Find("EventSystem");
                myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                StartCoroutine(DelayDeselectFarm());
            }
            else if(SelectScript.selectedLog)
            {
                Map.SetActive(false);
                Log.SetActive(true);
                Log.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = this.transform.parent.name;
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                SelectScript.selectedLog = false;
                Debug.Log("FarmsScript");
            }
        }
    }

    IEnumerator DelayDeselectFarm()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Deselected");
        Map.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<SelectScript>().deselectFarm();
        Map.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SelectScript>().deselectFarm();
        yield return null;
    }


    public void quarantine(int i)
    {

        GameObject aux = Instantiate(zone);
        aux.transform.parent = SideBarScript.instance.farms[i - 1].transform;
        aux.transform.SetAsLastSibling();
        aux.transform.localPosition = new Vector3(0, 0, 0);
        aux.transform.localScale = new Vector3(.4f, .4f, 1f);

        switch (i)
        {
            case 1:
                spreadDisease.instance.quarantedFarms.Add(1);
                spreadDisease.instance.quarantedFarms.Add(2);
                spreadDisease.instance.quarantedFarms.Add(9);
                break;
            case 2:
                spreadDisease.instance.quarantedFarms.Add(1);
                spreadDisease.instance.quarantedFarms.Add(2);
                spreadDisease.instance.quarantedFarms.Add(3);
                spreadDisease.instance.quarantedFarms.Add(10);
                break;
            case 3:
                spreadDisease.instance.quarantedFarms.Add(2);
                spreadDisease.instance.quarantedFarms.Add(3);
                break;
            case 4:
                spreadDisease.instance.quarantedFarms.Add(4);
                spreadDisease.instance.quarantedFarms.Add(11);
                break;
            case 5:
                spreadDisease.instance.quarantedFarms.Add(5);
                spreadDisease.instance.quarantedFarms.Add(7);
                spreadDisease.instance.quarantedFarms.Add(12);
                break;
            case 6:
                spreadDisease.instance.quarantedFarms.Add(5);
                spreadDisease.instance.quarantedFarms.Add(6);
                spreadDisease.instance.quarantedFarms.Add(7);
                spreadDisease.instance.quarantedFarms.Add(12);
                break;
            case 7:
                spreadDisease.instance.quarantedFarms.Add(6);
                spreadDisease.instance.quarantedFarms.Add(7);
                spreadDisease.instance.quarantedFarms.Add(8);
                spreadDisease.instance.quarantedFarms.Add(18);
                break;
            case 8:
                spreadDisease.instance.quarantedFarms.Add(7);
                spreadDisease.instance.quarantedFarms.Add(8);
                break;
            case 9:
                spreadDisease.instance.quarantedFarms.Add(1);
                spreadDisease.instance.quarantedFarms.Add(9);
                break;
            case 10:
                spreadDisease.instance.quarantedFarms.Add(2);
                spreadDisease.instance.quarantedFarms.Add(10);
                spreadDisease.instance.quarantedFarms.Add(14);
                break;
            case 11:
                spreadDisease.instance.quarantedFarms.Add(4);
                spreadDisease.instance.quarantedFarms.Add(11);
                break;
            case 12:
                spreadDisease.instance.quarantedFarms.Add(5);
                spreadDisease.instance.quarantedFarms.Add(6);
                spreadDisease.instance.quarantedFarms.Add(12);
                spreadDisease.instance.quarantedFarms.Add(17);
                spreadDisease.instance.quarantedFarms.Add(18);
                break;
            case 13:
                spreadDisease.instance.quarantedFarms.Add(13);
                break;
            case 14:
                spreadDisease.instance.quarantedFarms.Add(10);
                spreadDisease.instance.quarantedFarms.Add(14);
                spreadDisease.instance.quarantedFarms.Add(21);
                break;
            case 15:
                spreadDisease.instance.quarantedFarms.Add(15);
                spreadDisease.instance.quarantedFarms.Add(16);
                spreadDisease.instance.quarantedFarms.Add(23);
                break;
            case 16:
                spreadDisease.instance.quarantedFarms.Add(15);
                spreadDisease.instance.quarantedFarms.Add(16);
                break;
            case 17:
                spreadDisease.instance.quarantedFarms.Add(12);
                spreadDisease.instance.quarantedFarms.Add(17);
                spreadDisease.instance.quarantedFarms.Add(18);
                break;
            case 18:
                spreadDisease.instance.quarantedFarms.Add(7);
                spreadDisease.instance.quarantedFarms.Add(12);
                spreadDisease.instance.quarantedFarms.Add(17);
                spreadDisease.instance.quarantedFarms.Add(18);
                break;
            case 19:
                spreadDisease.instance.quarantedFarms.Add(19);
                break;
            case 20:
                spreadDisease.instance.quarantedFarms.Add(20);
                spreadDisease.instance.quarantedFarms.Add(21);
                spreadDisease.instance.quarantedFarms.Add(24);
                break;
            case 21:
                spreadDisease.instance.quarantedFarms.Add(14);
                spreadDisease.instance.quarantedFarms.Add(20);
                spreadDisease.instance.quarantedFarms.Add(21);
                spreadDisease.instance.quarantedFarms.Add(22);
                spreadDisease.instance.quarantedFarms.Add(25);
                break;
            case 22:
                spreadDisease.instance.quarantedFarms.Add(21);
                spreadDisease.instance.quarantedFarms.Add(22);
                spreadDisease.instance.quarantedFarms.Add(25);
                break;
            case 23:
                spreadDisease.instance.quarantedFarms.Add(15);
                spreadDisease.instance.quarantedFarms.Add(23);
                break;
            case 24:
                spreadDisease.instance.quarantedFarms.Add(20);
                spreadDisease.instance.quarantedFarms.Add(24);
                spreadDisease.instance.quarantedFarms.Add(25);
                spreadDisease.instance.quarantedFarms.Add(26);
                break;
            case 25:
                spreadDisease.instance.quarantedFarms.Add(21);
                spreadDisease.instance.quarantedFarms.Add(22);
                spreadDisease.instance.quarantedFarms.Add(24);
                spreadDisease.instance.quarantedFarms.Add(25);
                spreadDisease.instance.quarantedFarms.Add(27);
                break;
            case 26:
                spreadDisease.instance.quarantedFarms.Add(24);
                spreadDisease.instance.quarantedFarms.Add(26);
                break;
            case 27:
                spreadDisease.instance.quarantedFarms.Add(25);
                spreadDisease.instance.quarantedFarms.Add(27);
                break;
            case 28:
                spreadDisease.instance.quarantedFarms.Add(28);
                break;
            case 29:
                spreadDisease.instance.quarantedFarms.Add(29);
                break;
            case 30:
                spreadDisease.instance.quarantedFarms.Add(30);
                break;
            case 31:
                spreadDisease.instance.quarantedFarms.Add(31);
                break;
            case 32:
                spreadDisease.instance.quarantedFarms.Add(32);
                break;
        }
    }

}
