using UnityEngine;
using System.Collections;

public class EnsureReticleIsOn : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<GvrReticle>().enabled = true;
	}

    public void makeReticleGreatAgain()
    {
        gameObject.GetComponent<GvrReticle>().enabled = false;
        StartCoroutine(turnBackOn());
    }

    IEnumerator turnBackOn()
    {
        yield return new WaitForSeconds(2);
        gameObject.GetComponent<GvrReticle>().enabled = true;
    }
}
