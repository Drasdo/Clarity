using UnityEngine;
using System.Collections;

public class PauseBeforeEnd : MonoBehaviour {

    private MediaPlayerCtrl spherePlayer;

	// Use this for initialization
	void Start () {
        spherePlayer = GetComponent<MediaPlayerCtrl>();
    }
	
	// Update is called once per frame
	void Update () {

	    if(spherePlayer.GetDuration() > 0 && spherePlayer.GetSeekPosition() >= spherePlayer.GetDuration())
        {
            //spherePlayer.Pause();
        }
	}
}
