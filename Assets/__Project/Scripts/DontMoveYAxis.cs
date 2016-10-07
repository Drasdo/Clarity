using UnityEngine;
using System.Collections;

public class DontMoveYAxis : MonoBehaviour {

    private Vector3 startingPosition;
	// Use this for initialization
	void Start () {
        startingPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(transform.position.x, startingPosition.y, transform.position.z);
        transform.position = (transform.position - Vector3.zero).normalized * startingPosition.z + Vector3.zero;
    }
}
