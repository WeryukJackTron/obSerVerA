using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Exchange
{
    public short From;
    public short To;
    public ushort DaysBefore;
}

public class spreadDisease : MonoBehaviour
{
    // List of all farms
    public List<Transform> farmsList = new List<Transform>();
    // Tracker for current day
    public int dayNumber = 1;
    // List of all farms that are infected - the current day's infected farms
    //public List<ushort>[] prevInfectedFarms = new List<ushort>[14];
    // List of farms that are infected on the current day
    public List<ushort> currInfectedFarms = new List<ushort>();
    public List<ushort> quarantedFarms = new List<ushort>();

    public List<Exchange>[] logs = new List<Exchange>[32];

    public Sprite infected;
    public static spreadDisease instance;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 32; i++)
            logs[i] = new List<Exchange>();

        initialInfection();
        instance = this;
        //Debug.Log("Previously infected farms:");
        //Debug.Log(string.Join(", ", prevInfectedFarms));
        //Debug.Log("Currently infected farms:");
        //Debug.Log(string.Join(", ", currInfectedFarms));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void listChildren()
    {
        foreach (Transform child in transform)
        {
            Debug.Log(child);
        }
    }

    // Start disease at a random farm
    void initialInfection()
    {
        foreach (Transform child in transform)
        {
            farmsList.Add(child);
        }

        ushort rand = (ushort)UnityEngine.Random.Range(0, 32);

        farmsList[rand].GetChild(1).gameObject.SetActive(true);
        currInfectedFarms.Add((ushort)(rand + 1));
  
        //readCurrentDayEventLog();
    }

    public void readCurrentDayEventLog()
    {
        string filename = (Application.dataPath + "\\Scripts\\EventLogs\\day_" + (dayNumber++) + ".csv");
        string[] Lines = System.IO.File.ReadAllLines(filename);

        UpdateLogs();

        // Find the event log lines corresonding to all farms in currInfectedFarms
        List<ushort> locallogs = new List<ushort>(currInfectedFarms);
        for (int i = 0; i < Lines.Length - 1; i++)
        {

            string data = Lines[i];
            string[] values = data.Split(',');

            ushort from = ushort.Parse(values[0]);
            ushort to = ushort.Parse(values[1]);
            ushort days = ushort.Parse(values[2]);

            if (quarantedFarms.Contains(to) || quarantedFarms.Contains(from))
                continue;

            if(currInfectedFarms.Contains(from))
            {
                locallogs.Add(to);
                spreadFromContact(to);
            }

            Exchange exchange = new Exchange();
            exchange.From = (short)from;
            exchange.To = (short)to;
            exchange.DaysBefore = days;

            logs[from - 1].Add(exchange);
            logs[to - 1].Add(exchange);

            //for (int j = 0; j <= currInfectedFarms.Count - 1; j++)
            //{
            //    if ( int.Parse(Lines[i].ToString().Split(",")[0]) == currInfectedFarms[j])
            //    {
            //        int spreadToFarmX = int.Parse(Lines[i].ToString().Split(",")[2]);
            //        spreadFromContact(spreadToFarmX);
            //        Debug.Log(int.Parse(Lines[i].ToString().Split(",")[2]));
            //
            //    }
            //}
        }

        currInfectedFarms = new List<ushort>(locallogs);
    }

    private void UpdateLogs()
    {
        foreach (List<Exchange> log in logs)
        {
            List<int> bin = new List<int>();
            for (int i = 0; i < log.Count; i++)
            {
                if (log[i].DaysBefore == 14)
                    bin.Add(i);
                else
                {
                    Exchange exchange = log[i];
                    exchange.DaysBefore++;
                    log[i] = exchange;
                }
            }

            if (bin.Count == 0)
                continue;

            for (int i = bin.Count - 1; i >= 0; i--)
                log.RemoveAt(i);
        }
    }

    void spreadFromContact(int farm)
    {
        foreach (Transform item in farmsList)
        {
            if (item.name == farm.ToString())
            {
                //item.GetComponent<SpriteRenderer>().sprite = infected; 
                item.GetChild(3).gameObject.SetActive(true);
            }
        }
        
    }
}


