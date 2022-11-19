using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Threading;
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
    public float Propotion;
    /// <summary>Day on which the exchange occuried.</summary>
    public ushort Day;

    public Exchange(ushort from, ushort to, float propotion, ushort day)
    {
        From = from;
        To = to;
        Propotion = propotion;
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

    private static System.Random sRandomEngine = new System.Random();
    private static double sMinIntensity = 0.0349315068493151;
    private static double sMaxIntensity = 0.0527397260273973;

    public Farm(ushort id, bool initialize = true)
    {
        ID = id;
        Logs = new List<Exchange>();

        if (!initialize)
            return;

        double elapsed = sMaxIntensity - sMinIntensity;
        Intensity = sRandomEngine.NextDouble() * elapsed + sMinIntensity;

        Connections = new List<Tuple<ushort, float>>();
        int Max = 101;
        for (int aux = 0; aux < 10; aux++)
        {
            int to;
            while ((to = sRandomEngine.Next(1, GameContext.sNumberOfFarms + 1)) == id)
                to = sRandomEngine.Next(1, GameContext.sNumberOfFarms + 1);
            int rnd = sRandomEngine.Next(1, Max);
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
    /// <summary>A List containing the IDs of all farms that are under quarantine</summary>
    private static List<ushort> sQuarantineFarms = new List<ushort>();

    /// <summary>A list containing the IDs of all farms that are infected</summary>
    private static List<ushort> sInfectedFarms = new List<ushort>();

    private static volatile bool sModelRunning = false;
    private static ReaderWriterLock sLock = new ReaderWriterLock();
    private static Farm[] sFarms = null;
    private static System.Random sRandomEngine = new System.Random();
    private static string sDataPath = null;

    public static bool IsModelRunning() { return sModelRunning; }

    public static bool IsFarmQuarantine(ushort farmID)
    {
        sLock.AcquireReaderLock(-1);
        bool value = sQuarantineFarms.Contains(farmID);
        sLock.ReleaseReaderLock();
        return value;
    } 

    public static bool IsFarmInfected(ushort farmID)
    {
        sLock.AcquireReaderLock(-1);
        bool value = sInfectedFarms.Contains(farmID);
        sLock.ReleaseReaderLock();
        return value;
    }


    public static void QuarantineFarm(ushort farmID)
    {
        sLock.AcquireWriterLock(-1);
        sQuarantineFarms.Add(farmID);
        sLock.ReleaseWriterLock();
    }

    public static List<Exchange> GetLogs(uint ID)
    {
        int index = (int)ID - 1;
        sLock.AcquireReaderLock(-1);
        List<Exchange> log = new List<Exchange>(sFarms[index].Logs);
        sLock.ReleaseReaderLock();
        return log;
    }

    public static List<ushort> GetInfected()
    {
        sLock.AcquireReaderLock(-1);
        List<ushort> infected = new List<ushort>(sInfectedFarms);
        sLock.ReleaseReaderLock();
        return infected;
    }

    /**
     * <summary>Sets up the database and runs the model for the specified number of days</summary>
     */
    public static void Init(uint days)
    {
        sFarms = new Farm[GameContext.sNumberOfFarms];
        for (int i = 0; i < sFarms.Length; i++)
            sFarms[i] = new Farm((ushort)(i + 1));

        sDataPath = Application.dataPath;
        string args = string.Format("-e \"game.FMD::init('{0}/model.sqlite', beta = {1}, gamma = {2}, n = {3})\"", sDataPath, GameContext.sBeta.ToString("G", new System.Globalization.CultureInfo("en-US")), GameContext.sGamma.ToString("G", new System.Globalization.CultureInfo("en-US")), GameContext.sNumberOfFarms);

        Thread thread = new Thread(() =>
        {
            sModelRunning = true;
            RunRScript(args);
            _RunModel(days);
            sModelRunning = false;
        });
        thread.Start();
    }

    /**
     * <summary>Runs the fake model for the specified numbers of days. </summary>
     * <param name="days">Number of days that we should run the model.</param>
     */
    public static void Run(uint days = 1)
    {
        /*Thread thread = new Thread(() =>
        {
            sModelRunning = true;
            _RunModel(days);
            sModelRunning = false;
        });
        thread.Start();*/
        _RunModel(days);
    }

    /// <summary>Advances all occurrences in the logs by one day</summary>
    private static void AdvanceLogs()
    {
        sLock.AcquireWriterLock(-1);
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
        sLock.ReleaseWriterLock();
    }

    private static void CreateEvents()
    {
        sLock.AcquireReaderLock(-1);
        List<Farm> farms = new List<Farm>(sFarms);
        sLock.ReleaseReaderLock();

        List<Exchange> events = new List<Exchange>();
        foreach (Farm farm in farms)
        {
            sLock.AcquireReaderLock(-1);
            bool isSourceQuarantine = sQuarantineFarms.Contains(farm.ID);
            sLock.ReleaseReaderLock();

            if (isSourceQuarantine)
                continue;

            double p = sRandomEngine.NextDouble();
            if (p > farm.Intensity)//Check the farm should make a transfer today
                continue;

            //Find out to which of it's friends he will send to
            p = sRandomEngine.NextDouble();
            ushort to = 0;
            float lucklyhood = 0.0f;
            bool found = false;
            List<uint> friends = new List<uint>(farm.Connections.Count);
            foreach (Tuple<ushort, float> friend in farm.Connections)
            {
                lucklyhood += friend.Item2;
                
                sLock.AcquireReaderLock(-1);
                bool isDestinationQuarantine = sQuarantineFarms.Contains(friend.Item1);
                sLock.ReleaseReaderLock();

                if (isDestinationQuarantine)
                    continue;

                if (lucklyhood <= p && !found)
                {
                    to = friend.Item1;
                    found = true;
                }

                friends.Add(friend.Item1);
            }

            if (to == 0)//Selected farm means he/she could to anybody so we choose one randomly that is not the same farm or one of it's friends
            {
                to = (ushort)sRandomEngine.Next(1, GameContext.sNumberOfFarms + 1);
                sLock.AcquireReaderLock(-1);
                while (to == farm.ID || friends.Contains(to) || sQuarantineFarms.Contains(to))
                    to = (ushort)sRandomEngine.Next(1, GameContext.sNumberOfFarms + 1);
                sLock.ReleaseReaderLock();
            }

            float propotion = sRandomEngine.Next(1, 11) / 100.0f;
            Exchange exchange = new Exchange(farm.ID, to, propotion, (ushort)GameContext.sCurrentDay);
            events.Add(exchange);

            Farm src = farms[(int)farm.ID - 1];
            Farm dst = farms[(int)to - 1];

            src.Logs.Add(exchange);
            dst.Logs.Add(exchange);
        }

        foreach (Exchange exchange in events)
        {
            string query = string.Format("INSERT INTO events (event, time, node, dest, n, proportion, 'select', shift) VALUES(\"extTrans\", {0}, {1}, {2}, 0, {3}, 4, 0);", GameContext.sCurrentDay, exchange.From, exchange.To, exchange.Propotion);
            ExecuteQuery(query);
        }

        //Publish changes
        sLock.AcquireWriterLock(-1);
        for (int i = 0; i < farms.Count; i++)
            sFarms[i].Logs = farms[i].Logs;
        sLock.ReleaseWriterLock();
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
        sLock.AcquireWriterLock(-1);
        for (int i = 0; i < farms.Count; i++)
        {
            Farm farm = farms[i];
            sFarms[i].ID = farm.ID;
            sFarms[i].S = farm.S;
            sFarms[i].I = farm.I;
            sFarms[i].R = farm.R;

            if (farm.I > 0 && !sInfectedFarms.Contains(farm.ID))
            {
                sInfectedFarms.Add(farm.ID);
                UnityEngine.Debug.Log(farm.ID);
                //SpreadToFarm(farm.ID);
            }
            else if (farm.I == 0 && sInfectedFarms.Contains(farm.ID))
                sInfectedFarms.Remove(farm.ID);
        }
        sLock.ReleaseWriterLock();
    }

    private static void _RunModel(uint days = 1)
    {
        uint until = GameContext.sCurrentDay + days;
        for (uint i = GameContext.sCurrentDay; i < until; i++)
        {
            AdvanceLogs();
            CreateEvents();
            string args = string.Format("-e \"game.FMD::run('{0}/model.sqlite')\"", sDataPath);
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
        string connection = string.Format("URI=file:{0}/{1}", sDataPath, "model.sqlite");
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

    //private static void SpreadToFarm(int farm) { sFarmsObject.GetChild(farm - 1).GetChild(3).gameObject.SetActive(true); }

}

