using System;
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

    //Vector3Int aux = new Vector3Int(0, 0, 1);
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

    public void UpdateLog()
    {
        uint id = uint.Parse(this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        List<Exchange> log = ModelHandler.GetLogs(id);
        int j = 9;
        if (log.Count < 9)
        {
            for (int i = log.Count; i < 9; i++)
            {
                grid_log.transform.GetChild(i).gameObject.SetActive(false);
            }
            j = log.Count;
        }

        Array.Fill(LogScript.checkboxes, 0);//Since we just opened refilling all checkboxes with 0.

        for (int i = 0; i < j; i++)
        {
            grid_log.transform.GetChild(i).gameObject.SetActive(true);
            Exchange exchange = log[i];
            string line = string.Format("Farm: {0} send to farm: {1} [{2} days ago]", exchange.From, exchange.To, GameContext.sCurrentDay - exchange.Day);
            grid_log.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = line;

            if (id != exchange.From)
            {
                LogScript.checkboxes[i] = exchange.From;
            }
            else
            {
                LogScript.checkboxes[i] = exchange.To;
            }
        }
    }

    public void ShowHideCheckBoxes()
    {
        confirm.SetActive(!confirm.activeSelf);
        reset.SetActive(!reset.activeSelf);

        foreach(Transform transform in grid_log.transform)
        {
            if (!transform.gameObject.activeSelf)
                break;

            transform.GetChild(1).gameObject.SetActive(!transform.GetChild(1).gameObject.activeSelf);
        }

        //For some reason doesn't work Unity complains that Child is out of bounds when use GetChild(1)
        // Just like resetCheckboxes in LogScript
        //for(int i = 0; i < grid_log.transform.childCount; i++)
        //{
        //    grid_log.transform.GetChild(i).GetChild(1).gameObject.SetActive(!grid_log.transform.GetChild(i).GetChild(1).gameObject.activeSelf);
        //}
    }
}
