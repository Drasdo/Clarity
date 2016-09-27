using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    public void OnClick()
    {
        //begin download
       // if(something)
        //change to name of scene when not a template
        //if files are downloaded and ready to go, or if streaming is selected:
        SceneManager.LoadScene(1);
    }
}
