using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BranchingStoryController : MonoBehaviour {

    public GameObject TextSelector;
    public GameObject videoSphere;
    public GameObject choiceRotator;

    private List<Node> videoStructure = new List<Node>(); 

    private Node currentNode;
    private MediaPlayerCtrl spherePlayer;   //player of the sphere video
    private MediaPlayerCtrl[] choicePlayer; //will point to the video players for each choice, right then left
    private bool notRevealedChoices;        //so we know when the choices has begun revealing so we can only trigger that once
    private bool finalScene;                //flag so we know if we need to show choices or not  

    private NodeTree nodeTree; 
                                                
    private ChangeVideo changeVideo;

	void Start () {
        spherePlayer = videoSphere.GetComponent<MediaPlayerCtrl>();
        changeVideo = TextSelector.GetComponent<ChangeVideo>();
        choicePlayer = changeVideo.GetComponentsInChildren<MediaPlayerCtrl>(); //the assumption is right is first, will need to check
        nodeTree = GameObject.FindGameObjectWithTag("NodeTree").GetComponent<NodeTree>();
        finalScene = false;
    
        LoadInTreeStructure();
        currentNode = videoStructure[0]; ; //0 will be our starting video;
        assignEnvironmentProperties();
    }
	
	void Update () {
        //are we assuming the video is playing? lets go with that now
        if (!finalScene)
        {
            if (notRevealedChoices && spherePlayer.GetSeekPosition() > currentNode.choicesSecondsToShow)
            {
                revealTextChoices();
                notRevealedChoices = false;
            }
        }
        else
        {
            finalSceneChecker();
        }
    }

    public void UpdateTextSelectorForCurrentBranch(bool leftBranchSelected) //Run as a new video is loaded, so that all the paremeters are set and ready to go.
    {
        //we need to check if the element number of choice is -1 because that means we need to end)
        //but what if the choice plays a video but then thats the end choice? need to maybe know when the video is loaded and not do the choice options)
        currentNode = (leftBranchSelected) ? videoStructure[currentNode.leftChoiceElementNumber] : videoStructure[currentNode.rightChoiceElementNumber];
        TextSelector.GetComponent<RevealOnVisible>().ResetVisibility();
        assignEnvironmentProperties();
    }

    void assignEnvironmentProperties()
    {
        changeVideo.videoToChangeTo = currentNode.sphereVideo;
        choicePlayer[0].m_strFileName = currentNode.choiceVideoLeft; //Left
        choicePlayer[1].m_strFileName = currentNode.choiceVideoRight; //Right
        changeVideo.ChangeToNewVideo();
        notRevealedChoices = false;
        spherePlayer.transform.Rotate(new Vector3(0, currentNode.startingRotationOfVideoDegrees, 0)); //rotate the video to correct orientation if need be. Hopefully this doesn't actually have to be used, but nice to have anyway.
        choiceRotator.transform.Rotate(new Vector3(0, currentNode.choiceRotationToSummonAt, 0));
        currentNode.toMilliseconds();
        if(currentNode.choicesSecondsToShow == 0)
        {
            finalScene = true;
        }
        notRevealedChoices = true;
    }

    void revealTextChoices()
    {
        TextSelector.GetComponent<RevealOnVisible>().beginReveal();
    }

    void finalSceneChecker()
    {
        int seekPos = spherePlayer.GetSeekPosition();
        int totalDur = spherePlayer.GetDuration();
        if (totalDur != 0)
        {
            if (seekPos >= totalDur)
            {
                //oh gosh we hhave ended! What do?
                SceneManager.LoadScene(0);
            }
        }
    }

    void LoadInTreeStructure()
    {
        if (nodeTree.videoStructure.Count > 0)
        {
            videoStructure = nodeTree.videoStructure;
        }
    }
}
