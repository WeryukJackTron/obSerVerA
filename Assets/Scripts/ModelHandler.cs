using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.IO;
using UnityEngine;

/// <summary> Type for callback function that can be using for reading each row form the database </summary>
delegate void ActionRef<T>(ref T arg);

/**
 * <summary>Struct <c>Exchange</c>, data structure describing an Exchange between farms.</summary>
 * <see cref="Farm"/>
 */
public struct Exchange
{
    /// <summary>ID of the farm where the cattle originated from.</summary>
    public ushort From;
    /// <summary>ID of the farm where the cattle was send.</summary>
    public ushort To;
    /// <summary>Number of cattle that was transfered on this exchange.</summary>
    public ushort N;//TODO(Vasilis): Maybe change to propotions
    /// <summary>Day on which the exchange occuried.</summary>
    public ushort Day;

    public Exchange(ushort from, ushort to, ushort n, ushort day)
    {
        From = from;
        To = to;
        N = n;
        Day = day;
    }
}

/**
 * <summary>Struct <c>Farm</c>, data structure containing information about a Farm.</summary>
 */
public class Farm
{
    /// <summary>Farm's ID. Use ID-1 to find the index of the Farm on the list.</summary>
    public ushort ID;
    /// <summary>Number of cattle that neither <b>Infected</b> nor <b>Recovered</b>.</summary>
    public ushort S;
    /// <summary>Number of cattle that are <b>Infected</b>.</summary>
    public ushort I;
    /// <summary>Number of cattle that have <b>Recovered</b> from the infection.</summary>
    public ushort R;
    /// <summary>The possibility to send animals each day</summary>
    public double Intensity;

    /**
     * <summary>List of exchanges with this farm (aka Farm's log).</summary>
     * <see cref="Exchange"/>
     */
    public List<Exchange> Logs;

    /// <summary> List of friends that this farms has.
    /// Each element is basically a pair that contains a Farm's ID and "weight" value.
    /// If a value with farm's ID 0 (usually the last one) means that the "weight" represents the posibility to send anywhere else.
    /// </summary>
    public List<Tuple<ushort, float>> Connections;

    private static double sMinIntensity = 0.0349315068493151;
    private static double sMaxIntensity = 0.0527397260273974;

    public Farm(ushort id, bool initialize = true)
    {
        ID = id;
        Logs = new List<Exchange>();

        if (!initialize)
            return;

        double elapsed = sMaxIntensity - sMinIntensity;
        Intensity = UnityEngine.Random.value * elapsed + sMinIntensity;

        Connections = new List<Tuple<ushort, float>>();
        int Max = 101;
        for (int aux = 0; aux < 10; aux++)
        {
            int to;
            while ((to = UnityEngine.Random.Range(1, GameContext.sNumberOfFarms + 1)) == id)
                to = UnityEngine.Random.Range(1, GameContext.sNumberOfFarms + 1);
            int rnd = UnityEngine.Random.Range(1, Max);
            Max -= rnd;

            float weight = (float)rnd / 100;
            Connections.Add(new Tuple<ushort, float>((ushort)to, weight));
            if (Max <= 1)
                break;
        }

        if (Max > 1)
        {
            float weight = (float)Max / 100;
            Connections.Add(new Tuple<ushort, float>(0, weight));
        }
    }

    public Farm(ushort id, ushort s, ushort i, ushort r)
        : this(id, false)
    {
        S = s;
        I = i;
        R = r;
    }
}

public static class ModelHandler
{
    private static Transform sFarmsObject;
    private static Farm[] sFarms = null;


    public static List<Exchange> GetLogs(uint ID)
    {
        //TODO(Vasilis): Maybe add logs
        int index = (int)ID - 1;
        List<Exchange> log = new List<Exchange>(sFarms[index].Logs);
        return log;
    }


