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
        int id = int.Parse(this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        List<Exchange> log = spreadDisease.instance.logs[id - 1];
        int j = 9;
        if (log.Count < 9)
        {
            for(int i = log.Count; i < 9; i++)
            {
                grid_log.transform.GetChild(i).gameObject.SetActive(false);
            }
            j = log.Count;
        }
        for (int i = 0; i < j; i++)
        {
            grid_log.transform.GetChild(i).gameObject.SetActive(true);
            Exchange exchange = log[i];
            string line = string.Format("Farm: {0} send to farm: {1}, {2} days ago.", exchange.From, exchange.To, exchange.DaysBefore);
            grid_log.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = line;
        }
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
