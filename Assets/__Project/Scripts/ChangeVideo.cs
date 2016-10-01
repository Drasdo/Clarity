using UnityEngine;
using System.Collections;

public class ChangeVideo : MonoBehaviour {

    public GameObject videoSphere;
    public string videoToChangeTo;

    private MediaPlayerCtrl SphereMediaPlayer;

    void Start () {
        SphereMediaPlayer = videoSphere.GetComponent<MediaPlayerCtrl>();
    }

    // Update is called once per frame
    void Update () {

    }

    public void ChangeToNewVideo()
    {
        if(SphereMediaPlayer == null)
        {
            Start();
        }
        SphereMediaPlayer.Load(videoToChangeTo);
        print("Playing Video " + videoToChangeTo);
    }
}
