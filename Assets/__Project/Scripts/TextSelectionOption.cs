using UnityEngine;
using System.Collections;

public class TextSelectionOption : MonoBehaviour {

    public bool leftChoice;
    public bool rightChoice;

    private ChangeVideo changeVideo;

    // Use this for initialization
    void Start () {
        changeVideo = transform.parent.GetComponent<ChangeVideo>();
    }

    // Update is called once per frame
    void Update () {

    }
}

