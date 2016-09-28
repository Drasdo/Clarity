using UnityEngine;
using System.Collections;

public class StartingFrameSelector : MonoBehaviour {

    public Texture initTexture;
    public Texture leftChoice;
    public Texture rightChoice;
    public float duration = 2.0f;

    private bool transititioning = false;
    private Renderer rend;
    private float lookTimer = 0.0f;

    // Use this for initialization
    void Start () {
        rend = GetComponent<Renderer>();
        rend.material.SetTexture("_TexMat1", initTexture);
	}
	
	// Update is called once per frame
	void Update () {
	    if(transititioning)
        {
            lookTimer += Time.deltaTime;
            if (lookTimer / duration < duration)
            {
                rend.material.SetFloat("_Blend", Mathf.Lerp(0f, 1f, lookTimer / duration));
            } else
            {
                lookTimer = 0.0f;
                transititioning = false;
            }

        }
	}

    public void changeTexture(bool left)
    {
        if(left)
        {
            rend.material.SetTexture("_TexMat2", leftChoice);
        }
        else
        {
            rend.material.SetTexture("_TexMat2", rightChoice);
        }
        transititioning = true;
    }
}
