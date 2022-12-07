using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> A collection of flags for what we need to show each farm every time we open a region</summary>
public struct FarmInfo
{
    public bool Exclamation;
    public bool Infected;
    public bool Vet;
    public bool Zone;

    public FarmInfo(bool exclamation, bool infected, bool vet, bool zone)
    {
        Exclamation = exclamation;
        Infected = infected;
        Vet = vet;
        Zone = zone;
    }
}

/// <summary> Class containing static Fields that we want to have global access to.</summary>
public static class GameContext
{
    /// <summary>Reference to the GameObject named Log</summary>
    public static GameObject Log = null;

    /// <summary>Reference to the GameObject named Map</summary>
    public static GameObject Map = null;

    /// <summary>Reference to the Tranform of the GameObject named Farms (aka the one containing all the farms)</summary>
    public static Transform Farms = null;

    /// <summary> Reference to the Zone Prefab</summary>
    public static GameObject Zone = null;

    /// <summary> Day that we are currently in</summary>
    public static uint sCurrentDay;

    /// <summary> The number of Farms that the game has (and the model should use)</summary>
    public static int sNumberOfFarms = 150;

    /// <summary> The beta value that the model will use</summary>
    public static float sBeta = 0.1f;

    /// <summary> The gamma value that the model will use</summary>
    public static float sGamma = 0.077f;

    public static string sDataPath = null;


    public static FarmInfo[] sFarmsInfo = new FarmInfo[sNumberOfFarms];

    public static int maxVets = 15;
    public static int busyVets = 0;
    public static int sVetsSend;
    public static int sZonePlaced;

    public static void Init()
    {
        Map = GameObject.Find("Map");
        Farms = GameObject.Find("FarmParent").transform;
        Log = GameObject.Find("Canvas").transform.GetChild(2).gameObject;//Lol unity (from tranform to child and then gameobject :P)
        Zone = Resources.Load<GameObject>("Zone");

        sVetsSend = 0;
        sZonePlaced = 0;
    }

    public static void LogToFile(string line)
    {
        string filepath = sDataPath + "/script.log";
        if (!File.Exists(filepath))
            File.CreateText(filepath);
        File.AppendAllLines(filepath, new []{ line });
    }

}

