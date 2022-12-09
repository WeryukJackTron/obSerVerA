using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{

    /* public Tilemap pepe;
     public Grid paco;*/
    public GameObject grid_log;
    public GameObject confirm, reset;
    public Sprite InfectedFarmLog, FarmLog;
    public GameObject progressbar;
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
            string line = string.Format("{0} days ago", GameContext.sCurrentDay - exchange.Day);
            grid_log.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = line;
            if (exchange.From.ToString() == this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text)
            {
                grid_log.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = exchange.From.ToString();
                grid_log.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = exchange.To.ToString();
                grid_log.transform.GetChild(i).GetChild(2).localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                grid_log.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = exchange.To.ToString();
                grid_log.transform.GetChild(i).GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = exchange.From.ToString();
                grid_log.transform.GetChild(i).GetChild(2).localEulerAngles = new Vector3(0, 180, 0);
            }

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
        if(!ModelHandler.sInfectedVisibleFarms.Contains((ushort)int.Parse(LogScript.instance.farmidLog.text)))
            LogScript.instance.checkboxSelf.SetActive(!LogScript.instance.checkboxSelf.activeSelf);

        foreach(Transform transform in grid_log.transform)
        {
            if (!transform.gameObject.activeSelf)
                break;

            transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeSelf);
        }

        //For some reason doesn't work Unity complains that Child is out of bounds when use GetChild(1)
        // Just like resetCheckboxes in LogScript
        //for(int i = 0; i < grid_log.transform.childCount; i++)
        //{
        //    grid_log.transform.GetChild(i).GetChild(1).gameObject.SetActive(!grid_log.transform.GetChild(i).GetChild(1).gameObject.activeSelf);
        //}
    }

    public void checkFarm()
    {
        ushort farmid = (ushort)int.Parse(gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
        if (ModelHandler.sInfectedVisibleFarms.Contains(farmid))
        {
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = InfectedFarmLog;
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<Image>().sprite = FarmLog;
        }
        if (ModelHandler.sUnderInvestigationFarms.Contains(farmid))
        {
            progressbar.SetActive(true);
        }
        else
        {
            progressbar.SetActive(false);
        }
    }

    public void changeFarmLog(int i)
    {
        int id = LogScript.checkboxes[i];
        dragCamera.instance.JumpTo(id);
        this.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = id.ToString();
        UpdateLog();
        checkFarm();
    }
}
