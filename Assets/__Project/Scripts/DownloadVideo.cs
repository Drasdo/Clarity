using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;

public class DownloadVideo : MonoBehaviour
{
    public string currentVideo;
    public WebClient webClient;
    public float Size = 0.0f;
    private int currentDownload;
    private int currentSoundDownload;
    private string localPath;

    private List<string> videos;     //list of videos to load in
    private List<string> videoID;   //the identifier of the video
    private List<string> sounds;  //list of the wav files to download as well
    private List<string> soundID;
    private List<string> downloadedFiles; //a list of downloaded files so we can easily delete
    public bool videosComplete;     //so we know when the video files are finished.
    public bool soundsComplete;
    public bool downloadingComplete;

    private bool areWeChecking = true;
    private int checkingNumber = 0;
    private int timesRound = 0;

    private NodeTree nodeTree;
    private string VideoSizeToDownload;
    private DeviceVideoCapability.MaxVideoSize maxVidSize;
    private float loadedVideoCounter = 0f;
    private int SoundClipsLoaded = 0;

    private int skipLogger = 0; //to keep track of how many audio files we dont need to download, so we can save audio files to their correct node.
    private int skipLoggerTotal = 0; //value to hold total when we start counting down;

    WWW request;
    WWW requestSound;
    WWW www;
    byte[] fileData;

    void Start()
    {
        downloadedFiles = new List<string>();
        //Set our video counter so we know how many videos we have downloaded so far
        currentDownload = 0;
        currentSoundDownload = 0;
        downloadingComplete = false;
        videosComplete = false;
        soundsComplete = false;
    }

    void Update()
    {
        if(videosComplete && soundsComplete)
        {
            downloadingComplete = true;//relax, dont do it, when you wanna pursue it
            nodeTree.everythingReady = true;
        }
        else
        {
            if (request != null)
            {
                print("Downloaded: " + request.progress * 100 + " percent");
                if (request.isDone)
                {
                    if (!string.IsNullOrEmpty(request.error))
                    {
                        print("There was a Error: " + request.error);
                    }
                    if (!videosComplete)
                    {
                        DownloadComplete();
                        currentDownload++;
                        //if there are more videos to download, start the next download
                        if (currentDownload < videos.Count) //check
                        {
                            InitateDownload();
                        }
                        else
                        {
                            videosComplete = true;
                            print("Videos downloaded");
                            request = null;
                            InitateSoundDownload(areWeChecking); //start our first sound download
                        }
                    }
                }
            }
            else if (requestSound != null)
            {
                if (requestSound.isDone)
                {
                    //do sounds
                    DownloadingSoundComplete(requestSound);
                    currentSoundDownload++;
                    //if there are more sounds to download, start the next download
                    if (currentSoundDownload < sounds.Count) //check
                    {
                        InitateSoundDownload(areWeChecking);
                    }
                    else
                    {
                        soundsComplete = true;
                        requestSound = null;
                        currentSoundDownload = sounds.Count;
                        print("Everything DOwnloased");
                    }
                }
            }
        }
    }

    void DownloadComplete()
    {
        fileData = request.bytes;
        Size = fileData.Length;
        if (fileData.Length > 0)
        {
            localPath = correctLocalPath(".mp4", videoID[currentDownload]);
            File.WriteAllBytes(localPath, fileData);
            addFileToListOfDownloadedFiles(localPath);
            SaveCorrectPath(localPath);
        }
        nodeTree.videoStructure[currentDownload].sphereVideo = localPath;
    }

    void DownloadingSoundComplete(WWW process)
    {
        fileData = process.bytes;
        Size = fileData.Length;
        if (fileData.Length > 0)
        {
            localPath = correctLocalPath(".wav", soundID[currentSoundDownload]);
            File.WriteAllBytes(localPath, fileData);
            addFileToListOfDownloadedFiles(localPath);
            SaveCorrectPath(localPath);
        }
        while(nodeTree.videoStructure[currentSoundDownload+(skipLoggerTotal-skipLogger)].nodeTitle + "tailend" != soundID[currentSoundDownload])
        {
            skipLogger--;
        }
        nodeTree.videoStructure[currentSoundDownload + (skipLoggerTotal - skipLogger)].TailendAudio = process.audioClip;
        nodeTree.videoStructure[currentSoundDownload + (skipLoggerTotal - skipLogger)].TailendAudio.name = soundID[currentSoundDownload];
    }

