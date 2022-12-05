using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct Coordinate
{
    public int x;
    public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class worldMapPop : MonoBehaviour
{

    public List<Coordinate> coordsList = new List<Coordinate>();
    public GameObject farmPrefab, parent;

    // Start is called before the first frame update
    void Start()
    {
        genCoordList();
        string zone = this.gameObject.name;
        createChildFarms(zone);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void createChildFarms(string zone)
    {
        for (int i = 0; i < coordsList.Count; i++)
        {
            if (zone == "QuadMap1")
            {
                if ( (coordsList[i].x / 100 >= 250) && (coordsList[i].y / 100 >= 250))
                {
                    Instantiate(farmPrefab, new Vector3(coordsList[i].x / 100 - 250, coordsList[i].y / 100 - 250, 0), Quaternion.identity, parent.transform);
                }
            }
            else if (zone == "QuadMap2")
            {
                if ((coordsList[i].x / 100 < 250) && (coordsList[i].y / 100 >= 250))
                {
                    Instantiate(farmPrefab, new Vector3(coordsList[i].x / 100, coordsList[i].y / 100 - 250, 0), Quaternion.identity, parent.transform);
                }
            }
            else if (zone == "QuadMap3")
            {
                if ((coordsList[i].x / 100 < 250) && (coordsList[i].y / 100 < 250))
                {
                    Instantiate(farmPrefab, new Vector3(coordsList[i].x / 100, coordsList[i].y / 100, 0), Quaternion.identity, parent.transform);
                }
            }
            else if (zone == "QuadMap4")
            {
                if ((coordsList[i].x / 100 >= 250) && (coordsList[i].y / 100 < 250))
                {
                    Instantiate(farmPrefab, new Vector3(coordsList[i].x / 100 - 250, coordsList[i].y / 100, 0), Quaternion.identity, parent.transform);
                }
            }
            else
            {
                Instantiate(farmPrefab, new Vector3(coordsList[i].x / 100, coordsList[i].y / 100, 0), Quaternion.identity, parent.transform);
            }
        }
        
    }

    public void backToWorldMap()
    {
        SceneManager.LoadScene("WorldMap");
    }

    void genCoordList()
    {
        //string query = "SELECT x, y FROM ldata;";
        //ExecuteQuery(query, true, (ref IDataReader reader) =>
        //{
        //    int x = (int)reader.GetDouble(0);
        //    int y = (int)reader.GetDouble(1);
        //
        //    coordsList.Add(new Coordinate(x, y));
        //});

        //List<string> temp = new List<string>();
        //var path = string.Format("{0}/Scripts/worldMapScripts/coordinates.txt", Application.dataPath);
        //foreach (string line in System.IO.File.ReadLines(path))
        //{
        //    temp.Add(line);
        //    string[] tokens = line.Split(',');
        //    string x = tokens[0];
        //    string y = tokens[1];
        //
        //    x = x.Substring(0, x.LastIndexOf("."));
        //    y = y.Substring(0, y.LastIndexOf("."));
        //
        //    int xInt = int.Parse(x);
        //    int yInt = int.Parse(y);
        //
        //    Coordinate coordinate = new Coordinate(xInt, yInt);
        //
        //    coordsList.Add(coordinate);
        //}

    }

}
