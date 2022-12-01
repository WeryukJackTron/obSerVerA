using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FarmsScript : MonoBehaviour
{
    public GameObject idfarm;
    public SpriteRenderer highlight;
    public float Radius = 22.23117306f;

    public static FarmsScript instance;

    bool options = false;
    public void OnMouseOver()
    {
        highlight.enabled = true;
        idfarm.SetActive(true);
        options = true;
    }

    public void OnMouseExit()
    {
        highlight.enabled = false;
        idfarm.SetActive(false);
        options = false;
    }

    public bool zoned = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        idfarm.GetComponent<TextMeshPro>().text = this.gameObject.transform.parent.name;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.P))
            GameContext.Map.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<SelectScript>().deselectFarm();*/
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit.collider != null && options && hit.collider.transform.parent.gameObject.name == this.transform.parent.gameObject.name)
            {
                InteractionScript.instance.setFarmId(int.Parse(this.transform.parent.name));
                if (this.transform.parent.GetChild(1).gameObject.activeSelf || this.transform.parent.GetChild(3).gameObject.activeSelf)
                {
                    this.transform.parent.GetChild(1).gameObject.SetActive(false);
                    this.transform.parent.GetChild(3).gameObject.SetActive(false);
                    this.transform.parent.GetComponent<SpriteRenderer>().sprite = FarmInitScript.Infected;

                    GameObject myEventSystem = GameObject.Find("EventSystem");
                    myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    StartCoroutine(DelayDeselectFarm());

                    int index = int.Parse(transform.parent.gameObject.name) - 1;
                    GameContext.sFarmsInfo[index].Infected = true;
                    if (GameContext.sFarmsInfo[index].Exclamation)
                        GameContext.sFarmsInfo[index].Exclamation = false;
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
                    /*this.transform.parent.GetChild(4).gameObject.SetActive(true);
                    this.transform.parent.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;
                    zoned = true;
                    nextDayButton.farmid.Add(int.Parse(this.transform.parent.name));
                    GameObject myEventSystem = GameObject.Find("EventSystem");
                    myEventSystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    StartCoroutine(DelayDeselectFarm());*/
                }
                else if (SelectScript.selectedLog)
                {
                    /*GameContext.Log.SetActive(true);
                    GameContext.Log.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = this.transform.parent.name;
                    GameContext.Log.GetComponent<TestScript>().UpdateLog();
                    this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    SelectScript.selectedLog = false;
                    GameContext.Map.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<SelectScript>().deselectFarm();
                    GameContext.Map.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SelectScript>().deselectFarm();
                    GameContext.Map.SetActive(false);*/
                }
            }
            else if(hit.collider == null)
            {
                StartCoroutine(DelayDeselectFarm());
            }
        }
    }

    IEnumerator DelayDeselectFarm()
    {
        yield return new WaitForSeconds(0.01f);
        InteractionScript.instance.setFarmId(0);
        /*Debug.Log("Deselected");
        GameContext.Map.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<SelectScript>().deselectFarm();
        GameContext.Map.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<SelectScript>().deselectFarm();*/
        yield return null;
    }

    IEnumerator delayVetQuarantine(Transform farm)
    {
        yield return new WaitForSeconds(0.01f);
        farm.GetChild(4).gameObject.SetActive(true);

        int index = int.Parse(farm.name) - 1;
        UnityEngine.Debug.Log("Sending vet to: " + (index + 1));
        GameContext.sFarmsInfo[index].Vet = true;
    }

    public void quarantine(int farmID)
    {
        GameObject aux = Instantiate(GameContext.Zone);
        Transform transform = null;
        foreach (GameObject gameObject in SideBarScript.Farms)
        {
            if (int.Parse(gameObject.name) == farmID)
            {
                transform = gameObject.transform;
                break;
            }
        }

        aux.transform.parent = transform;
        aux.transform.SetAsLastSibling();
        aux.transform.localPosition = new Vector3(0, 0, 0);
        aux.transform.localScale = new Vector3(.4f, .4f, 1f);
        GameContext.sFarmsInfo[farmID - 1].Zone = true;

        float radius = 22.23117306f;//I found it using gizmos :D
        Vector2 pos = new Vector2(-100000.0f, -100000.0f);
        foreach(Transform trans in GameContext.Farms)
        {
            if (int.Parse(trans.gameObject.name) != farmID)
                continue;

            pos = trans.position;
            break;
        }
        if (pos.x != -100000.0f)
        {
            for (int i = 0; i < GameContext.Farms.childCount; i++)//Check which of the farms are inside the quarantine radius
            {
                Transform trans = GameContext.Farms.GetChild(i);
                if (int.Parse(trans.gameObject.name) == farmID)
                    continue;

                Transform child = GameContext.Farms.GetChild(i);
                float dist = Vector2.Distance(pos, child.position);
                if (dist <= radius)//Farm i+1 is inside the quarantine radius
                {
                    ushort id = ushort.Parse(trans.gameObject.name);
                    if (!ModelHandler.IsFarmQuarantine(id))
                    {
                        ModelHandler.QuarantineFarm(id);
                        StartCoroutine(delayVetQuarantine(GameContext.Farms.GetChild(i).transform));
                    }
                }
            }
        }

        if (!ModelHandler.IsFarmQuarantine((ushort)farmID))//Also add the farm that was actually specified
            ModelHandler.QuarantineFarm((ushort)farmID);

        //switch (farmID)
        //{
        //    case 1:
        //        GameContext.sQuarantineFarms.Add(1);
        //        GameContext.sQuarantineFarms.Add(2);
        //        GameContext.sQuarantineFarms.Add(9);
        //        break;
        //    case 2:
        //        GameContext.sQuarantineFarms.Add(1);
        //        GameContext.sQuarantineFarms.Add(2);
        //        GameContext.sQuarantineFarms.Add(3);
        //        GameContext.sQuarantineFarms.Add(10);
        //        break;
        //    case 3:
        //        GameContext.sQuarantineFarms.Add(2);
        //        GameContext.sQuarantineFarms.Add(3);
        //        break;
        //    case 4:
        //        GameContext.sQuarantineFarms.Add(4);
        //        GameContext.sQuarantineFarms.Add(11);
        //        break;
        //    case 5:
        //        GameContext.sQuarantineFarms.Add(5);
        //        GameContext.sQuarantineFarms.Add(7);
        //        GameContext.sQuarantineFarms.Add(12);
        //        break;
        //    case 6:
        //        GameContext.sQuarantineFarms.Add(5);
        //        GameContext.sQuarantineFarms.Add(6);
        //        GameContext.sQuarantineFarms.Add(7);
        //        GameContext.sQuarantineFarms.Add(12);
        //        break;
        //    case 7:
        //        GameContext.sQuarantineFarms.Add(6);
        //        GameContext.sQuarantineFarms.Add(7);
        //        GameContext.sQuarantineFarms.Add(8);
        //        GameContext.sQuarantineFarms.Add(18);
        //        break;
        //    case 8:
        //        GameContext.sQuarantineFarms.Add(7);
        //        GameContext.sQuarantineFarms.Add(8);
        //        break;
        //    case 9:
        //        GameContext.sQuarantineFarms.Add(1);
        //        GameContext.sQuarantineFarms.Add(9);
        //        break;
        //    case 10:
        //        GameContext.sQuarantineFarms.Add(2);
        //        GameContext.sQuarantineFarms.Add(10);
        //        GameContext.sQuarantineFarms.Add(14);
        //        break;
        //    case 11:
        //        GameContext.sQuarantineFarms.Add(4);
        //        GameContext.sQuarantineFarms.Add(11);
        //        break;
        //    case 12:
        //        GameContext.sQuarantineFarms.Add(5);
        //        GameContext.sQuarantineFarms.Add(6);
        //        GameContext.sQuarantineFarms.Add(12);
        //        GameContext.sQuarantineFarms.Add(17);
        //        GameContext.sQuarantineFarms.Add(18);
        //        break;
        //    case 13:
        //        GameContext.sQuarantineFarms.Add(13);
        //        break;
        //    case 14:
        //        GameContext.sQuarantineFarms.Add(10);
        //        GameContext.sQuarantineFarms.Add(14);
        //        GameContext.sQuarantineFarms.Add(21);
        //        break;
        //    case 15:
        //        GameContext.sQuarantineFarms.Add(15);
        //        GameContext.sQuarantineFarms.Add(16);
        //        GameContext.sQuarantineFarms.Add(23);
        //        break;
        //    case 16:
        //        GameContext.sQuarantineFarms.Add(15);
        //        GameContext.sQuarantineFarms.Add(16);
        //        break;
        //    case 17:
        //        GameContext.sQuarantineFarms.Add(12);
        //        GameContext.sQuarantineFarms.Add(17);
        //        GameContext.sQuarantineFarms.Add(18);
        //        break;
        //    case 18:
        //        GameContext.sQuarantineFarms.Add(7);
        //        GameContext.sQuarantineFarms.Add(12);
        //        GameContext.sQuarantineFarms.Add(17);
        //        GameContext.sQuarantineFarms.Add(18);
        //        break;
        //    case 19:
        //        GameContext.sQuarantineFarms.Add(19);
        //        break;
        //    case 20:
        //        GameContext.sQuarantineFarms.Add(20);
        //        GameContext.sQuarantineFarms.Add(21);
        //        GameContext.sQuarantineFarms.Add(24);
        //        break;
        //    case 21:
        //        GameContext.sQuarantineFarms.Add(14);
        //        GameContext.sQuarantineFarms.Add(20);
        //        GameContext.sQuarantineFarms.Add(21);
        //        GameContext.sQuarantineFarms.Add(22);
        //        GameContext.sQuarantineFarms.Add(25);
        //        break;
        //    case 22:
        //        GameContext.sQuarantineFarms.Add(21);
        //        GameContext.sQuarantineFarms.Add(22);
        //        GameContext.sQuarantineFarms.Add(25);
        //        break;
        //    case 23:
        //        GameContext.sQuarantineFarms.Add(15);
        //        GameContext.sQuarantineFarms.Add(23);
        //        break;
        //    case 24:
        //        GameContext.sQuarantineFarms.Add(20);
        //        GameContext.sQuarantineFarms.Add(24);
        //        GameContext.sQuarantineFarms.Add(25);
        //        GameContext.sQuarantineFarms.Add(26);
        //        break;
        //    case 25:
        //        GameContext.sQuarantineFarms.Add(21);
        //        GameContext.sQuarantineFarms.Add(22);
        //        GameContext.sQuarantineFarms.Add(24);
        //        GameContext.sQuarantineFarms.Add(25);
        //        GameContext.sQuarantineFarms.Add(27);
        //        break;
        //    case 26:
        //        GameContext.sQuarantineFarms.Add(24);
        //        GameContext.sQuarantineFarms.Add(26);
        //        break;
        //    case 27:
        //        GameContext.sQuarantineFarms.Add(25);
        //        GameContext.sQuarantineFarms.Add(27);
        //        break;
        //    case 28:
        //        GameContext.sQuarantineFarms.Add(28);
        //        break;
        //    case 29:
        //        GameContext.sQuarantineFarms.Add(29);
        //        break;
        //    case 30:
        //        GameContext.sQuarantineFarms.Add(30);
        //        break;
        //    case 31:
        //        GameContext.sQuarantineFarms.Add(31);
        //        break;
        //    case 32:
        //        GameContext.sQuarantineFarms.Add(32);
        //        break;
        //}
    }
    /*
    public void OnDeselect(BaseEventData eventData)
    {
        StartCoroutine(DelayDeselectFarm());
    }

    public void OnSelect(BaseEventData eventData)
    {
        InteractionScript.instance.setFarmId(int.Parse(this.transform.parent.name));
    }*/

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, Radius);
    //}

}
