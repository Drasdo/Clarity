using UnityEngine;
using System.Collections;

public class BACLevel : MonoBehaviour {

    public AddBlur addBlur;
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

	// Use this for initialization
	void Start () {
        timer = gameObject.AddComponent<BasicTimer>();
        gameObject.SetActive(false);
        BACplayer = gameObject.GetComponent<MediaPlayerCtrl>();
        ren = gameObject.GetComponent<Renderer>();
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
                addBlur.setCurrentBlur(Mathf.Lerp(fromBlur, toBlur+addBlur.blurIntensity, timer.lerpVal()));
                if(timer.IsTimerFinished())
                {
                    fadingBAC = true;
                    timer.ResetOrCancelTimer();
                    timer.StartTimer(2.0f); //start the fade timer
                }
                //we are currently holding the new BAC level

            }
            else if(showingBAC && fadingBAC)
            {
                //we are fading the bac away
                //lerp the blur back down to the appropriate level
                addBlur.setCurrentBlur(Mathf.Lerp(toBlur+addBlur.blurIntensity, toBlur, timer.lerpVal()));
                //lerp fade away this renderer
                ren.material.SetColor("_MainTex", new Color(0, 0, 0, Mathf.Lerp(0, 255, timer.lerpVal())));
                if(timer.IsTimerFinished())
                {
                    resetValues();
                    gameObject.SetActive(false); //check if you can reset the values afterwards (may not run)
                }

            }

        }
	}

    private void playCorrectVideo()
    {
        switch(currentLevel)
        {
            case 0:
                //set video string
                break;
            case 1:
                //something
                break;
            case 2:
                //do something
                break;
            case 3:
                //do something
                break;
            default:
                //lets be honest, probably an error
                break;
        }
        
    }

    void resetValues()
    {
        increasingBAC = false;
        showingBAC = false;
        fadingBAC = false;
        timer.ResetOrCancelTimer();
        ren.material.SetColor("_MainTex", new Color(0, 0, 0, 1));
    }

    public void increaseBAC()
    {
        increasingBAC = true;
    }

    public void decreaseBAC()
    {
        currentLevel--;
    }

    public void enableGameObject()
    {
        gameObject.SetActive(true);
    }

    //TODO: something needs to turn this on at the right time once increaseBAC has been set so shit happens
}
