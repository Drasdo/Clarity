using UnityEngine;
using UnityEngine.iOS;
using System.Collections;

public class DeviceVideoCapability : MonoBehaviour {

    public enum MaxVideoSize { Unset, HD, QHD, FOURK };
    public MaxVideoSize deviceMax = MaxVideoSize.Unset;
    public MaxVideoSize currentSelection = MaxVideoSize.Unset;

    // Use this for initialization
    void Start () {
#if UNITY_IOS
        iPhoneQuality();
#elif UNITY_EDITOR_WIN
        deviceMax = MaxVideoSize.FOURK;
#elif UNITY_ANDROID  //PC where im testing, so...
        AndroidQuality(); 
#else   //a catch just in case, go for lowest quality so we know it works
        deviceMax = MaxVideoSize.HD;
#endif
        currentSelection = deviceMax;
    }

    // Update is called once per frame
    void Update () {

	}

    void iPhoneQuality()
    {
#if UNITY_IOS
        DeviceGeneration iOSGen = Device.generation;
        string deviceMod = SystemInfo.deviceModel; //might be able to catch iphone 7 without updating

        if (Debug.isDebugBuild)
        {
            Debug.Log("iPhone.generation     : " + Device.generation);
            Debug.Log("SystemInfo.deviceType : " + SystemInfo.deviceType);
            Debug.Log("SystemInfo.deviceModel: " + SystemInfo.deviceModel);
        }
        switch(iOSGen)
        {
            case DeviceGeneration.iPhone3GS:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone4:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone4S:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone5:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone5S:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone6:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone6Plus:
                deviceMax = MaxVideoSize.QHD; //TODO: check
                break;
            case DeviceGeneration.iPhone6S:
                deviceMax = MaxVideoSize.HD;
                break;
            case DeviceGeneration.iPhone6SPlus:
                deviceMax = MaxVideoSize.FOURK;
                break;
          case (DeviceGeneration)28: //SHOULD BE 7???
                deviceMax = MaxVideoSize.FOURK;
                break;
            case (DeviceGeneration)29: //should be 7 plus. Possible its not though. may need to update unity *shudders*
                deviceMax = MaxVideoSize.FOURK;
                break;                                      
            default:
                //check to see if maybe iphone 7?
                if(deviceMod == "iPhone9,1")
                {
                    //iphone 7
                    deviceMax = MaxVideoSize.FOURK;
                }
                else if(deviceMod == "iPhone9,2")
                {
                    //iphone 7 plus
                    deviceMax = MaxVideoSize.FOURK;
                }
                else
                {
                    //just in case we missed something,
                    deviceMax = MaxVideoSize.FOURK;
                }
                break;
        }
#endif
    }

    void AndroidQuality()
    {
        int H = Screen.currentResolution.height;
        if(H >= 2160)
        {
            deviceMax = MaxVideoSize.FOURK;
        } else if (H >= 1440)
        {
            deviceMax = MaxVideoSize.QHD;
        } else
        {
            deviceMax = MaxVideoSize.HD;
        }
    }

    public void WakeUp()
    {
        Start();
    }

    public void SetCurrentQuality(int value)
    {
        currentSelection = (MaxVideoSize)value;
    }
}
