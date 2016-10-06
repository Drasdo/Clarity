using UnityEngine;
using System.Collections;

public class ChangeNodeTree : MonoBehaviour {

    public DownloadVideo DV;
    public GameObject treeStructure;
    public string assignedStructure;

    private NodeTree correctNodeTree;

	// Use this for initialization
	void Start () {
        NodeTree[] NTs = treeStructure.GetComponents<NodeTree>();
        foreach (NodeTree tree in NTs)
        {
            if(tree.structureName == assignedStructure)
            {
                correctNodeTree = tree;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        DV.assignNodeTree(correctNodeTree);
        NodeTree.currentTree = correctNodeTree.structureName;
    }
}
