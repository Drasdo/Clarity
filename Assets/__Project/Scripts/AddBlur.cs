using UnityEngine;
using System.Collections;

public class AddBlur : MonoBehaviour {

    public Shader blurShader;
    public float blurIntensity = 0.10f;

    public float currentBlur = 0f;

    private GameObject eyeLeft;
    private GameObject eyeRight;
    private StereoController SC;
    public bool blinking = false;
	private MediaPlayerCtrl spherePlayer;

    private BasicTimer timer;

    // Use this for initialization
    void Start () {
        timer = gameObject.AddComponent<BasicTimer>();
    }
	
	// Update is called once per frame
	void Update () {
	    if(SC == null)
        {
            SwitchOffDirectRender();
        }
        if(eyeLeft == null)
        {
            FindEyes();
            AddBlurToEyes();
        }
		if(blinking)
		{
			spherePlayer.SetVolume(Mathf.Lerp(0.0f, 1.0f, timer.timeRemaining() / 6.5f));
			float vol = Mathf.Lerp(0.0f, 1.0f, timer.timeRemaining() / 6.5f);
			Debug.Log (vol);
		}
	}

    void FindEyes()
    {
        GvrEye[] eyes = GetComponentsInChildren<GvrEye>();
        foreach (GvrEye eye in eyes)
            if (eye.gameObject.name == "Main Camera Left")
            {
                eyeLeft = eye.gameObject;
            }
            else
            {
                eyeRight = eye.gameObject;
            }
    }

    void AddBlurToEyes()
    {
        PostProcess.BlinkEffect eyeL = eyeLeft.AddComponent<PostProcess.BlinkEffect>();
        PostProcess.BlinkEffect eyeR = eyeRight.AddComponent<PostProcess.BlinkEffect>();
        //eyeLeft.AddComponent<Blinker>();
        //eyeRight.AddComponent<Blinker>();

        eyeLeft.AddComponent<UnityStandardAssets.ImageEffects.MotionBlur>();
        eyeRight.AddComponent<UnityStandardAssets.ImageEffects.MotionBlur>();
        eyeLeft.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().shader = blurShader;
        eyeLeft.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = 0;
        eyeRight.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().shader = blurShader;
        eyeRight.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = 0;
    }

    public void updateBlurValues(float addBlurIntensity)
    {
        //if addBlurIntensity is -1 lets make the number 0 so we have a safe reset)
        if(addBlurIntensity == -1)
        {
            currentBlur = 0;
        }
        currentBlur += addBlurIntensity;
        if(currentBlur < 0)
        {
            currentBlur = 0;
        }
        eyeLeft.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = currentBlur;
        eyeRight.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = currentBlur;
    }

    void SwitchOffDirectRender()
    {
        if (gameObject.GetComponent<StereoController>() != null)
        {
            SC = gameObject.GetComponent<StereoController>();
            SC.directRender = false;
            SC.UpdateStereoValues();
        }
    }

    public void setCurrentBlur(float blurValue)
    {
        eyeLeft.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = blurValue;
        eyeRight.GetComponent<UnityStandardAssets.ImageEffects.MotionBlur>().blurAmount = blurValue;
    }

    public void setCurrentFinalBlur(float blur)
    {
        if(blur == -1)
            currentBlur = 0;
        else
        currentBlur = blur;
    }

    public void makeBlink(System.Action action, MediaPlayerCtrl sP)
    {
        eyeLeft.GetComponent<PostProcess.BlinkEffect>().Blink(null, action);
        eyeRight.GetComponent<PostProcess.BlinkEffect>().Blink();
        turnDownVolume(sP);
    }

	public void makeBlink(MediaPlayerCtrl sP)
    {
        eyeLeft.GetComponent<PostProcess.BlinkEffect>().Blink();
        eyeRight.GetComponent<PostProcess.BlinkEffect>().Blink();
        turnDownVolume(sP);
    }

	private void turnDownVolume(MediaPlayerCtrl sP)
    {
		spherePlayer = sP;
        blinking = true;
        timer.StartTimer(6.5f);
    }
}
