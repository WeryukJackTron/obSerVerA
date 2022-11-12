using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetMouseButtonDown(0))
        {
            CastRay();
            Debug.Log(selectedRegion);
        }
    }

    void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().color = hoverColor;
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
        }
    }

}
