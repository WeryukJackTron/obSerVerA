using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FarmProperty
{
    public ushort ID;
    public float x;
    public float y;

    public FarmProperty(ushort id, float x, float y)
    {
        ID = id;
        this.x = x;
        this.y = y;
    }
}

public class FarmInitScript : MonoBehaviour
{
    public static Sprite Infected = null;
    public static int sNextIndex;
    public static FarmProperty[] sProps;

    // Start is called before the first frame update
    void Start()
    {
        if (Infected == null)
            Infected = Resources.Load<Sprite>("Barn_Infected");

        FarmProperty prop = sProps[sNextIndex];
        SideBarScript.Farms[sNextIndex] = gameObject;
        gameObject.name = "" + prop.ID;
        float x = prop.x / 25000.0f * (1920.0f - 250.0f) - 960.0f;
        float y = prop.y / 25000.0f * 1000.0f - 540.0f;
        x = Mathf.Max(x, -937.0f);//, 706.0f);
        y = Mathf.Max(y, -470.0f);//, 500.0f);
        
        transform.localPosition = new Vector3(x, y, 0);
        sNextIndex++;

        FarmInfo info = GameContext.sFarmsInfo[prop.ID - 1];
        if (info.Exclamation)
            transform.GetChild(1).gameObject.SetActive(true);

        if (info.Infected)
            transform.GetComponent<SpriteRenderer>().sprite = Infected;

        if (info.Vet)
            transform.GetChild(4).gameObject.SetActive(true);

        if (info.Zone)
        {
            GameObject aux = Instantiate(GameContext.Zone);
            aux.transform.parent = this.transform;
            aux.transform.SetAsLastSibling();
            aux.transform.localPosition = new Vector3(0, 0, 0);
            aux.transform.localScale = new Vector3(.4f, .4f, 1f);
            StartCoroutine(delayCheckZone(prop.ID));
        }
    }

    IEnumerator delayCheckZone(int farmID)
    {
        yield return new WaitForSeconds(0.01f);
        float radius = 22.23117306f;//I found it using gizmos :D
        Vector2 pos = new Vector2(-100000.0f, -100000.0f);
        foreach (Transform trans in GameContext.Farms)
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
                    if (ModelHandler.IsFarmInfected(id))
                    {
                        trans.GetComponent<SpriteRenderer>().sprite = Infected;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
