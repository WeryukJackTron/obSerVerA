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
    public static int sNextIndex;
    public static FarmProperty[] sProps;

    // Start is called before the first frame update
    void Start()
    {
        FarmProperty prop = sProps[sNextIndex];
        SideBarScript.Farms[sNextIndex] = gameObject;
        gameObject.name = "" + prop.ID;
        float x = prop.x / 25000.0f * 1920.0f - 960.0f;
        float y = prop.y / 25000.0f * 1080.0f - 540.0f;

        transform.localPosition = new Vector3(x, y, 0);
        sNextIndex++;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
