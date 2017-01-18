using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Node : System.Object {
    public string nodeTitle;
    public string nodeID;
    public string sphereVideo;                  //where the spherevideo is located locally, either as it always been or downloaded
   // [HideInInspector]
    public string SphVidOnlineLoc;              //online location of video
    public bool isThereEndAudio = true;
    public AudioClip TailendAudio;                 //audio to play while the user makes a choice
    public bool endingVideo = false;
    public bool choiceIsVideo = false;             //because they dont seem to be giving me videos, we need an option to set the choices as text for icons

    public int choicesSecondsToShow;
    public int choiceRotationToSummonAt;
    public int startingRotationOfVideoDegrees;

    public float waitTime = 0.8f;               //time that black stays between video so that it can load.


    [Header("BAC/ Blur Settings")]
    [Tooltip("During this video, should the BAC increase at a given time?")]
    public bool increaseBlurAtTime = false;
    [Tooltip("During this video, should the BAC decrease at a given time?")]
    public bool reduceBlurAtTime = false;
    [Tooltip("The time to increase/decrease the BAC")]
    public int TimeForChange;
    [Tooltip("Reset the blur at the start of playing this video?")]
    public bool resetBlur;                      //should this video reset the blur that has been applied?
    [Tooltip("Reset the blur at the start of playing this video?")]
    public bool doubleBlur = false;

    [Header("Choice Settings")]

    [Tooltip("Text that will be seen for the left hand choice at the end of video")]
    public string choiceVideoLeft = "Left Choice";
    [Tooltip("Video ID of video that will play next when the BAC is at a given level")]
    public List<int> leftChoiceID = new List<int>(5);
    [Tooltip("does selecting this increase the BAC/ blur level")]
    public bool choiceLeftAddBlur;              //does the left choice here mean adding blur to the scene?
    [Tooltip("does selecting this reduce the BAC/ blur level")]
    public bool reduceBlurLeft = false;

    [Space(10)]
    [Tooltip("Text that will be seen for the right hand choice at the end of video")]
    public string choiceVideoRight = "Right Choice";
    [Tooltip("Video ID of video that will play next when the BAC is at a given level")]
    public List<int> rightChoiceID = new List<int>(5);
    [Tooltip("does selecting this increase the BAC/ blur level")]
    public bool choiceRightAddBlur;             //does the right choice [[[as above]]]
    [Tooltip("does selecting this reduce the BAC/ blur level")]
    public bool reduceBlurRight = false;

    [Header("Playback Settings")]
    [Tooltip("Video playback type. 0 = start to end (default). 1 = end early. 2 = start late. 3 = skip segment")]
    public int playbackType = 0;
    [Tooltip("Time that video will end at")]
    public int endEarlyTime = -1;
    [Tooltip("time that video will start at")]
    public int startLateTime = -1;
    [Tooltip("time that the video will skip a segment at")]
    public int skipAtTime = -1;
    [Tooltip("time that segment skip will skip to and continue playing")]
    public int skipToTime = -1;

    [Header("Ending Video Settings")]
    [Tooltip("This video if it is playing when the user hits BAC: Extreme, will terminate immediately")]
    public bool endToBlack;
    [Tooltip("If this video is being played when BAC: Extreme, it will finish at this time")]
    public int earlyVideoEndAt = -1;

    public void toMilliseconds()
    {
        if(choicesSecondsToShow <= 1000)
        {  
            choicesSecondsToShow = choicesSecondsToShow * 1000;
        }
    }
}
