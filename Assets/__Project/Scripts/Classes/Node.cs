using UnityEngine;
using System.Collections;

[System.Serializable]
public class Node : System.Object {
    public string nodeTitle;
    public string sphereVideo;
    public string SphVidOnlineLoc;
    public string choiceVideoLeft;
    public string choiceVideoRight;
    public int choicesSecondsToShow;
    public int choiceRotationToSummonAt;
    public int startingRotationOfVideoDegrees;

    public int leftChoiceElementNumber;
    public int rightChoiceElementNumber;

    public void toMilliseconds()
    {
        choicesSecondsToShow = choicesSecondsToShow * 1000;
    }
}
