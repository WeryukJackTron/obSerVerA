using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static void Init()
    {
        //if (SceneManager.GetActiveScene().name != "Map" && SceneManager.GetActiveScene().name != "MapWithLoading" && SceneManager.GetActiveScene().name != "RealMap")//Check if scene is the one containing game
        //    return;

        Map = GameObject.Find("Map");
        Farms = GameObject.Find("Farms").transform;
        Log = GameObject.Find("Canvas").transform.GetChild(1).gameObject;//Lol unity (from tranform to child and then gameobject :P)
        Zone = Resources.Load<GameObject>("Zone");

        //sCurrentDay = 1;
        //ModelHandler.Init(7);
    }
}

