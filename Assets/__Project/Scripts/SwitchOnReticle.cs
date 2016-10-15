using UnityEngine;
using System.Collections;

public class SwitchOnReticle : MonoBehaviour {
    
    private GameObject reticle;
	// Use this for initialization
	void Start () {
        reticle = GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(0).gameObject;
        reticle.GetComponent<Renderer>().enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
