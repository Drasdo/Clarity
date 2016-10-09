using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DontDestroy : MonoBehaviour {

    public float initializationTime;
    public static List<GameObject> DontDestroys = new List<GameObject>();

    void Awake()
    {
        initializationTime = Time.timeSinceLevelLoad;

        foreach (GameObject dontDoIt in DontDestroys)
        {
            if (dontDoIt.name == gameObject.name)
            {
                DestroyImmediate(gameObject);
                return;
            }
        }
        //if we get to the end, does that mean we are ok, add the object?
        DontDestroys.Add(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
