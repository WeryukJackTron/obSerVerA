using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        createChildFarms();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void createChildFarms()
    {
        for (int i = 0; i < coordsList.Count; i++)
        {
            Instantiate(farmPrefab, new Vector3(coordsList[i].x/100, coordsList[i].y/100, 0), Quaternion.identity, parent.transform);
        }
    }

    void genCoordList()
    {
        List<string> temp = new List<string>();
        var path = "C:\\Users\\shane\\OneDrive\\Documents\\GitHub\\SVARM\\Assets\\Scripts\\coordinates.txt";
        foreach (string line in System.IO.File.ReadLines(path))
        {
            temp.Add(line);
            string[] tokens = line.Split(',');
            string x = tokens[0];
            string y = tokens[1];

            x = x.Substring(0, x.LastIndexOf("."));
            y = y.Substring(0, y.LastIndexOf("."));

            int xInt = int.Parse(x);
            int yInt = int.Parse(y);

            Coordinate coordinate = new Coordinate(xInt, yInt);

            coordsList.Add(coordinate);
        }

    }

}
