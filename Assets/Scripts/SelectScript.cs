using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectScript : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public int tipo;
    public static bool selected = false;
    public static bool selectedLog = false;
    public static SelectScript instance;
    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(Deselecting());
    }

    IEnumerator Deselecting()
    {
        yield return new WaitForSeconds(0.01f);
        if (tipo == 0)
            selected = false;
        else
            selectedLog = false;
        print("Hello");
        yield return null;
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (selectedFarm)
        {
            GameObject farm = SideBarScript.instance.farms[idFarm - 1];
            if (tipo == 0 && !farm.transform.GetChild(4).gameObject.activeSelf)
            {
                farm.transform.GetChild(4).gameObject.SetActive(true);
                farm.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;
                farm.transform.GetChild(0).GetComponent<FarmsScript>().zoned = true;
                nextDayButton.farmid.Add(idFarm);
            }
            else if(tipo == 1)
            {
                StartCoroutine(DeselectingFarm(farm));
            }
            deselectFarm();
        }
        else
        {
            if (tipo == 0)
                selected = true;
            else
                selectedLog = true;
        }
    }

    IEnumerator DeselectingFarm(GameObject farm)
    {
        yield return new WaitForSeconds(0.01f);
        GameContext.Map.SetActive(false);
        GameContext.Log.SetActive(true);
        GameContext.Log.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = farm.name;
        GameContext.Log.GetComponent<TestScript>().UpdateLog();
        Debug.Log("SelectScript");
        yield return null;
    }

    public bool selectedFarm = false;
    int idFarm = -1;
    public void farmSelected(int id)
    {
        selectedFarm = true;
        idFarm = id;
    }

    public void deselectFarm()
    {
        selectedFarm = false;
        idFarm = -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