    public void InitateDownload()
    {
        if (!videosComplete)
        {
            StartCoroutine(downloadStreamingVideo(videos[currentDownload]));
        }
        else if (!soundsComplete)
        {
            InitateSoundDownload(false);
        }
    }

    public void InitateSoundDownload(bool checking)
    {
        setVideoSizeString();
        setSoundsToDownload();
        StartCoroutine(downloadAudioFile(sounds[currentSoundDownload], checking));
    }

    public string GetProgress()
    {
        if(videos != null)
        {
            if(downloadingComplete)
            {
                return "Videos Ready";
            }
            else if(request != null)
            {
                int temp = (int)(request.progress * 100);
                return "Downloading Video " + (currentDownload) + " of " + videos.Count + "; " + temp + "% Complete";
            }
            else
            {
                if(!soundsComplete)
                {
                    return "Checking for Files";
                }
            }
        }
        else
        {
            return " ";
        }
        if(timesRound >= 1)
        {
            return " ";
        }
        else
        {
            return " ";
        }
    }

    void SaveCorrectPath(string appendTo)
    {
#if UNITY_EDITOR_WIN
            localPath = "file://" + appendTo;
#elif UNITY_IOS
                 localPath = "file://" + appendTo;
#else
                localPath = "file:///" + appendTo;
#endif
    }

    void checkforDownloadedVideo(string videoPath, bool checking)
    {
        //Will need to have an identifier number for each video passed in so we know which video we are loading
        currentVideo = videoPath;
        FileInfo info;

        localPath = correctLocalPath(".mp4", videoID[currentDownload]);
        info = new FileInfo(localPath);
        if (info.Exists == true)
        {
            addFileToListOfDownloadedFiles(localPath);
            SaveCorrectPath(localPath);
            if (checking)
            {
                nodeTree.videoStructure[currentDownload].sphereVideo = localPath;
            }
            currentDownload++;
            loadedVideoCounter++;
            if (loadedVideoCounter >= videos.Count) //check
            {
                videosComplete = true;
                print("Videos Complete, need to download sounds too");
            }
        }
        else //if the file doesn't exist we want to stop?
        {
            localPath = videoPath;
        }
    }

    IEnumerator checkforDownloadedSounds(string soundPath, bool checking)
    {
        //Will need to have an identifier number for each video passed in so we know which video we are loading
        currentVideo = soundPath;
        FileInfo info;

        localPath = correctLocalPath(".wav", videoID[currentSoundDownload]);
        info = new FileInfo(localPath);
        if (info.Exists == true)
        {
            SaveCorrectPath(localPath);
            if (checking)
            {
                //so the file exists... but we still need to turn it into a audioclip
                yield return StartCoroutine(LoadClip(localPath, currentSoundDownload++));
                //return;
            }
            if (currentSoundDownload >= sounds.Count) //check
            {
                soundsComplete = true;
                print("Everything downloaded");
            }
        }
        else
        {
            localPath = soundPath;
        }
    }

    IEnumerator downloadStreamingVideo(string path)
    {
        checkforDownloadedVideo(path, false);

        // Start a download of the given URL
        request = new WWW(localPath);
        yield return request;
    }

    IEnumerator downloadAudioFile(string path, bool checking)
    {
        checkforDownloadedSounds(path, checking);
        if(checking)
        {
            checkingNumber++;
        }
        // Start a download of the given URL
        requestSound = new WWW(path);
        yield return requestSound;
    }

    public void setVideosToDownload()
    {
        videos = new List<string>();
        videoID = new List<string>();
        foreach (Node videoNode in nodeTree.videoStructure)
        {
            videos.Add(videoNode.SphVidOnlineLoc + VideoSizeToDownload + ".mp4");
            videoID.Add(videoNode.nodeTitle);
        }
    }

