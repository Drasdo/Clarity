using UnityEngine;
using System.Collections;

public class RefreshCamera : MonoBehaviour {

    private GameObject MainCamera;
    public GameObject CameraPrefab;
	// Use this for initialization
    void Awake () {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        Destroy(MainCamera);
        MainCamera = GameObject.Instantiate(CameraPrefab);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
