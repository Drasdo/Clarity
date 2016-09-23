using UnityEngine;
using System.Collections;

public class DownloadProgress : MonoBehaviour {

    private TextMesh downloadText;
    public DownloadVideo DV;

	// Use this for initialization
	void Start () {
        downloadText = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        downloadText.text = DV.GetProgress();
	}
}
