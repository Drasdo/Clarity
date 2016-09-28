using UnityEngine;
using System.Collections;

public class StartingFrameSelector : MonoBehaviour {

    public Texture initTexture;
    public Texture leftChoice;
    public Texture rightChoice;
    public float duration = 2.0f;

    private Material currentMat;
    private bool transititioning = false;
    private Renderer rend;



    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        currentMat = GetComponent<Material>();
        currentMat.SetTexture("_TexMat1", initTexture);
        rend.material = currentMat;
	}
	
	// Update is called once per frame
	void Update () {
	    if(transititioning)
        {
            float lerp = Mathf.PingPong(Time.time, duration) / duration;
            rend.material.SetFloat("_Blend", lerp);
        }
	}

    public void changeTexture(bool left)
    {
        if(left)
        {
            currentMat.SetTexture("_TexMat2", leftChoice);
        }
        else
        {
            currentMat.SetTexture("_TexMat2", rightChoice);
        }
        transititioning = true;
    }
}
