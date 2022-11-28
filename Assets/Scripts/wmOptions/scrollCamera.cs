using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrollCamera : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        //54 to -56
        if (transform.position.y < 54 && transform.position.y > -56)
        {
            transform.position += new Vector3(0f, Input.GetAxis("Mouse ScrollWheel") * 5000 * Time.deltaTime, 0f);
        }
        else
        {
            if (transform.position.y > 0)
            {
                transform.position -= new Vector3(0f, 1f, 0f);
            }
            else
            {
                transform.position += new Vector3(0f, 1f, 0f);
            }
        }

    }



}
