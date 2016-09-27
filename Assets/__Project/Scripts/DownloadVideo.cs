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
    private string localPath;

    private List<string> videos;     //list of videos to load in
    private List<string> videoID;   //the identifier of the video
    private bool downloadingComplete;

    private NodeTree nodeTree;
    private string VideoSizeToDownload;
    private DeviceVideoCapability.MaxVideoSize maxVidSize;

    WWW request;
    byte[] fileData;

    void Start()
    {
        //Add video strings from videoStructure too this list so we can load them in
        nodeTree = GameObject.FindGameObjectWithTag("NodeTree").GetComponent<NodeTree>();
        setVideoSizeString();
        setVideosToDownload();
        //Set our video counter so we know how many videos we have downloaded so far
        currentDownload = 0;
        downloadingComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!downloadingComplete)
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
                    DownloadComplete();
                    currentDownload++;
                    //if there are more videos to download, start the next download
                    if (currentDownload < videos.Count) //check
                    {
                        InitateDownload();
                    } else
                    {
                        downloadingComplete = true;
                        print("Downloading is complete! You can press play now!");
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
            SaveCorrectPath();
        }
        nodeTree.videoStructure[currentDownload].sphereVideo = localPath;
    }

    public void InitateDownload()
    {
        setVideoSizeString();
        setVideosToDownload();
        print("Beginning download of " + videos[currentDownload]);
        print("Downloading to " + Application.persistentDataPath);
        StartCoroutine(downloadStreamingVideo(videos[currentDownload]));
    }

    public string GetProgress()
    {
        if(downloadingComplete)
        {
            return "Downloads Complete";
        }
        else if (request != null)
        {
            int temp = (int)(request.progress * 100);
            return "Downloading Video " + (currentDownload + 1) + " of " + videos.Count + "; " + temp + "% Complete";
        }
        else
        {
            return "Error: Some sort of Error";
        }
    }

    void SaveCorrectPath()
    {
        #if UNITY_EDITOR_WIN
            localPath = "file://" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4";
#elif UNITY_IOS
            localPath = "file://" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4";
#else
            localPath = "file:///" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4";
#endif
    }

    IEnumerator downloadStreamingVideo(string videoPath)
    {
        //Will need to have an identifier number for each video passed in so we know which video we are loading
        localPath = videoPath;
        currentVideo = videoPath;
        FileInfo info = new FileInfo(Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4");
        if (info.Exists == true)
        {
            print("file://" + Application.persistentDataPath + "/" + videoID[currentDownload] + VideoSizeToDownload + ".mp4 exists");
            SaveCorrectPath();
        }

        // Start a download of the given URL
        request = new WWW(localPath);
        yield return request;
    }

    public void setVideosToDownload()
    {
        videos = new List<string>();
        videoID = new List<string>();
        foreach (Node videoNode in nodeTree.videoStructure)
        {
            if (videoNode.SphVidOnlineLoc == "")
            {
                videos.Add(videoNode.sphereVideo);
            }
            else
            {
                videos.Add((videoNode.SphVidOnlineLoc + VideoSizeToDownload + ".mp4")); // todo: CANT DO THIS UNTIL WE SAY TO ACTUALLY DOWNLOAD
            }
            videoID.Add(videoNode.nodeTitle);
        }
    }

    public void setVideoSizeString()
    {
        maxVidSize = nodeTree.GetComponent<DeviceVideoCapability>().currentSelection;
        switch(maxVidSize)
        {
            case DeviceVideoCapability.MaxVideoSize.FOURK:
                VideoSizeToDownload = "4k";
                break;
            case DeviceVideoCapability.MaxVideoSize.QHD:
                VideoSizeToDownload = "qhd";
                break;
            case DeviceVideoCapability.MaxVideoSize.Unset:
                nodeTree.GetComponent<DeviceVideoCapability>().WakeUp();
                setVideoSizeString();
                break;
            default:
                VideoSizeToDownload = "hd";
                break;
        }
    }
}