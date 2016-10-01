using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class BranchingStoryController : MonoBehaviour {

    public GameObject TextSelector;
    public GameObject videoSphere;
    public GameObject choiceRotator;
    public AddBlur addBlur;

    private List<Node> videoStructure = new List<Node>(); 

    private Node currentNode;
    private MediaPlayerCtrl spherePlayer;   //player of the sphere video
    private GameObject[] choiceGameObject; //will point to the video players for each choice, right then left
    private bool notRevealedChoices;        //so we know when the choices has begun revealing so we can only trigger that once
    private bool finalScene;//flag so we know if we need to show choices or not  
    private GameObject reticle; //the reticle, so we can turn it off when not showing options
    private AudioClip tailendClip;
    private NodeTree nodeTree;
    private bool fadingOut = false;
    private bool leftBranchSelected = false;

    private BasicTimer timer;       //for fade
    private BasicTimer sceneTimer;  //for scene length and playing audio
    private bool mainSceneComplete = false;
    private bool tryPlaySound = false;
                                                
    private ChangeVideo changeVideo;

	void Start () {
        spherePlayer = videoSphere.GetComponent<MediaPlayerCtrl>();
        changeVideo = TextSelector.GetComponent<ChangeVideo>();
        reticle = GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(0).gameObject;
        timer = gameObject.AddComponent<BasicTimer>();
        sceneTimer = gameObject.AddComponent<BasicTimer>();

        choiceGameObject = new GameObject[2];
        MediaPlayerCtrl[] temp = changeVideo.GetComponentsInChildren<MediaPlayerCtrl>(); //the assumption is left is first, will need to check
        choiceGameObject[0] = temp[0].gameObject;
        choiceGameObject[1] = temp[1].gameObject; //this is bad, but anyway
        nodeTree = GameObject.FindGameObjectWithTag("NodeTree").GetComponent<NodeTree>();
        finalScene = false;
    
        LoadInTreeStructure();
        currentNode = videoStructure[0]; ; //0 will be our starting video;
        reticle.GetComponent<Renderer>().enabled = false;
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
            float duration = spherePlayer.GetDuration() / 1000f;
            if (!sceneTimer.IsTimerTicking() && duration > 0f && !mainSceneComplete)
            {
                sceneTimer.StartTimer(duration); //start timer of length of video
            } else if(sceneTimer.IsTimerFinished())
            {
                tryPlaySound = true; //this works but only if i breakpoint it? so it needs that tiny bit longer to play?
                sceneTimer.ResetOrCancelTimer();
                mainSceneComplete = true;
            }

            if(tryPlaySound) //so because playing the audio was being a turd im just gonna keep trying until IT FUCKING PLAYs
            {
                if (!GetComponent<AudioSource>().isPlaying) //if we aren't playing TRY PLAYING
                {
                    GetComponent<AudioSource>().Play();
                } else
                {
                    tryPlaySound = true;
                }
            }
        }
        else
        {
            finalSceneChecker();
        }
        if (fadingOut)
        {
            FadeOutMusic();
            if (timer.IsTimerFinished()) //3 second timer because all the fades are 3 seconds
            {
                fadingOut = false;
                UpdateTextSelectorForCurrentBranch();
                timer.ResetOrCancelTimer();
                
            }
        }
    }

    void FadeOutMusic()
    {
        GetComponent<AudioSource>().volume = Mathf.Lerp(0f, 1.0f, timer.timeRemaining() / 2.8f);
    }

    public void fadeOut(bool leftBranchSel)
    {
        changeVideo.videoToChangeTo = currentNode.SphVidFadeOutLocal;
        changeVideo.ChangeToNewVideo();
        leftBranchSelected = leftBranchSel;
        fadingOut = true;
        TextSelector.GetComponent<RevealOnVisible>().ResetVisibility();
        timer.StartTimer(2.8f);
    }

    void UpdateTextSelectorForCurrentBranch() //Run as a new video is loaded, so that all the paremeters are set and ready to go.
    {
        if (!notRevealedChoices) //why did I make this variable so confusing? if Not Not Revealed Choices... so if the choices have been revealed
        {
            //BEFORE WE DO THAT, LETS MAKE SURE THAT BLUR IS ADDED
            reticle.GetComponent<Renderer>().enabled = false; //disable reticle
            setBlurOptions(leftBranchSelected);
            //we need to check if the element number of choice is -1 because that means we need to end)
            //but what if the choice plays a video but then thats the end choice? need to maybe know when the video is loaded and not do the choice options)
            currentNode = (leftBranchSelected) ? videoStructure[currentNode.leftChoiceElementNumber] : videoStructure[currentNode.rightChoiceElementNumber];

            assignEnvironmentProperties();
            //is there anything else that needs to be reset?
        }
    }

    void assignEnvironmentProperties()
    {
        changeVideo.videoToChangeTo = currentNode.sphereVideo;
        setupTextSelectOptions();
        tailendClip = currentNode.TailendAudio;
        GetComponent<AudioSource>().clip = tailendClip;
        mainSceneComplete = false;
        tryPlaySound = false;

        changeVideo.ChangeToNewVideo();
        notRevealedChoices = false;
        spherePlayer.transform.Rotate(new Vector3(0, currentNode.startingRotationOfVideoDegrees, 0)); //rotate the video to correct orientation if need be. Hopefully this doesn't actually have to be used, but nice to have anyway.
        choiceRotator.transform.Rotate(new Vector3(0, currentNode.choiceRotationToSummonAt, 0));
        currentNode.toMilliseconds();
        if(currentNode.resetBlur)
        {
            addBlur.updateBlurValues(-1);
        }
        if(currentNode.choicesSecondsToShow == 0)
        {
            finalScene = true;
        }
        notRevealedChoices = true;
    }

    void revealTextChoices()
    {
        TextSelector.GetComponent<RevealOnVisible>().beginReveal();
        reticle.GetComponent<Renderer>().enabled = true;
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

    void setBlurOptions(bool leftSelected)
    {
        if(currentNode.choiceLeftAddBlur && leftSelected)
        {
            addBlur.updateBlurValues(addBlur.blurIntensity);
        }
        else if (currentNode.choiceRightAddBlur && !leftSelected)
        {
            addBlur.updateBlurValues(addBlur.blurIntensity);
        }
    }

    void setupTextSelectOptions()
    {
        if (currentNode.choiceIsVideo)
        {
            choiceGameObject[0].transform.GetChild(0).GetComponent<GUIElementReaction>().isMovieSprite = true; //Movie sprite, so lets tell the gui that
            choiceGameObject[1].transform.GetChild(0).GetComponent<GUIElementReaction>().isMovieSprite = true;
            choiceGameObject[0].GetComponent<MediaPlayerCtrl>().enabled = true;
            choiceGameObject[1].GetComponent<MediaPlayerCtrl>().enabled = true;
            choiceGameObject[0].GetComponent<MediaPlayerCtrl>().m_strFileName = currentNode.choiceVideoRight; //Right
            choiceGameObject[1].GetComponent<MediaPlayerCtrl>().m_strFileName = currentNode.choiceVideoLeft; //Left
            //disable GUI renderers
            choiceGameObject[0].transform.GetChild(0).gameObject.SetActive(false); //no longer playing movie, so disable
            choiceGameObject[1].transform.GetChild(0).gameObject.SetActive(false);

        }
        else //choices are icons, so we need to somehow set that up
        {
            choiceGameObject[0].GetComponent<MediaPlayerCtrl>().enabled = false; //disable the video player cause fuck that shit
            choiceGameObject[1].GetComponent<MediaPlayerCtrl>().enabled = false;
            choiceGameObject[0].GetComponent<Renderer>().enabled = false; //disable the video player cause fuck that shit
            choiceGameObject[1].GetComponent<Renderer>().enabled = false;
            choiceGameObject[0].transform.GetChild(0).GetComponent<GUIElementReaction>().isMovieSprite = false; //no longer playing movie, so disable
            choiceGameObject[1].transform.GetChild(0).GetComponent<GUIElementReaction>().isMovieSprite = false;
            choiceGameObject[0].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = currentNode.choiceVideoRight; //set the text for the choice
            choiceGameObject[1].transform.GetChild(0).GetChild(0).GetComponent<TextMesh>().text = currentNode.choiceVideoLeft; //this is such bad code i am so sorry
        }
    }
}
