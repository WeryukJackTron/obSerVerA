using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragCamera : MonoBehaviour
{
    private Vector3 Origin;
    private Vector3 Difference;
    private bool Drag = false;

    public float maxX;
    public float maxY;
    public float minX;
    public float minY;
    public Camera camera;

    Vector3 pos;


    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Difference = (camera.ScreenToWorldPoint(Input.mousePosition)) - camera.transform.position;
            if (Drag == false)
            {
                Drag = true;
                Origin = camera.ScreenToWorldPoint(Input.mousePosition);
            }     
        }
        else
        {
            Drag = false;
        }

        if (Drag == true)
        {
            pos = Origin - Difference;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            camera.transform.position = pos;
        }

    }

}
