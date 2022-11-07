using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * <summary>Struct <c>Exchange</c>, data structure describing an Exchange between farms.</summary>
 */
public struct Exchange
{
    /// <summary>ID of the farm where the cattle originated from.</summary>
    public ushort From;
    /// <summary>ID of the farm where the cattle was send.</summary>
    public ushort To;
    /// <summary>Days before the current that the exchange occuried.</summary>
    public uint DaysBefore;
}

public static class ModelHandler
{
    private static Transform sFarmsObject;
    
    /**
     * <summary>Sets up the CSV files</summary>
     */
    public static void Init()
    {
        //Write csv for 30 days
        for (int day = 1; day <= 30; day++)
        {
            string filepath = string.Format("{0}\\Scripts\\EventLogs\\day_{1}.csv", Application.dataPath, day);
            List<string> lines = new List<string>();
            for(int from = 1; from <= 32; from++)
            {
                int to;
                while ((to = UnityEngine.Random.Range(1, 33)) == from);//Continue changing values until we get another farm
                int days = UnityEngine.Random.Range(1, 15);

                string line = string.Format("{0}, {1}, {2}", from, to, days);
                lines.Add(line);
            }

            File.WriteAllLines(filepath, lines);
        }

        //Find Game object that containing all Farms and cache it for later use
        sFarmsObject = GameObject.Find("Farms").transform;

        //Make one random farm infected on start
        ushort rand = (ushort)UnityEngine.Random.Range(0, 32);
        sFarmsObject.GetChild(rand).GetChild(1).gameObject.SetActive(true);
        GameContext.sInfectedFarms.Add((ushort)(rand + 1));
    }

    /**
     * <summary>Runs the fake model for the specified numbers of days. </summary>
     * <param name="days">Number of days that we should run the model.</param>
     */
    public static void Run(int days = 1)
    {
        int until = GameContext.sCurrentDay + 1;
        for (int day = GameContext.sCurrentDay; day < until; day++)
        {
            string filepath = string.Format("{0}\\Scripts\\EventLogs\\day_{1}.csv", Application.dataPath, day);
            AdvanceLogs();

            string[] lines = File.ReadAllLines(filepath);
            List<ushort> infectedFarms = new List<ushort>(GameContext.sInfectedFarms);
            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                ushort from = ushort.Parse(values[0]);
                ushort to = ushort.Parse(values[1]);
                ushort numOfDays = ushort.Parse(values[2]);

                if (GameContext.sQuarantineFarms.Contains(from) || GameContext.sQuarantineFarms.Contains(to))
                    continue;

                if(GameContext.sInfectedFarms.Contains(from))
                {
                    infectedFarms.Add(to);
                    SpreadToFarm(to);
                }

                Exchange exchange = new Exchange();
                exchange.From = (ushort)from;
                exchange.To = (ushort)to;
                exchange.DaysBefore = numOfDays;

                GameContext.sLogs[from - 1].Add(exchange);
                GameContext.sLogs[to - 1].Add(exchange); 
            }

            GameContext.sInfectedFarms = new List<ushort>(infectedFarms);
            GameContext.sCurrentDay++;
        }
    }

    /// <summary>Advances all occurrences in the logs by one day</summary>
    private static void AdvanceLogs()
    {
        foreach(List<Exchange> log in GameContext.sLogs)
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

            //Remove old logs
            for (int i = bin.Count - 1; i >= 0; i--)
                log.RemoveAt(i);
        }
    }

    private static void SpreadToFarm(int farm) { sFarmsObject.GetChild(farm - 1).GetChild(3).gameObject.SetActive(true); }

}

