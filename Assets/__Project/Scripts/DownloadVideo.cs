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
    public bool videosComplete;     //so we know when the video files are finished.
    public bool downloadingComplete;

    private NodeTree nodeTree;
    private string VideoSizeToDownload;
    private DeviceVideoCapability.MaxVideoSize maxVidSize;
    private float loadedVideoCounter = 0f;

    WWW request;
    WWW requestSound;
    WWW www;
    byte[] fileData;

    void Start()
    {
        //Add video strings from videoStructure too this list so we can load them in
        nodeTree = GameObject.FindGameObjectWithTag("NodeTree").GetComponent<NodeTree>();
        setVideoSizeString();
        setVideosToDownload();
        setSoundsToDownload();

        //Set our video counter so we know how many videos we have downloaded so far
        currentDownload = 0;
        currentSoundDownload = 0;
        downloadingComplete = false;
        videosComplete = false;

        //when we start we should look in the file structure and see if there is anything already in there, and have em ready to go if we do
        foreach (string vid in videos)
        {
            checkforDownloadedVideo(vid, true);
        }
        foreach (string sou in sounds)
        {
            StartCoroutine(checkforDownloadedSounds(sou, true));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!downloadingComplete)
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
                            InitateSoundDownload(); //start our first sound download
                        }
                    }
                }
            }
            else if (requestSound != null)
            {
                if (requestSound.isDone)
                {
                    //do sounds
                    DownloadingSoundComplete();
                    currentSoundDownload++;
                    //if there are more sounds to download, start the next download
                    if (currentSoundDownload < sounds.Count) //check
                    {
                        InitateSoundDownload();
                    }
                    else
                    {
                        downloadingComplete = true;
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
            File.WriteAllBytes(Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4", fileData);
            print("Saving mp4 to " + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4");
            SaveCorrectPath(".mp4");
        }
        if (currentDownload % 2 == 0) //if we are on a main video
        {
            nodeTree.videoStructure[(int)currentDownload / 2].sphereVideo = localPath;
        }
        else
        {
            nodeTree.videoStructure[(int)currentDownload / 2].SphVidFadeOutLocal = localPath;
        }
    }

    void DownloadingSoundComplete()
    {
        fileData = requestSound.bytes;
        Size = fileData.Length;
        if (fileData.Length > 0)
        {
            File.WriteAllBytes(Application.persistentDataPath + "/" + soundID[currentSoundDownload] + ".wav", fileData);
            SaveCorrectPath(".wav");
        }
        nodeTree.videoStructure[currentSoundDownload].TailendAudio = requestSound.audioClip;
    }

    public void InitateDownload()
    {
        if (!videosComplete)
        {
            setVideoSizeString();
            setVideosToDownload();
            print("Beginning download of " + videos[currentDownload]);
            print("Downloading to " + Application.persistentDataPath);
            StartCoroutine(downloadStreamingVideo(videos[currentDownload]));
        }
        else if (!downloadingComplete)
        {
            InitateSoundDownload();
        }
    }

    public void InitateSoundDownload()
    {
        setVideoSizeString();
        setSoundsToDownload();
        StartCoroutine(downloadAudioFile(sounds[currentSoundDownload]));
    }

    public string GetProgress()
    {
        if (downloadingComplete)
        {
            return "Videos Ready";
        }
        else if (request != null)
        {
            if (currentDownload % 2 == 0)
            {
                int temp = (int)(request.progress * 100);
                return "Downloading Video " + (currentDownload/2 + 1) + " of " + videos.Count/2 + "; " + temp + "% Complete";
            }
            else
            {
                return "Downloading Video " + (currentDownload/2 + 1) + " of " + videos.Count / 2 + "; Saving File";
            }
        }
        else
        {
            if (currentDownload + 1 > videos.Count)
            {
                return "Finalising Downloads";
            }
            else
            {
                return " ";
            }
        }
    }

    void SaveCorrectPath(string fileFormat)
    {
        if (fileFormat == ".mp4")
        {
#if UNITY_EDITOR_WIN
            localPath = "file://" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + fileFormat;
#elif UNITY_IOS
                 localPath = "file://" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + fileFormat;
#else
                localPath = "file:///" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + fileFormat;
#endif
        }
        else if (fileFormat == ".wav")
        {
#if UNITY_EDITOR_WIN
            localPath = "file://" + Application.persistentDataPath + "/" + soundID[currentSoundDownload] + fileFormat;
#elif UNITY_IOS
                localPath = "file://" + Application.persistentDataPath + "/" + soundID[currentSoundDownload] + fileFormat;
#else
                localPath = "file:///" + Application.persistentDataPath + "/" + soundID[currentSoundDownload] + fileFormat;
#endif
        }
    }

    void checkforDownloadedVideo(string videoPath, bool checking)
    {
        //Will need to have an identifier number for each video passed in so we know which video we are loading
        localPath = videoPath;
        currentVideo = videoPath;
        FileInfo info;
        info = new FileInfo(Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4");
        if (info.Exists == true)
        {
            print("file://" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4 exists");
            SaveCorrectPath(".mp4");
            if (checking)
            {
                if (currentDownload % 2 == 0) //if we are on a main video
                {
                    nodeTree.videoStructure[(int)currentDownload / 2].sphereVideo = localPath;
                }
                else //a fade video
                {
                    nodeTree.videoStructure[(int)currentDownload / 2].SphVidFadeOutLocal = localPath;
                }
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
            return;
        }
    }

    IEnumerator checkforDownloadedSounds(string soundPath, bool checking)
    {
        //Will need to have an identifier number for each video passed in so we know which video we are loading
        localPath = soundPath;
        currentVideo = soundPath;
        FileInfo info;
        info = new FileInfo(Application.persistentDataPath + "/" + soundID[currentSoundDownload] + ".wav");
        if (info.Exists == true)
        {
            SaveCorrectPath(".wav");
            if (checking)
            {
                //so the file exists... but we still need to turn it into a audioclip
                yield return StartCoroutine(LoadClip(localPath));
                //currentSoundDownload++;
                //return;
                currentSoundDownload++;
            }
            if (currentSoundDownload >= sounds.Count) //check
            {
                downloadingComplete = true;
                print("Everything downloaded");
            }
        }
        else //if the file doesn't exist we want to stop?
        {
            //return;
        }
    }

    IEnumerator downloadStreamingVideo(string path)
    {
        checkforDownloadedVideo(path, false);

        // Start a download of the given URL
        request = new WWW(localPath);
        yield return request;
    }

    IEnumerator downloadAudioFile(string path)
    {
        checkforDownloadedSounds(path, false);

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
            if (videoNode.SphVidOnlineLoc == "")
            {
                videos.Add(videoNode.sphereVideo + ".mp4");
                videos.Add(videoNode.sphereVideo + ".mp4");
            }
            else
            {
                videos.Add(videoNode.SphVidOnlineLoc + VideoSizeToDownload + ".mp4");
                videos.Add(videoNode.SphVidOnlineLoc + VideoSizeToDownload + "fade" + ".mp4");
            }
            videoID.Add(videoNode.nodeTitle);
            videoID.Add(videoNode.nodeTitle + "fade");
        }
    }

    public void setSoundsToDownload()
    {
        sounds = new List<string>();
        soundID = new List<string>();
        foreach (Node videoNode in nodeTree.videoStructure)
        {
            sounds.Add(videoNode.SphVidOnlineLoc + "Tailend" + ".wav");
            soundID.Add(videoNode.nodeTitle + "Tailend");
        }
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
        foreach (string vid in videoID)
        {
            FileInfo info;
            info = new FileInfo(Application.persistentDataPath + "/" + vid + VideoSizeToDownload + ".mp4");
            if (info.Exists == true)
            {
                info.Delete();
            }
            //all files deleted, lets reset all these values to their starting state
            currentDownload = 0;
            downloadingComplete = false;
            request = null;
        }
        foreach (string sound in soundID)
        {
            FileInfo info;
            info = new FileInfo(Application.persistentDataPath + "/" + sound + ".wav");
            if (info.Exists == true)
            {
                info.Delete();
            }
            //all files deleted, lets reset all these values to their starting state
            currentDownload = 0;
            currentSoundDownload = 0;
            downloadingComplete = false;
            videosComplete = false;
            requestSound = null;
        }
    }

    IEnumerator LoadClip(string path)
    {
        www = new WWW(path);
        yield return www;
        AudioClip clip = www.GetAudioClip(false, false, AudioType.WAV);
        nodeTree.videoStructure[currentSoundDownload].TailendAudio = clip;
        nodeTree.videoStructure[currentSoundDownload].TailendAudio.name = soundID[currentSoundDownload];
    }
}