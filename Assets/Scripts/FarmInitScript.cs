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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
