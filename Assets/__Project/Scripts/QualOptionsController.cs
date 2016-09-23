using UnityEngine;
using System.Collections;

public class QualOptionsController : MonoBehaviour {

    public DeviceVideoCapability DVC;
    public GameObject fourk;
    public GameObject qhd;
    public GameObject hd;

    // Use this for initialization
    void Start () {
        if (DVC.deviceMax == DeviceVideoCapability.MaxVideoSize.Unset)
        {
            DVC.WakeUp();
        }
        if (DVC.deviceMax == DeviceVideoCapability.MaxVideoSize.HD)
        {
            Destroy(fourk);
            Destroy(qhd);
            hd.transform.localPosition = new Vector3(0, 0, 0);
        }
        else if (DVC.deviceMax == DeviceVideoCapability.MaxVideoSize.QHD)
        {
            Destroy(fourk);
            hd.transform.localPosition = new Vector3(0.75f, 0, 0);
            qhd.transform.localPosition = new Vector3(-0.75f, 0, 0);
        }
        //the default is all 3, so no need to do anything
    }
}
