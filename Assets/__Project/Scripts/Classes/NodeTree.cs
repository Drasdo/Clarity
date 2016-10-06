using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NodeTree : MonoBehaviour
{
    public static string currentTree;
    public string structureName;
    public string onlineFileLocation;
    public List<Node> videoStructure = new List<Node>();
    public float initializationTime;

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