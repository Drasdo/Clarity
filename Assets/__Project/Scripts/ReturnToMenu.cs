using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown("space"))
        {
            GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(0).gameObject.GetComponent<EnsureReticleIsOn>().makeReticleGreatAgain();
            SceneManager.LoadScene(0);
        }
	}
}
