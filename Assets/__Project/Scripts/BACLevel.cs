using UnityEngine;
using System.Collections;

public class BACLevel : MonoBehaviour {

    public static BACLevel BAC;

    private AddBlur addBlur;
    public string videoLocations;

    private MediaPlayerCtrl BACplayer;
    private bool increasingBAC = false;
    private int currentLevel = 0;

    private bool showingBAC = false;
    private bool fadingBAC = false;

    private float fromBlur = 0f;
    private float toBlur = 0f;

    private Renderer ren;

    private BasicTimer timer;

    private Vector3 startPosition;
    private Vector3 endPosition;

	// Use this for initialization
	void Start () {
        timer = gameObject.AddComponent<BasicTimer>();
        //gameObject.SetActive(false);
        BACplayer = gameObject.GetComponent<MediaPlayerCtrl>();
        ren = gameObject.GetComponent<Renderer>();
        addBlur = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AddBlur>();
        BAC = this;
        startPosition = new Vector3(0, 1, 8);
        endPosition = transform.localPosition;
        playCorrectVideo();
    }
	
	// Update is called once per frame
	void Update () {
        if(increasingBAC)
        {
            if(!showingBAC && !fadingBAC)
            {
                //nothing has started happening yet
                playCorrectVideo();
                fromBlur = toBlur;
                toBlur += addBlur.blurIntensity;
                timer.StartTimer(3.0f); //start a timer for 3 seconds. Match to the animation length
                showingBAC = true;
            }
            else if(showingBAC && !fadingBAC)
            {
                //lerp the blur up to an inflated number
                addBlur.setCurrentBlur(Mathf.Lerp(toBlur + addBlur.blurIntensity/2, fromBlur, timer.lerpVal()));
                if(timer.IsTimerFinished())
                {
                    fadingBAC = true;
                    timer.ResetOrCancelTimer();
                    timer.StartTimer(10.0f); //start the move timer
                }
                //we are currently holding the new BAC level

            }
            else if(showingBAC && fadingBAC)
            {
                //we are fading the bac away
                //lerp the blur back down to the appropriate level
                addBlur.setCurrentBlur(Mathf.Lerp(toBlur, toBlur + addBlur.blurIntensity/2, timer.lerpVal()));
                //lerp fade away this renderer
                transform.localPosition = Vector3.Lerp(endPosition, transform.localPosition, timer.lerpVal());
                if(timer.IsTimerFinished())
                {
                    resetValues();
                    //gameObject.SetActive(false); //check if you can reset the values afterwards (may not run)
                }
            }
        }
	}

    private void playCorrectVideo()
    {
        switch(currentLevel)
        {
            case 0:
                BACplayer.Load(videoLocations + "/zer.mp4");
                break;
            case 1:
                BACplayer.Load(videoLocations + "/low.mp4");
                break;
            case 2:
                BACplayer.Load(videoLocations + "/med.mp4");
                break;
            case 3:
                BACplayer.Load(videoLocations + "/hig.mp4");
                break;
            default:
                BACplayer.Load(videoLocations + "/ext.mp4");
                break;
        }
    }

    void resetValues()
    {
        increasingBAC = false;
        showingBAC = false;
        fadingBAC = false;
        timer.ResetOrCancelTimer();
        gameObject.transform.localPosition = endPosition;
    }

    public void increaseBAC()
    {
        resetValues();
        increasingBAC = true;
        transform.localPosition = startPosition;
        transform.parent.GetComponent<MoveToFrontOfCamera>().moveImmediatelyToHere();
        currentLevel++;
    }

    public void decreaseBAC()
    {
        currentLevel--;
        playCorrectVideo();
        BACplayer.Pause();
        addBlur.updateBlurValues(-addBlur.blurIntensity);
        //BACplayer.SeekTo(BACplayer.GetDuration() - 1);
    }

    public int getCurrentBAC()
    {
        return currentLevel;
        // 0 = none
        // 1 = low
        // 2 = medium
        // 3 = high
        // 4 = high
    }
}
