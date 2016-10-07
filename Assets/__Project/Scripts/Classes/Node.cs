using UnityEngine;
using System.Collections;

[System.Serializable]
public class Node : System.Object {
    public string nodeTitle;
    public string nodeID;
    public string sphereVideo;                  //where the spherevideo is located locally, either as it always been or downloaded
    [HideInInspector]
    public string SphVidOnlineLoc;              //online location of video
    public bool isThereEndAudio = true;
    public AudioClip TailendAudio;                 //audio to play while the user makes a choice
    public bool endingVideo = false;
    public string choiceVideoLeft = "Left Choice";
    public string choiceVideoRight = "Right Choice";
    public bool choiceIsVideo = false;             //because they dont seem to be giving me videos, we need an option to set the choices as text for icons
    public int choicesSecondsToShow;
    public int choiceRotationToSummonAt;
    public int startingRotationOfVideoDegrees;
    public bool resetBlur;                      //should this video reset the blur that has been applied?
    public bool choiceLeftAddBlur;              //does the left choice here mean adding blur to the scene?
    public bool choiceRightAddBlur;             //does the right choice [[[as above]]]

    public int leftChoiceElementNumber;
    public int rightChoiceElementNumber;
    public int blurExceedsMaxMoveTo = -1;

    public void toMilliseconds()
    {
        choicesSecondsToShow = choicesSecondsToShow * 1000;
    }
}
