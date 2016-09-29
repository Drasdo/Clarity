using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DontDestroyOnLoad : MonoBehaviour {

    float duration = 0.75f;
    float timer = 0.0f;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            //i dunno, wait a second then delete?
            RemoveMe();
        }
    }

    void RemoveMe()
    {
        timer += Time.deltaTime;
        if(timer / duration > duration)
        {
            Destroy(gameObject);
        }
    }
}
