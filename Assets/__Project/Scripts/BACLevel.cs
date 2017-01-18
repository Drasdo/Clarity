using UnityEngine;
using System.Collections;

public class BACLevel : MonoBehaviour {

    public static BACLevel BAC;

    private AddBlur addBlur;
    public string videoLocations;

    private Playback[] BACplayer;
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
        BACplayer = gameObject.GetComponents<Playback>();
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
                timer.StartTimer(6f); //start a timer for 6 seconds. Match to the animation length
                showingBAC = true;
            }
            else if(showingBAC && !fadingBAC)
            {
                //lerp the blur up to an inflated number
                addBlur.setCurrentBlur(Mathf.Lerp(toBlur + addBlur.blurIntensity/4, fromBlur, timer.lerpVal()));
                if(timer.IsTimerFinished())
                {
                    fadingBAC = true;
                    timer.ResetOrCancelTimer();
                    timer.StartTimer(5.5f); //start the move timer
                }
                //we are currently holding the new BAC level

            }
            else if(showingBAC && fadingBAC)
            {
                //we are fading the bac away
                //lerp the blur back down to the appropriate level
                addBlur.setCurrentBlur(Mathf.Lerp(toBlur, toBlur + addBlur.blurIntensity/4, timer.lerpVal()));
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
        switch (currentLevel)
        {
            case 0:
                //BACplayer[0].Play();
                break;
            case 1:
                BACplayer[0].Play();
                break;
            case 2:
                BACplayer[1].Play();
                break;
            case 3:
                BACplayer[2].Play();
                break;
            default:
                BACplayer[3].Play();
                break;
        }
        print("Playing Video");
    }

    void resetValues()
    {
        increasingBAC = false;
        showingBAC = false;
        fadingBAC = false;
        if(timer != null)
            timer.ResetOrCancelTimer();
        if(gameObject != null)
        gameObject.transform.localPosition = endPosition;
    }

    public void increaseBAC()
    {
        resetValues();
        currentLevel++;
        if (currentLevel >= 5)
        {
            currentLevel = 4;
        }
        increasingBAC = true;
        transform.localPosition = startPosition;
        transform.parent.GetComponent<MoveToFrontOfCamera>().moveImmediatelyToHere();
    }

    public void increaseBAC(bool twice)
    {
        resetValues();
        currentLevel++;
        if (currentLevel >= 5)
        {
            currentLevel = 4;
        }
        increasingBAC = true;
        transform.localPosition = startPosition;
        transform.parent.GetComponent<MoveToFrontOfCamera>().moveImmediatelyToHere();
        if (twice)
        {
            currentLevel++;
            fromBlur += addBlur.blurIntensity;
            toBlur += addBlur.blurIntensity;
        }
    }

    public void decreaseBAC()
    {
        currentLevel--;
        if (currentLevel <= 0)
        {
            currentLevel = 0;
        }
        playCorrectVideo();
        //BACplayer.Pause();
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
        // 4 = ext
    }
}
