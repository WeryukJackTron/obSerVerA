using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragCamera : MonoBehaviour
{
    public static dragCamera instance = null;

    private Vector3 Origin;
    private Vector3 Difference;
    private bool Drag = false;

    public float maxX;
    public float maxY;
    public float minX;
    public float minY;
    public Camera camera;

    Vector3 pos;

    private void Start() { instance = this; }

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

    public void JumpTo(int farmID)
    {
        Transform trans = SideBarScript.Farms[farmID - 1].transform;
        Vector3 position = trans.position;
        position.x = Mathf.Clamp(position.x + 8, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        position.z = -10.0f;
        camera.transform.position = position;
    }
}
