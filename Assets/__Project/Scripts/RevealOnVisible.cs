using UnityEngine;
using System.Collections;

public class RevealOnVisible : MonoBehaviour
{
    public float speed = 10f;
    public bool onlyMe = false;

    Vector3 startSize;
    Vector3 currentSize;
    Vector3 endSize;
    bool shouldReveal = false;
    float startTime;
    float endTime;
    float t;

    // Use this for initialization
    void Start()
    {
        startSize = Vector3.zero;
        endSize = transform.localScale;
        startTime = 0.0f;
        t = 0;

        for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
        {
            if (!onlyMe)
            {
                Transform child = gameObject.transform.GetChild(childIndex);
                child.gameObject.AddComponent<RevealOnVisible>();
            }
        }
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldReveal)
        {
            if (startTime.Equals(0.0f)) //should only happen first time
            {
                startTime = Time.time;
                endTime = startTime + speed;
                makeChildrenActive();
            }
            smallToBigReveal();
        }
    }

    void smallToBigReveal()
    {
        t += Time.deltaTime;
        currentSize = Vector3.Lerp(startSize, endSize, Mathf.Sin(t * Mathf.PI / speed)); //current size
        transform.localScale = currentSize;

        if (Vector3Compare.V3Equal(currentSize, endSize, 0.000001f))
        {
            shouldReveal = false;
            startTime = 0.0f;
            endTime = 0.0f;
            t = 0.0f;
            transform.localScale = endSize;
        }
    }

    public void beginReveal()
    {
        gameObject.SetActive(true);
        if (!onlyMe)
        {
            if (transform.childCount > 0)
            {
                for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
                {
                    gameObject.transform.GetChild(childIndex).GetComponent<RevealOnVisible>().beginReveal();
                }
            }
        }
        shouldReveal = true;
    }

    void makeChildrenActive()
    {
        if (!onlyMe)
        {
            for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                gameObject.transform.GetChild(childIndex).GetComponent<RevealOnVisible>().beginReveal();
            }
        }
    }

    public void ResetVisibility()
    {
        startSize = Vector3.zero;
        endSize = transform.localScale;
        startTime = 0.0f;
        if (!onlyMe)
        {
            for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                gameObject.transform.GetChild(childIndex).GetComponent<RevealOnVisible>().ResetVisibility();
            }
        }
        gameObject.SetActive(false);

        transform.localScale = startSize; //reset the size after it is hidden so that the player doesn't see
    }
}