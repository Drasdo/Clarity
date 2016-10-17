using UnityEngine;
using System.Collections;

public class AddBlur : MonoBehaviour {

    public Shader blurShader;
    public float blurIntensity = 0.3f;

    public float currentBlur = 0f;

    private GameObject eyeLeft;
    private GameObject eyeRight;
    private StereoController SC;

	// Use this for initialization
	void Start () {

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
}
