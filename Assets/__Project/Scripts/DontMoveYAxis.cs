using UnityEngine;
using System.Collections;

public class DontMoveYAxis : MonoBehaviour {

    private Camera camera;
    private Vector3 startingPosition;
    private Vector3 cameraForward;
	// Use this for initialization
	void Start () {
        startingPosition = transform.GetChild(0).transform.position;
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if(camera == null)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
        cameraForward = camera.transform.forward;
        cameraForward.y = 0;
        cameraForward = (cameraForward - Vector3.zero).normalized * startingPosition.z + Vector3.zero;
        transform.LookAt(cameraForward);
        //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
    }
}
