using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NodeTree : MonoBehaviour
{
    public List<Node> videoStructure = new List<Node>();

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        foreach(Node node in videoStructure)
        {
            if(node.SphVidOnlineLoc == "")
            {
                node.SphVidOnlineLoc = node.sphereVideo;
            }
        }
    }
}