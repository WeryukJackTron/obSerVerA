using Mono.Data.SqliteClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using System.Threading;
using UnityEngine;
using System.Numerics;

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
    public static List<ushort> sUnderInvestigationFarms = new List<ushort>();
    public static List<ushort> sInfectedVisibleFarms = new List<ushort>();

    /// <summary>A list containing the IDs of all farms that are infected</summary>
    private static List<ushort> sInfectedFarms = new List<ushort>();

    private static volatile bool sModelRunning = false;
    private static ReaderWriterLock sLock = new ReaderWriterLock();
    private static Farm[] sFarms = null;
    private static System.Random sRandomEngine = new System.Random();

    private static volatile bool sLoseFlag = false;
    private static volatile bool sWinFlag = false; 

    public static bool HasWon() { return sWinFlag; }
    public static bool HasLost() { return sLoseFlag; }
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

    public static List<ushort> GetWhoCalled()
    {
        List<ushort> ids = new List<ushort>();
        
        sLock.AcquireReaderLock(-1);
        foreach (ushort farmID in sInfectedFarms)
        {
            if (sQuarantineFarms.Contains(farmID))
                continue;

            Farm farm = sFarms[farmID - 1];
            //if (farm.I == 0)
            //    continue;
            if (GameContext.sCalledSVA.Contains(farm.ID))
                continue;

            const double p = 0.05;
            if (sRandomEngine.NextDouble() <= p)
                ids.Add(farmID);
            //long N = farm.S + farm.I + farm.R;
            //double p = 0.25 * farm.I / N;//P(T+)
            //p = 1.0 - Math.Pow(1.0 - p, N);//P(call_vet)
            //string str = string.Format("Farm {0} chance: {1}", farmID, p);
            //UnityEngine.Debug.Log(str);
            //for (int i = 0; i < farm.I; i++)
            //{
            //    if(sRandomEngine.NextDouble() <= p)
            //    {
            //        ids.Add(farm.ID);
            //        break;
            //    }
            //}
            //double q = 1.0 - p;
            //
            //double result = N * p * Math.Pow(q, N - 1);//Binomial
            //string str = string.Format("Farm {0} has a chance of calling SVA: {1}", farmID, result);
            //UnityEngine.Debug.Log(str);
            //if (sRandomEngine.NextDouble() <= result)
            //    ids.Add(farmID);
        }
        sLock.ReleaseReaderLock();
        
        return ids;
    }

    /**
     * <summary>Sets up the database and runs the model for the specified number of days</summary>
     */
    public static void Init(uint days)
    {
        sFarms = new Farm[GameContext.sNumberOfFarms];
        for (int i = 0; i < sFarms.Length; i++)
            sFarms[i] = new Farm((ushort)(i + 1));

        GameContext.sDataPath = Application.dataPath;
        string args = string.Format("-e \"game.FMD::init('{0}/model.sqlite', beta = {1}, gamma = {2}, n = {3})\"", GameContext.sDataPath, GameContext.sBeta.ToString("G", new System.Globalization.CultureInfo("en-US")), GameContext.sGamma.ToString("G", new System.Globalization.CultureInfo("en-US")), GameContext.sNumberOfFarms);

        sModelRunning = true;
        Thread thread = new Thread(() =>
        {
            RunRScript(args);
            SetInitState();
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
        List<Tuple<ushort, ushort>> ex = new List<Tuple<ushort, ushort>>();
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

            ex.Add(new Tuple<ushort, ushort>(src.ID, dst.ID));
            src.Logs.Add(exchange);
            dst.Logs.Add(exchange);
        }

        foreach (Exchange exchange in events)
        {
            string query = string.Format("INSERT INTO events (event, time, node, dest, n, proportion, 'select', shift) VALUES(\"extTrans\", {0}, {1}, {2}, 0, {3}, 4, 0);", GameContext.sCurrentDay, exchange.From, exchange.To, exchange.Propotion.ToString("G", new System.Globalization.CultureInfo("en-US")));
            ExecuteQuery(query);
        }

        //Publish changes
        sLock.AcquireWriterLock(-1);
        for (int i = 0; i < farms.Count; i++)
            sFarms[i].Logs = farms[i].Logs;
        sLock.ReleaseWriterLock();

        {//Debuging only
            string str = "[";
            if (ex.Count > 0)
            {
                for (int i = 0; i < ex.Count - 1; i++)
                {
                    Tuple<ushort, ushort> aux = ex[i];
                    str += aux.Item1 + "=>" + aux.Item2 + ", ";
                }
                Tuple<ushort, ushort> temp = ex[ex.Count - 1];
                str += temp.Item1 + "=>" + temp.Item2 + "]";
            }
            else
                str += ']';
            UnityEngine.Debug.Log("Exchanges: " + str);
        }
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
                FarmTracker.DayInfected[farm.ID - 1] = (int)GameContext.sCurrentDay;
                //SpreadToFarm(farm.ID);
            }
            else if (farm.I == 0 && sInfectedFarms.Contains(farm.ID))
                sInfectedFarms.Remove(farm.ID);
        }

        sLoseFlag = ((float)sInfectedFarms.Count / GameContext.sNumberOfFarms) >= 0.5f;
        sWinFlag = true;
        foreach(ushort farmID in sInfectedFarms)
        {
            if(!sQuarantineFarms.Contains(farmID))
            {
                sWinFlag = false;
                break;
            }
        }
        sLock.ReleaseWriterLock();

        {//Debuging only
            string str = "[";
            if (sInfectedFarms.Count > 0)
            {
                for (int i = 0; i < sInfectedFarms.Count - 1; i++)
                    str += sInfectedFarms[i] + ", ";
                str += sInfectedFarms[sInfectedFarms.Count - 1] + "]";
            }
            else
                str += ']';
            UnityEngine.Debug.Log("Infected {" + sInfectedFarms.Count + "}: " + str);
        }
    }

    private static void _RunModel(uint days = 1)
    {
        uint until = GameContext.sCurrentDay + days;
        for (uint i = GameContext.sCurrentDay; i < until; i++)
        {
            AdvanceLogs();
            CreateEvents();
            string args = string.Format("-e \"game.FMD::run('{0}/model.sqlite')\"", GameContext.sDataPath);
            RunRScript(args);
            UpdateFarms();

            GameContext.sCurrentDay++;
            if (sInfectedFarms.Count >= 10)
                break;
        }
    }

    private static void RunRScript(string args)
    {
        GameContext.LogToFile("Running: Rscript.exe" + args);
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "Rscript.exe";
        startInfo.RedirectStandardOutput = true;//Although not need it
        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;
        startInfo.Arguments = args;
        Process Rscript = Process.Start(startInfo);
        Rscript.WaitForExit();
        if (Rscript.ExitCode != 0)
        {
            UnityEngine.Debug.Log("Rscript failed");
            GameContext.LogToFile("Fail executing: Rscript.exe" + args);
            //Log Standard output
            GameContext.LogToFile("stdout:");
            while(!Rscript.StandardOutput.EndOfStream)
            {
                string line = string.Format("\t{0}", Rscript.StandardOutput.ReadLine());
                GameContext.LogToFile(line);
            }
            //Log Standard error
            GameContext.LogToFile("stderr:");
            while (!Rscript.StandardError.EndOfStream)
            {
                string line = string.Format("\t{0}", Rscript.StandardError.ReadLine());
                GameContext.LogToFile(line);
            }
        }
    }

    private static void SetInitState()
    {
        string query = "UPDATE U SET I = 0;";
        ExecuteQuery(query);

        int farm1 = sRandomEngine.Next(1, 151);
        int farm2 = sRandomEngine.Next(1, 151);
        int farm3 = sRandomEngine.Next(1, 151);

        while (farm1 == farm2 || farm2 == farm3)
            farm2 = sRandomEngine.Next(1, 151);
        while (farm1 == farm3)
            farm3 = sRandomEngine.Next(1, 151);

        query = string.Format("UPDATE U set I = {0} WHERE node = {1};", sRandomEngine.Next(5, 11), farm1);
        ExecuteQuery(query);
        query = string.Format("UPDATE U set I = {0} WHERE node = {1};", sRandomEngine.Next(5, 11), farm2);
        ExecuteQuery(query);
        query = string.Format("UPDATE U set I = {0} WHERE node = {1};", sRandomEngine.Next(5, 11), farm3);
        ExecuteQuery(query);
    }

    private static void ExecuteQuery(string query, bool selection = false, ActionRef<IDataReader> action = null)
    {
        string connection = string.Format("URI=file:{0}/{1}", GameContext.sDataPath, "model.sqlite");
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

