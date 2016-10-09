using UnityEngine;
using System.Collections;

public class ChangeNodeTree : MonoBehaviour {

    public DownloadVideo DV;
    private GameObject treeStructure;
    public string assignedStructure;

    private NodeTree correctNodeTree;

	void Start () {
        treeStructure = GameObject.FindGameObjectWithTag("NodeTree");
        NodeTree[] NTs = treeStructure.GetComponents<NodeTree>();
        foreach (NodeTree tree in NTs)
        {
            if(tree.structureName == assignedStructure)
            {
                correctNodeTree = tree;
            }
        }
    }
        

    public void OnClick()
    {
        DV.assignNodeTree(correctNodeTree);
        NodeTree.currentTree = correctNodeTree.structureName;
    }
}
