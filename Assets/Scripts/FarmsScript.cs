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
                GameObject aux = Instantiate(zone);
                aux.transform.parent = this.transform.parent;
                aux.transform.SetAsLastSibling();
                aux.transform.localPosition = new Vector3(0, 0, 0);
                aux.transform.localScale = new Vector3(.4f, .4f, 1f);
                zoned = true;
                GameObject myEventSystem = GameObject.Find("EventSystem");
                myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                StartCoroutine(DelayDeselectFarm());
                Debug.Log("First");
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


}
