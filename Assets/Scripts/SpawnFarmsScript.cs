using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFarmsScript : MonoBehaviour
{
    public Transform FarmPrefab;
    public Transform Target;

    // Start is called before the first frame update
    void Start()
    {
        GameContext.Init();
        for (int i = 0; i < FarmInitScript.sProps.Length; i++)
            Instantiate(FarmPrefab, Target);
    }

}
