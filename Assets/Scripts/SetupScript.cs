using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SetupScript : MonoBehaviour
{
    public Transform ProgressBar;
    public Transform Tip;
    public List<string> Tips;

    public uint Days = 7;
    public float Beta = 0.1f;
    public float Gamma = 0.077f;

    public float AngularVelocity = 360.0f;
    public float Min = 0.05f;
    public float Max = 0.9f;

    public int Iterations = 5;
    public float MinFarmDistance = 4.5f;

    private bool mExpanding = true;
    private float mAccumulatror = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        GameContext.sCurrentDay = 1;
        GameContext.sBeta = Beta;
        GameContext.sGamma = Gamma;
        ModelHandler.Init(Days);
    }

    // Update is called once per frame
    void Update()
    {
        if(!ModelHandler.IsModelRunning())
        {
            StartGame();
            return;
        }

        Image img = ProgressBar.GetComponent<Image>();
        if (mAccumulatror >= 1.0f)
        {
            mExpanding = !mExpanding;
            mAccumulatror -= 1.0f;
        }

        float min = mExpanding ? Min : Max;
        float max = mExpanding ? Max : Min;
        img.fillAmount = Mathf.Lerp(min, max, mAccumulatror);
        mAccumulatror += Time.deltaTime;

        float z = ProgressBar.eulerAngles.z - Time.deltaTime * AngularVelocity;
        ProgressBar.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, z));
    }

    void StartGame()
    {
        //for(int i = 0; i < GameContext.sFarmsInfo.Length; i++)
        //{
        //    FarmInfo info = new FarmInfo(false, false, false, false);
        //    GameContext.sFarmsInfo[i] = info;
        //}
        //
        //List<ushort> infected = ModelHandler.GetInfected();
        //if (infected.Count > 0)
        //{
        //    int index = UnityEngine.Random.Range(0, infected.Count);
        //    GameContext.sFarmsInfo[infected[index] - 1].Exclamation = true;
        //}
        //else
        //    UnityEngine.Debug.Log("Not farm with infection");
        //
        //SceneManager.LoadScene("WorldMap");

        FarmInitScript.sProps = new FarmProperty[GameContext.sNumberOfFarms];
        FarmInitScript.sNextIndex = 0;
        string query = "SELECT node, x, y FROM ldata;";
        int index = 0;
        ExecuteQuery(query, true, (ref IDataReader reader) =>
        {
            ushort id = (ushort)reader.GetInt16(0);

            float x = ((float)reader.GetDouble(1) / 50000.0f - 0.5f) * 110.0f;
            float y = ((float)reader.GetDouble(2) / 50000.0f - 0.5f) * 110.0f;

            FarmInitScript.sProps[index] = new FarmProperty(id, x, y);
            index++;
        });

        float squareDistance = MinFarmDistance * MinFarmDistance;
        for (int iter = 0; iter < Iterations; iter++)
        {
            for (int i = 0; i < GameContext.sNumberOfFarms - 1; i++)
            {
                Vector2 pos0 = new Vector2(FarmInitScript.sProps[i].x, FarmInitScript.sProps[i].y);
                for (int j = 1; j < GameContext.sNumberOfFarms; j++)
                {
                    Vector2 pos1 = new Vector2(FarmInitScript.sProps[j].x, FarmInitScript.sProps[j].y);

                    float distance = QuickDistance(pos0.x, pos0.y, pos1.x, pos1.y);
                    if (distance <= squareDistance)
                    {
                        Vector2 dir = (pos0 - pos1).normalized;
                        pos0 += dir * MinFarmDistance;
                        if (pos0.x > -55.0f && pos0.x < 55.0f && pos0.y > -55.0f && pos0.y < 55.0f)
                            FarmInitScript.sProps[i] = new FarmProperty(FarmInitScript.sProps[i].ID, pos0.x, pos0.y);
                        else
                        {
                            pos1 -= dir * MinFarmDistance;
                            FarmInitScript.sProps[i] = new FarmProperty(FarmInitScript.sProps[j].ID, pos1.x, pos1.y);
                        }
                    }

                }
            }
        }

        SceneManager.LoadScene("wmOption2");
    }

    /// <summary>Calculates Euclidean distance without using sqrt cause is expensive</summary>
    private static float QuickDistance(float x0, float y0, float x1, float y1)
    {
        float x = x0 - x1; x *= x; 
        float y = y0 - y1; y *= y;
        return x + y;
    }

    private static void ExecuteQuery(string query, bool selection = false, ActionRef<IDataReader> action = null)
    {
        string connection = string.Format("URI=file:{0}/{1}", Application.dataPath, "model.sqlite");
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
}