    /**
     * <summary>Sets up the CSV files</summary>
     */
    public static void Init(uint days)
    {
        //Find Game object that containing all Farms and cache it for later use
        sFarmsObject = GameObject.Find("Farms").transform;

        //TODO(Vasilis): Maybe add Thread
        sFarms = new Farm[GameContext.sNumberOfFarms];
        for (int i = 0; i < sFarms.Length; i++)
            sFarms[i] = new Farm((ushort)(i + 1));

        string args = string.Format("-e \"game.FMD::init('{0}/model.sqlite', beta = {1}, n = {2})\"", Application.dataPath, GameContext.sBeta, GameContext.sNumberOfFarms);
        RunRScript(args);

        _RunModel(days);

        if (GameContext.sInfectedFarms.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, GameContext.sInfectedFarms.Count);
            sFarmsObject.GetChild(GameContext.sInfectedFarms[index] - 1).GetChild(1).gameObject.SetActive(true);
        }
        else
            UnityEngine.Debug.Log("No infected farm");
    }

    /**
     * <summary>Runs the fake model for the specified numbers of days. </summary>
     * <param name="days">Number of days that we should run the model.</param>
     */
    public static void Run(uint days = 1)
    {
        //TODO(Vasilis): Maybe add Thread
        _RunModel(days);
    }

    /// <summary>Advances all occurrences in the logs by one day</summary>
    private static void AdvanceLogs()
    {
        //TODO(Vasilis): Maybe add logs
        for (int i = 0; i < sFarms.Length; i++)
        {
            List<int> bin = new List<int>();
            for (int j = 0; j < sFarms[i].Logs.Count; j++)
            {
                if (GameContext.sCurrentDay - sFarms[i].Logs[j].Day == 14)
                    bin.Add(j);

                if (bin.Count == 0)
                    continue;
            }

            if (bin.Count == 0)
                continue;

            //Remove old logs
            for (int j = bin.Count - 1; j >= 0; j--)
                sFarms[i].Logs.RemoveAt(j);
        }
    }

    private static void CreateEvents()
    {
        //TODO(Vasilis): Maybe add locks
        List<Farm> farms = new List<Farm>(sFarms);

        List<Exchange> events = new List<Exchange>();
        foreach (Farm farm in farms)
        {
            double p = UnityEngine.Random.value;
            if (p > farm.Intensity)//Check the farm should make a transfer today
                continue;

            //Find out to which of it's friends he will send to
            p = UnityEngine.Random.value;
            ushort to = 0;
            float lucklyhood = 0.0f;
            bool found = false;
            List<uint> friends = new List<uint>(farm.Connections.Count);
            foreach (Tuple<ushort, float> friend in farm.Connections)
            {
                lucklyhood += friend.Item2;
                if (lucklyhood <= p && !found)
                {
                    to = friend.Item1;
                    found = true;
                }

                friends.Add(friend.Item1);
            }

            if (to == 0)//Selected farm means he/she could to anybody so we choose one randomly that is not the same farm or one of it's friends
            {
                to = (ushort)UnityEngine.Random.Range(1, GameContext.sNumberOfFarms + 1);
                while (to == farm.ID || friends.Contains(to))
                    to = (ushort)UnityEngine.Random.Range(1, GameContext.sNumberOfFarms + 1);
            }

            Exchange exchange = new Exchange(farm.ID, to, 1, (ushort)GameContext.sCurrentDay);
            events.Add(exchange);

            Farm src = farms[(int)farm.ID - 1];
            Farm dst = farms[(int)to - 1];

            src.Logs.Add(exchange);
            dst.Logs.Add(exchange);
        }

        foreach (Exchange exchange in events)
        {
            string query = string.Format("INSERT INTO events (event, time, node, dest, n, proportion, 'select', shift) VALUES(\"extTrans\", {0}, {1}, {2}, {3}, 0.0, 4, 0);", GameContext.sCurrentDay, exchange.From, exchange.To, 1);
            ExecuteQuery(query);
        }

        //Publish changes
        //TODO(Vasilis): Maybe add locks
        for (int i = 0; i < farms.Count; i++)
            sFarms[i].Logs = farms[i].Logs;
    }

    private static void UpdateFarms()
    {
        List<Farm> farms = new List<Farm>();
        string query = string.Format("SELECT node, S, I, R FROM U WHERE time = {0};", GameContext.sCurrentDay);
        ExecuteQuery(query, true, (ref IDataReader reader) =>
        {
            ushort ID = (ushort)reader.GetInt16(0);
            ushort S = (ushort)reader.GetInt16(1);
            ushort I = (ushort)reader.GetInt16(2);
            ushort R = (ushort)reader.GetInt16(3);

            farms.Add(new Farm(ID, S, I, R));
        });

        //Publish results
        //TODO(Vasilis): Maybe add locks
        for (int i = 0; i < farms.Count; i++)
        {
            Farm farm = farms[i];
            sFarms[i].ID = farm.ID;
            sFarms[i].S = farm.S;
            sFarms[i].I = farm.I;
            sFarms[i].R = farm.R;

            if (farm.I > 0 && !GameContext.sInfectedFarms.Contains(farm.ID))
            {
                GameContext.sInfectedFarms.Add(farm.ID);
                //SpreadToFarm(farm.ID);
            }
            else if (farm.I == 0 && GameContext.sInfectedFarms.Contains(farm.ID))
                GameContext.sInfectedFarms.Remove(farm.ID);
        }
    }

    private static void _RunModel(uint days = 1)
    {
        uint until = GameContext.sCurrentDay + days;
        for (uint i = GameContext.sCurrentDay; i < until; i++)
        {
            AdvanceLogs();
            CreateEvents();
            string args = string.Format("-e \"game.FMD::run('{0}/model.sqlite')\"", Application.dataPath);
            RunRScript(args);
            UpdateFarms();

            GameContext.sCurrentDay++;
        }
    }

    private static void RunRScript(string args)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "Rscript.exe";
        startInfo.RedirectStandardOutput = true;//Although not need it
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.Arguments = args;
        Process Rscript = Process.Start(startInfo);
        Rscript.WaitForExit();

        if (Rscript.ExitCode != 0)
            UnityEngine.Debug.Log("Rscript failed");
    }

    private static void ExecuteQuery(string query, bool selection = false, ActionRef<IDataReader> action = null)
    {
        string connection = string.Format("URI=file:{0}/{1}", Application.dataPath, "model.sqlite");
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        using (var cmd = dbcon.CreateCommand())
        {
            cmd.CommandText = query;
            if (!selection)
                cmd.ExecuteNonQuery();
            else
            {
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                    action(ref reader);
            }
        }
        dbcon.Close();
    }

    private static void SpreadToFarm(int farm) { sFarmsObject.GetChild(farm - 1).GetChild(3).gameObject.SetActive(true); }

}

