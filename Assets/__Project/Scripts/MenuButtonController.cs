using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButtonController : MonoBehaviour {

    private GUIElementReaction thisGUIEl;

    void Start()
    {
        thisGUIEl = GetComponent<GUIElementReaction>();
    }

    public void OnClick()
    {
        if (thisGUIEl.isEnabled)
        {
            SceneManager.LoadScene(1);
        }
    }
}
