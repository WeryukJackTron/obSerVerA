using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class buildEventSheet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int day = 1; day <= 30; day++)
        {
            Save(day);
        }
    }

    public void readCSV()
    {
        string[] Lines = System.IO.File.ReadAllLines(Application.dataPath + "\\Scripts\\EventLogs\\eventLog.csv");
        //string[] Columns = Lines[/*   INDEX  */].Split(';');
        for (int i = 0; i <= Lines.Length - 1; i++)
        {
            Debug.Log(Lines[i]);
        }
    }

    private List<string[]> rowData = new List<string[]>();

    void Save(int day)
    {

        string[] rowDataTemp = new string[3];

        // Loop through every farm
        for (int i = 1; i < 33; i++)
        {
            // Give each farm between 1 and 5 events
            int numActions = UnityEngine.Random.Range(1, 6);
            for (int j = 0; j < numActions; j++)
            {
                rowDataTemp = new string[3];
                rowDataTemp[0] = "" + i; // ID of Farm A
                //rowDataTemp[1] = "" + UnityEngine.Random.Range(0, 2); // 0 to 1 - sent or reveived cows
                rowDataTemp[1] = "" + UnityEngine.Random.Range(1, 33); // 1 to 32 - ID of Farm B
                rowDataTemp[2] = "" + UnityEngine.Random.Range(1, 15); // 1 to 14 - number of days ago
                rowData.Add(rowDataTemp);
            }
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        // Create unique file paths
        string filePath = Application.dataPath + "\\Scripts\\EventLogs\\day_" + day + ".csv";

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

        rowData.Clear();
    }

}
