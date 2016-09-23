using UnityEngine;
using System.Collections;

public class LookToCenter : MonoBehaviour {

    public float speed = 3.0f; //Speed at which the object rotates towards the camera.
    private GameObject mainCamera;

	// Use this for initialization
	void Start () {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 correctLookVector = new Vector3(mainCamera.transform.position.x, 0, mainCamera.transform.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - correctLookVector), speed * Time.deltaTime);
    }
}
