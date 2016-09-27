using UnityEngine;
using System.Collections;

[System.Serializable]
public class Node : System.Object {
    public string nodeTitle;
    public string sphereVideo;                  //where the spherevideo is located locally, either as it always been or downloaded
    public string SphVidOnlineLoc;              //online location of video
    public string SphVidFadeOutLocal;           //where the fadout for this clip is located locally, after download
    public string SphVidFadeOut;                //where the fadeout ending for this clip is located (online)
    public string choiceVideoLeft;
    public string choiceVideoRight;
    public int choicesSecondsToShow;
    public int choiceRotationToSummonAt;
    public int startingRotationOfVideoDegrees;
    public bool resetBlur;                      //should this video reset the blur that has been applied?
    public bool choiceLeftAddBlur;              //does the left choice here mean adding blur to the scene?
    public bool choiceRightAddBlur;             //does the right choice [[[as above]]]

    public int leftChoiceElementNumber;
    public int rightChoiceElementNumber;

    public void toMilliseconds()
    {
        choicesSecondsToShow = choicesSecondsToShow * 1000;
    }
}