    public void setSoundsToDownload()
    {
        sounds = new List<string>();
        soundID = new List<string>();
        foreach (Node videoNode in nodeTree.videoStructure)
        {
            if (videoNode.isThereEndAudio)
            {
                sounds.Add(videoNode.SphVidOnlineLoc + "tailend" + ".wav");
                soundID.Add(videoNode.nodeTitle + "tailend");
            } else
            {
                skipLogger++;
            }
        }
        skipLoggerTotal = skipLogger;
    }

    public void setVideoSizeString()
    {
        maxVidSize = nodeTree.GetComponent<DeviceVideoCapability>().currentSelection;
        switch (maxVidSize)
        {
            case DeviceVideoCapability.MaxVideoSize.FOURK:
                VideoSizeToDownload = "4K";
                break;
            case DeviceVideoCapability.MaxVideoSize.QHD:
                VideoSizeToDownload = "2K";
                break;
            case DeviceVideoCapability.MaxVideoSize.Unset:
                nodeTree.GetComponent<DeviceVideoCapability>().WakeUp();
                setVideoSizeString();
                break;
            default:
                VideoSizeToDownload = "HD";
                break;
        }
    }

    public void deleteFiles()
    {
        foreach (string file in downloadedFiles)
        {
            FileInfo info;
            info = new FileInfo(file);
            if (info.Exists == true)
            {
                info.Delete();
            }  
        }
        //all files deleted, lets reset all these values to their starting state
        currentDownload = 0;
        videosComplete = false;
        request = null;
        currentDownload = 0;
        currentSoundDownload = 0;
        soundsComplete = false;
        requestSound = null;
        downloadedFiles = new List<string>();
    }

    IEnumerator LoadClip(string path, int currentVal)
    {
        www = new WWW(path);
        yield return www;
        //AudioClip clip = www.GetAudioClip(false, false, AudioType.WAV);
        byte[] fileData = www.bytes;
        Size = fileData.Length;
        if (fileData.Length > 0)
        {
            localPath = correctLocalPath(".wav", soundID[currentVal]);
            File.WriteAllBytes(localPath, fileData);
            addFileToListOfDownloadedFiles(localPath);
            SaveCorrectPath(localPath);
        }
        nodeTree.videoStructure[currentVal].TailendAudio = www.audioClip;
        nodeTree.videoStructure[currentVal].TailendAudio.name = soundID[currentVal];
    }

    void checkForLocalFiles()
    {
        //when we start we should look in the file structure and see if there is anything already in there, and have em ready to go if we do
        foreach (string vid in videos)
        {
            checkforDownloadedVideo(vid, true);
        }
        InitateSoundDownload(areWeChecking);
        localPath = "";
        //StartCoroutine(checkforDownloadedSounds(sounds[currentSoundDownload], true));
    }

    public void assignNodeTree(NodeTree newNodeTree)
    {
        if (nodeTree != newNodeTree)
        {
                Start(); //reset values like downloads and stuff because we might have fresh bacon to download
                nodeTree = newNodeTree;
            if(!nodeTree.everythingReady)
            {
                setVideoSizeString();
                setVideosToDownload();
                setSoundsToDownload();
                checkForLocalFiles();
            }
            else
            {
                downloadingComplete = true;
                soundsComplete = true;
                videosComplete = true;
            }
        }
    }

    public void changeQuality()
    {
        setVideoSizeString();
        setVideosToDownload();
    }

    string correctLocalPath(string fileFormat, string fileID)
    {
        if(fileFormat == ".mp4")
        {
            return Application.persistentDataPath + "/" + fileID + nodeTree.structureName + VideoSizeToDownload + ".mp4";
        }
        else if(fileFormat == ".wav")
        {
            return Application.persistentDataPath + "/" + fileID + nodeTree.structureName + ".wav";
        }
        else
        {
            print("Error in creating localpath");
            return "There has been an Error";
        }
    }

    void addFileToListOfDownloadedFiles(string location)
    {
        if(!downloadedFiles.Contains(location))
        {
            downloadedFiles.Add(location);
        }
    }
}