using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class worldMapInteractions : MonoBehaviour
{

    private Color basicColor = Color.green;
    private Color hoverColor = new Color(107f / 255f, 255f / 255f, 107f / 255f);

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = basicColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().color = hoverColor;
    }

    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().color = basicColor;
    }
}
