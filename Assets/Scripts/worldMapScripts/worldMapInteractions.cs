using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;

public class worldMapInteractions : MonoBehaviour
{

    private Color basicColor = Color.green;
    private Color hoverColor = new Color(107f / 255f, 255f / 255f, 107f / 255f);

    public string selectedRegion = "";

    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = basicColor;
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().color = hoverColor;
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
        }
    }

    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = basicColor;
    }

    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit.collider != null)
        {
            selectedRegion = (hit.collider.gameObject.name);
            int quad = int.Parse(selectedRegion.Substring(4));
            Vector2 xClamp = new Vector2(0.0f, 0.0f);
            Vector2 yClamp = new Vector2(0.0f, 0.0f);

            switch(quad)
            {
                case 1:
                    xClamp = new Vector2(25000.0f, 50000.0f);
                    yClamp = new Vector2(25000.0f, 50000.0f);
                    break;
                case 2:
                    xClamp = new Vector2(0.0f, 25000.0f);
                    yClamp = new Vector2(25000.0f, 50000.0f);
                    break;
                case 3:
                    xClamp = new Vector2(0.0f, 25000.0f);
                    yClamp = new Vector2(0.0f, 25000.0f);
                    break;
                case 4:
                    xClamp = new Vector2(25000.0f, 50000.0f);
                    yClamp = new Vector2(0.0f, 25000.0f);
                    break;
            }

            string query = string.Format("SELECT node, x, y FROM ldata WHERE x > {0} AND x < {1} AND y > {2} AND y < {3};", xClamp.x, xClamp.y, yClamp.x, yClamp.y);
            List<FarmProperty> props = new List<FarmProperty>();
            ExecuteQuery(query, true, (ref IDataReader reader) =>
            {
                ushort id = (ushort)reader.GetInt16(0);
                float x = (float)reader.GetDouble(1);
                float y = (float)reader.GetDouble(2);

                x -= xClamp.x;
                y -= yClamp.x;

                props.Add(new FarmProperty(id, x, y));
            });

            FarmInitScript.sNextIndex = 0;
            FarmInitScript.sProps = new FarmProperty[props.Count];
            for (int i = 0; i < props.Count; i++)
            {
                FarmProperty prop = props[i];
                FarmInitScript.sProps[i] = new FarmProperty(prop.ID, prop.x, prop.y);
            }

            SideBarScript.Farms = new GameObject[props.Count];
            SceneManager.LoadScene("RealMap");
        }
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
