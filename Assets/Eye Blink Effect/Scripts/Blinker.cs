using UnityEngine;
using System.Collections;

public class Blinker : MonoBehaviour {

    public PostProcess.BlinkEffect blinkey;
	// Use this for initialization
	void Start () {
        blinkey = GetComponent<PostProcess.BlinkEffect>();
        blinkey.Blink();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
