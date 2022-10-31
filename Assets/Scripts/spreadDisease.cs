using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spreadDisease : MonoBehaviour
{
    // List of all farms
    public List<Transform> farmsList = new List<Transform>();
    // Tracker for current day
    public int dayNumber = 1;
    // List of all farms that are infected - the current day's infected farms
    public List<int> prevInfectedFarms = new List<int>();
    // List of farms that are infected on the current day
    public List<int> currInfectedFarms = new List<int>();
    public Sprite infected;
    public static spreadDisease instance;

    // Start is called before the first frame update
    void Start()
    {
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

        int rand = UnityEngine.Random.Range(0, 32);

        farmsList[rand].GetChild(1).gameObject.SetActive(true);
        //farmsList[rand].GetComponent<SpriteRenderer>().sprite = infected;
        // Convert string name of farm to int, add it to currInfected Farms
        currInfectedFarms.Add(int.Parse(farmsList[rand].name));

        Debug.Log(currInfectedFarms[0]);

    }

    public void readCurrentDayEventLog(int dayNum)
    {
        string filename = "D:\\Unity Projects\\Test\\My project\\Assets\\Scripts\\EventLogs\\day_" + dayNum + ".csv";
        string[] Lines = System.IO.File.ReadAllLines(filename);
        
        // Find the event log lines corresonding to all farms in currInfectedFarms
        for (int i = 0; i < Lines.Length - 1; i++)
        {
            for (int j = 0; j <= currInfectedFarms.Count - 1; j++)
            {
                if ( int.Parse(Lines[i].ToString().Split(",")[0]) == currInfectedFarms[j])
                {
                    int spreadToFarmX = int.Parse(Lines[i].ToString().Split(",")[2]);
                    spreadFromContact(spreadToFarmX);
                    Debug.Log(int.Parse(Lines[i].ToString().Split(",")[2]));

                }
            }
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


