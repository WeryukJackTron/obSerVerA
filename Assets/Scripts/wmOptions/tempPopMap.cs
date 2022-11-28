using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public struct TempCoordinate
{
    public int x;
    public int y;

    public TempCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}


public class tempPopMap : MonoBehaviour
{
    public List<TempCoordinate> coordsList = new List<TempCoordinate>();
    public GameObject farmPrefab, parent;

    // Start is called before the first frame update
    void Start()
    {
        genCoordList();
        populateMap();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void genCoords()
    {

    }

    void genCoordList()
    {
        int xRange = (int)Math.Ceiling(GetComponent<Renderer>().bounds.size.x);
        int yRange = (int)Math.Ceiling(GetComponent<Renderer>().bounds.size.y);

        for (int i = 0; i < 150; i++)
        {
            System.Random rnd = new System.Random();
            int x = rnd.Next(-1 * xRange/2 + 1, xRange/2 ); // creates a number between 1 and 12
            int y = rnd.Next(-1 * yRange/2 + 1, yRange/2 );
            coordsList.Add(new TempCoordinate(x, y));
        }

    }

    void populateMap()
    {
        for (int i = 0; i < coordsList.Count; i++)
        {
            Instantiate(farmPrefab, new Vector3(coordsList[i].x, coordsList[i].y, 0), Quaternion.identity, parent.transform);
        }
    }

}
