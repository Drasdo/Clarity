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
        if(SphereMediaPlayer.GetSeekPosition() > 100f) //if the video is playing
        {
            this.enabled = false;
        }
    }

    public void ChangeToNewVideo()
    {
        if(SphereMediaPlayer == null)
        {
            Start();
        }
        this.enabled = true;
        SphereMediaPlayer.UnLoad();
        SphereMediaPlayer.Load(videoToChangeTo); 
        SphereMediaPlayer.Play();  
        print("Playing Video " + videoToChangeTo);
    }
}