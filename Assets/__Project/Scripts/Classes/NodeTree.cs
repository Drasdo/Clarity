﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NodeTree : MonoBehaviour
{
    public static string currentTree;
    public string structureName;
    public bool localFiles = false;
    public string onlineFileLocation;
    public List<Node> videoStructure = new List<Node>();
    public float initializationTime;
    public bool everythingReady = false;

    void Awake()
    {
        initializationTime = Time.timeSinceLevelLoad;
        DontDestroyOnLoad(transform.gameObject);
        GameObject[] nodeTrees = GameObject.FindGameObjectsWithTag("NodeTree");

        foreach (GameObject tree in nodeTrees)
        {
            if (!tree.Equals(gameObject))
            {
                if (initializationTime > tree.GetComponent<NodeTree>().initializationTime)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(tree);
                }
            }
        }
        UpdateFields();
    }

    void UpdateFields()
    {
        foreach(Node node in videoStructure)
        {
            if(localFiles)
            {
                //structureName + "/" + node.nodeTitle + "/" + node.nodeTitle + "_";
                node.SphVidOnlineLoc = structureName + "/" + node.nodeTitle + "/" + node.nodeTitle + "_HD.mp4";
            }
            if(node.SphVidOnlineLoc == "")
            {
                node.SphVidOnlineLoc = node.sphereVideo;
            }
            else
            {
                node.SphVidOnlineLoc = onlineFileLocation + structureName + "/" + node.nodeTitle + "/" + node.nodeTitle + "_";
            }
        }
    }
}