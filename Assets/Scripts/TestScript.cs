using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScript : MonoBehaviour
{

   /* public Tilemap pepe;
    public Grid paco;*/
    public GameObject grid_log;
    public GameObject confirm, reset;
    public static TestScript instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    Vector3Int aux = new Vector3Int(0, 0, 1);
    void Update()
    {
       /* Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        Vector3Int cellpos = paco.LocalToCell(pos);
        if(aux == new Vector3Int(0, 0, 1))
            aux = cellpos;
        if (cellpos != aux)
        {
            //pepe.SetTileFlags(aux, TileFlags.None);
            pepe.SetColor(aux, Color.white);
            aux = new Vector3Int(0, 0, 1);
        }
        else
        {
            //pepe.SetTileFlags(cellpos, TileFlags.None);
            pepe.SetColor(cellpos, Color.yellow);
        }
        if (Input.GetMouseButtonUp(0))
        {
            pepe.SetColor(cellpos, Color.red);
            pepe.SetTileFlags(cellpos, TileFlags.LockColor);
        }*/
    }

    public void ShowHideCheckBoxes()
    {
        confirm.SetActive(!confirm.activeSelf);
        reset.SetActive(!reset.activeSelf);
        for(int i = 0; i < grid_log.transform.childCount; i++)
        {
            grid_log.transform.GetChild(i).GetChild(1).gameObject.SetActive(!grid_log.transform.GetChild(i).GetChild(1).gameObject.activeSelf);
        }
    }
}
