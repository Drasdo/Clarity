using UnityEngine;

public class AutoPlayEMTVideo : MonoBehaviour
{
    public MediaPlayerCtrl scrMedia;

    void Start()
    {
        scrMedia.Play(); // Autoplay on start
        //scrMedia.loop = true; // Loop forever
    }

}