using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> Class containing static Fields that we want to have global access to.</summary>
public static class GameContext
{
    /// <summary>An array of lists containing log for all farms</summary>
    /// <example>Use <c>farmID - 1</c> to access the correct farm.</example>
    public static List<Exchange>[] sLogs = new List<Exchange>[32];

    /// <summary>A List containing the IDs of all farms that are under quarantine</summary>
    public static List<ushort> sQuarantineFarms = new List<ushort>();

    /// <summary>A list containing the IDs of all farms that are infected</summary>
    public static List<ushort> sInfectedFarms = new List<ushort>();

    /// <summary>Reference to the GameObject named Log</summary>
    public static GameObject Log = null;

    /// <summary>Reference to the GameObject named Map</summary>
    public static GameObject Map = null;

    /// <summary>Reference to the Tranform of the GameObject named Farms (aka the one containing all the farms)</summary>
    public static Transform Farms = null;

    /// <summary> Reference to the Zone Prefab</summary>
    public static GameObject Zone = null;

    /// <summary> Day that we are currently in</summary>
    public static int sCurrentDay;

    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {
        if (SceneManager.GetActiveScene().name != "Map")//Check if scene is the one containing game
            return;

        //Initilize log Lists C# doesn't call default contructor
        for (int i = 0; i < 32; i++)
            sLogs[i] = new List<Exchange>();

        Map = GameObject.Find("Map");
        Farms = GameObject.Find("Farms").transform;
        Log = GameObject.Find("Canvas").transform.GetChild(1).gameObject;//Lol unity (from tranform to child and then gameobject :P)
        Zone = Resources.Load<GameObject>("Zone");

        sCurrentDay = 1;
        ModelHandler.Init();
    }
}

