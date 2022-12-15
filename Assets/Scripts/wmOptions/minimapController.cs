using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapController : MonoBehaviour
{
    public Camera mainCamera, minimapCamera;
    public Collider mapCollider;
    public Material cameraBoxMaterial;
    private Vector3 BottomLeftPos, BottomRightPos, TopLeftPos, TopRightPos;

    void Update()
    {
        // subtract 480f from Screen.width to account for side bar blocking view 
        BottomLeftPos = minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(0f, 0f)));
        BottomRightPos = minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(Screen.width - 480f, 0f)));
        TopLeftPos = minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(0, Screen.height)));
        TopRightPos = minimapCamera.WorldToViewportPoint(GetCameraFrustumPosition(new Vector3(Screen.width - 480f, Screen.height)));
        BottomLeftPos.z = 1f;
        BottomRightPos.z = 1f;
        TopLeftPos.z = 1f;
        TopRightPos.z = 1f;
    } 

    private Vector3 GetCameraFrustumPosition(Vector3 position)
    {
        var positionRay = mainCamera.ScreenPointToRay(position);
        RaycastHit hit;
        if (mapCollider.Raycast(positionRay, out hit, 20f)){
            return hit.point;
        }
        else
        {
            return new Vector3();
        }
    }

    public void OnPostRender()
    {
        GL.PushMatrix();
        {
            cameraBoxMaterial.SetPass(0);
            GL.LoadOrtho();
            GL.Begin(GL.LINES);
            GL.Color(Color.blue);
            {
                GL.Vertex(BottomLeftPos);
                GL.Vertex(BottomRightPos);
                GL.Vertex(BottomRightPos);
                GL.Vertex(TopRightPos);
                GL.Vertex(TopRightPos);
                GL.Vertex(TopLeftPos);
                GL.Vertex(TopLeftPos);
                GL.Vertex(BottomLeftPos);
            }
            GL.End();
        }
        GL.PopMatrix();
    }
}
