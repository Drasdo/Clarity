using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class BranchingStoryController : MonoBehaviour {


    public GameObject TextSelector;
    public GameObject videoSphere;
    public GameObject choiceRotator;
    public GameObject fadeSphere;
    public float fadeTimerLength = 0.5f;

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
    private bool fadingIn = false;
    private bool moveStraightToNextClip = false;
    private bool leftBranchSelected = false;
    private AddBlur addBlur;
    private BACLevel bac;

    private BasicTimer timer;       //for fade
    private BasicTimer timerIn;       //for fadein
    private BasicTimer timerFadeWaiter;  
    private BasicTimer sceneTimer;  //for scene length and playing audio
    private BasicTimer blurTimer;
    private bool mainSceneComplete = false;
    private bool tryPlaySound = false;
    private bool haveSkipped = false;
    private bool haveIncreasedBlur = false;
	private bool readyToPlay = false;
                                                
    private ChangeVideo changeVideo;
    private Color blackAllAlpha;

    Action reloadScene;

    //private int previousNodeDuration = 0;
    private int previousNodeSeekPos = 0;

	void Start () {
        spherePlayer = videoSphere.GetComponent<MediaPlayerCtrl>();
        changeVideo = TextSelector.GetComponent<ChangeVideo>();
        reticle = GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(0).gameObject;
        timer = gameObject.AddComponent<BasicTimer>();
        timerIn = gameObject.AddComponent<BasicTimer>();
        sceneTimer = gameObject.AddComponent<BasicTimer>();
        timerFadeWaiter = gameObject.AddComponent<BasicTimer>();
        blurTimer = gameObject.AddComponent<BasicTimer>();
        addBlur = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AddBlur>();
        bac = GameObject.FindGameObjectWithTag("BACLevel").GetComponent<BACLevel>();

        choiceGameObject = new GameObject[2];
        MediaPlayerCtrl[] temp = changeVideo.GetComponentsInChildren<MediaPlayerCtrl>(); //the assumption is left is first, will need to check
        choiceGameObject[0] = temp[0].gameObject;
        choiceGameObject[1] = temp[1].gameObject; //this is bad, but anyway
        finalScene = false;

        //We need to get the correct tree structure and then load it in
        getCorrectNodeTree();
        LoadInTreeStructure();

        currentNode = videoStructure[0]; ; //0 will be our starting video;
        //reticle.GetComponent<Renderer>().enabled = false;
        assignEnvironmentProperties();
       
        blackAllAlpha = new Color(0, 0, 0, 0);
    }
	
	void Update () {
        //are we assuming the video is playing? lets go with that now
        if (!finalScene)
        {
            if(!moveStraightToNextClip)
            {
                if(spherePlayer.GetSeekPosition() != previousNodeSeekPos)
                {
                    if(notRevealedChoices && spherePlayer.GetSeekPosition() > currentNode.choicesSecondsToShow)
                    {
                        revealTextChoices();
                        notRevealedChoices = false;
                    }
                }
            }
            if (tryPlaySound) //so because playing the audio was being a turd im just gonna keep trying until IT FUCKING PLAYs
            {
                if (!GetComponent<AudioSource>().isPlaying && GetComponent<AudioSource>().clip.loadState == AudioDataLoadState.Loaded)
                {
                    GetComponent<AudioSource>().Play();
                } else
                {
                    tryPlaySound = true;
                }
            }
        }

        float duration = spherePlayer.GetDuration() / 1000f;
        if (sceneTimer.IsTimerFinished())
        {
            if (!moveStraightToNextClip)
            {
                tryPlaySound = true;
            }
            else
            {
                notRevealedChoices = false;
				if (!finalScene || currentNode.earlyVideoEndAt == -1)
                {
                    fadeOut(true);
                }
            }
            sceneTimer.ResetOrCancelTimer();
            mainSceneComplete = true;
            if (finalScene)
            {
                if (currentNode.earlyVideoEndAt != -1)
                {
                    reloadScene = loadMenuScene;
                    addBlur.makeBlink(reloadScene, spherePlayer);
                }
                else
                {
                    fadeOut(true);
                }
            }
#if UNITY_EDITOR
			spherePlayer.Pause();   //breaks thigs if not run when using editor
#endif
#if UNITY_IOS
			if(!addBlur.blinking)
			{
			spherePlayer.Pause();   //breaks thigs if not run when using editor
			}
#endif
        }
        else if (!sceneTimer.IsTimerTicking() && duration > 0f && !mainSceneComplete)
        {
            //check this first, as most important trump (ehhh) card
            if (currentNode.earlyVideoEndAt != -1 && finalScene) //we are finishing early at this time, so do the thing
            {
                sceneTimer.StartTimer(currentNode.earlyVideoEndAt); //start timer of length of video
            }
            else if (currentNode.playbackType == 1) //we need to end early so,
            {
                sceneTimer.StartTimer(currentNode.endEarlyTime);
            }
            else //otherwise set duration of scene to the end
            {
                sceneTimer.StartTimer(duration - 0.5f); //start timer of length of video
            }
            //now would be a good time to jump to where the video playback should start if thats a thing
            if(currentNode.playbackType == 2)
            {
                spherePlayer.SeekTo(currentNode.startLateTime*1000);
            }
        }
        else
        {
            if(currentNode.playbackType == 3 && !haveSkipped)//we need to check where we are up to, then skip ahead
            {
                if (spherePlayer.GetSeekPosition() >= currentNode.skipAtTime*1000)
                {
                    spherePlayer.SeekTo(currentNode.skipToTime*1000);
                    haveSkipped = true;
                }
            }
        }
        if (fadingOut)
        {
            FadeOutMusic();
            if (timer.IsTimerFinished())
            {
				spherePlayer.SetVolume (1.0f);
                if(finalScene)
                {
                    addBlur.updateBlurValues(-1);
                    reticle.GetComponent<GazeLookSelection>().enableReticle();
                    reticle.GetComponent<EnsureReticleIsOn>().makeReticleGreatAgain();
					addBlur.blinking = false;
                    SceneManager.LoadScene(0);

                    fadingIn = true;
                }
                fadingOut = false;
                UpdateTextSelectorForCurrentBranch();
                timer.ResetOrCancelTimer();
                //reset orientation of camera so that we are looking the right way
                GameObject.FindGameObjectWithTag("MainCamera").transform.rotation = Quaternion.identity;
            }
        }
        if(fadingIn)
        {
            if(!timerFadeWaiter.IsTimerTicking())
            {
                timerFadeWaiter.StartTimer(currentNode.waitTime);
            }
			if(timerFadeWaiter.IsTimerFinished())
            {
				spherePlayer.Play ();
                if(!timerIn.IsTimerTicking())
                {
                    timerIn.StartTimer(fadeTimerLength);
                }
                else
                {
                    FadeIn();
                    if(timerIn.IsTimerFinished()) //fading in is over
                    {
                        timerIn.ResetOrCancelTimer();
                        timerFadeWaiter.ResetOrCancelTimer();
                        fadingIn = false;
                        fadeSphere.GetComponent<Renderer>().material.SetColor("_Color", blackAllAlpha);
                        if(SceneManager.GetActiveScene().buildIndex == 0) //if we are menu
                        {
                            Destroy(fadeSphere);
                            Destroy(gameObject);
							addBlur.updateBlurValues(-1);
                        }
                    }
                }
            }
        }
        if (!haveIncreasedBlur && (currentNode.increaseBlurAtTime || currentNode.reduceBlurAtTime))
        {
            if (!blurTimer.IsTimerTicking()) //timer isn't on
            {
                blurTimer.StartTimer(currentNode.TimeForChange);
            }
            else if (blurTimer.IsTimerFinished())
            {
                if (currentNode.increaseBlurAtTime)
                {
                    if (currentNode.doubleBlur)
                    {
                        bac.increaseBAC(true);
                        haveIncreasedBlur = true;
                    }
                    else
                    {
                        bac.increaseBAC();
                        haveIncreasedBlur = true;
                    }
                }
                else
                {
                    bac.decreaseBAC();
                    haveIncreasedBlur = true;
                }
            }
        }
		if(readyToPlay == true)
		{
			if(spherePlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.READY ||
				spherePlayer.GetCurrentState() == MediaPlayerCtrl.MEDIAPLAYER_STATE.PLAYING || 
				SceneManager.GetActiveScene().buildIndex == 0)
			{
				fadingIn = true;
				spherePlayer.Play ();
				readyToPlay = false;
			}
		}
    }

    void FadeOutMusic()
    {
        fadeSphere.GetComponent<Renderer>().material.SetColor("_Color", new Color(0, 0, 0, Mathf.Lerp(1.0f, 0.0f, timer.timeRemaining() / fadeTimerLength)));
    }

    void FadeIn()
    {
        fadeSphere.GetComponent<Renderer>().material.SetColor("_Color", new Color(0, 0, 0, Mathf.Lerp(0.0f, 1.0f, timerIn.timeRemaining() / fadeTimerLength)));
    }

    public void fadeOut(bool leftBranchSel)
    {
        leftBranchSelected = leftBranchSel;
        TextSelector.GetComponent<RevealOnVisible>().ResetVisibility();
        fadingOut = true;
        timer.StartTimer(fadeTimerLength);
    }

    void UpdateTextSelectorForCurrentBranch() //Run as a new video is loaded, so that all the paremeters are set and ready to go.
    {
        if (!notRevealedChoices) //why did I make this variable so confusing? if Not Not Revealed Choices... so if the choices have been revealed
        {
            previousNodeSeekPos = spherePlayer.GetSeekPosition();
            //BEFORE WE DO THAT, LETS MAKE SURE THAT BLUR IS ADDED
            reticle.GetComponent<Renderer>().enabled = false; //disable reticle
            setBlurOptions(leftBranchSelected);
            //we need to check if the element number of choice is -1 because that means we need to end)
            //but what if the choice plays a video but then thats the end choice? need to maybe know when the video is loaded and not do the choice options)
            //before we assign the new current node, we need to check if we should just be ending here
            if (bac.getCurrentBAC() >= 4 /*extreme*/ && currentNode.endToBlack)
            {
                //Dont event bother with loading stuff, Kill it off right now
                reloadScene = loadMenuScene;
                addBlur.makeBlink(reloadScene, spherePlayer);
            }
            else //we need to move to the next video, so do the stuff
            {
                currentNode = (leftBranchSelected) ? videoStructure[currentNode.leftChoiceID[bac.getCurrentBAC()]] : videoStructure[currentNode.rightChoiceID[bac.getCurrentBAC()]];
                //we have the new node
                sceneTimer.ResetOrCancelTimer();
                assignEnvironmentProperties();
                if(bac.getCurrentBAC() >= 4)
                {
                    //but flag final scene of we are at BAC 4
                    finalScene = true;
                }
            }
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
        haveIncreasedBlur = false;

        notRevealedChoices = false;
        spherePlayer.transform.rotation = Quaternion.identity;
        spherePlayer.transform.Rotate(new Vector3(0, currentNode.startingRotationOfVideoDegrees, 0)); //rotate the video to correct orientation if need be. Hopefully this doesn't actually have to be used, but nice to have anyway.
        choiceRotator.transform.rotation = Quaternion.identity;
        choiceRotator.transform.Rotate(new Vector3(0, currentNode.choiceRotationToSummonAt, 0));
        currentNode.toMilliseconds();
        haveSkipped = false;
        if (currentNode.resetBlur)
        {
            addBlur.updateBlurValues(-1);
            //this shouldnt happen 

        }
        if(currentNode.endingVideo)
        {
            finalScene = true;
        }
        notRevealedChoices = true;
        if(currentNode.choicesSecondsToShow <= -0.5f)
        {
            moveStraightToNextClip = true;
        }
        else
        {
            moveStraightToNextClip = false;
        }
        if(currentNode.nodeTitle != "1_1")
        {
			readyToPlay = true;
        }
        //spherePlayer.Load(currentNode.sphereVideo);
        changeVideo.ChangeToNewVideo();
        reticle.GetComponent<Renderer>().enabled = false;
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
                //trigger final fade out
                
            }
        }
        //if
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
        if(bac == null)
        {
            bac = BACLevel.BAC;
        }
        if(currentNode.choiceLeftAddBlur && leftSelected)
        {
            //addBlur.updateBlurValues(addBlur.blurIntensity);

            if(currentNode.doubleBlur)
            {
                bac.increaseBAC(true);
            } else
            {
                bac.increaseBAC();
            }
        }
        else if (currentNode.choiceRightAddBlur && !leftSelected)
        {
            //addBlur.updateBlurValues(addBlur.blurIntensity);
            if (currentNode.doubleBlur)
            {
                bac.increaseBAC(true);
            }
            else
            {
                bac.increaseBAC();
            }
        }
        if(currentNode.reduceBlurRight && !leftSelected)
        {
            bac.decreaseBAC();
            //addBlur.updateBlurValues(-addBlur.blurIntensity);
        }
        else if (currentNode.reduceBlurLeft && leftSelected)
        {
            //addBlur.updateBlurValues(-addBlur.blurIntensity);
            bac.decreaseBAC();
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

    void getCorrectNodeTree()
    {
        GameObject treeStructure = GameObject.FindGameObjectWithTag("NodeTree");
        NodeTree[] NTs = treeStructure.GetComponents<NodeTree>();
        foreach (NodeTree nt in NTs)
        {
            if(nt.structureName == NodeTree.currentTree)
            {
                nodeTree = nt;
            }
        }

    }

    void loadMenuScene()
    {
        fadeSphere.GetComponent<Renderer>().material.SetColor("_Color", new Color(0, 0, 0, 100f));
        fadingIn = true;
        addBlur.updateBlurValues(-1);
        reticle.GetComponent<GazeLookSelection>().enableReticle();
        reticle.GetComponent<EnsureReticleIsOn>().makeReticleGreatAgain();
		addBlur.blinking = false;
        SceneManager.LoadScene(0);

    }
}
