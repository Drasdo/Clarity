using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIElementReaction : MonoBehaviour {

    private SpriteRenderer ren;
    public string setName = "";
    public Sprite normal;
    public Sprite hover;
    public Sprite select;
    public Sprite selectHover;
    public Sprite off;
    public bool isMovieSprite = false;

    public bool moveToPositionOnSelect = false;
    public Vector3 moveToPosition;
    public float moveToScaler = 1;
    public bool triggerNewUI = false;
    public GameObject newUI;

    public bool isEnabled = true;
    public bool disableUntilDownloaded = false;


    private bool moving;
    private Vector3 startingPosition;
    private Vector3 selectedPosition;
    private Vector3 highlightPosition;
    private Vector3 startingScale;
    private Vector3 destination;
    private Vector3 currentMoveToScaler;
    private float speed = 5.0f;

    private DownloadVideo DV;

    private GazeLookSelection gazeLookHandler;

    [HideInInspector]
    public bool isCurrentSelection; //only public for access to other scripts, shouldn't be touched in editor
    private List<GUIElementReaction> selectionSet = new List<GUIElementReaction>();

    private float t;

    void Start()
    {
        if (!isMovieSprite)
        {
            ren = GetComponentInChildren<SpriteRenderer>();
        }
        moving = false;
        UpdatePositions();
        UpdateScaler(1.0f);
        gazeLookHandler = GameObject.Find("GvrReticle").GetComponent<GazeLookSelection>();
        if (disableUntilDownloaded)
        {
            DV = GameObject.Find("VideoDownloader").GetComponent<DownloadVideo>();
            updateRenderer(off);
            isEnabled = false;
        }

        //get each gameobject in this set so we know who to switch off if we need to
        foreach (Transform child in transform.parent.gameObject.GetComponentsInChildren<Transform>())
        {
            GUIElementReaction temp = child.GetComponent<GUIElementReaction>();
            if (temp != null) //script exists
            {
                int test1 = temp.GetInstanceID();
                int test2 = this.GetComponent<GUIElementReaction>().GetInstanceID();
                if (temp.GetInstanceID() != this.GetComponent<GUIElementReaction>().GetInstanceID())
                {
                    //need to check if its THIS SCRIPT
                    if (temp.setName == setName)
                    {
                        selectionSet.Add(temp);
                    }
                }
            }
        }
               
    }

    void Update()
    {
        if (enabled) //i dont think this is neccessary? why is it here?
        {
            if (moving)
            {
                MoveTowardsDestination();
            }
        }
        if (disableUntilDownloaded)
        {
            if (!DV.downloadingComplete)
            {
                isEnabled = false;
                updateRenderer(off);
            }
            else
            {
                enableWhenDownloadsComplete();
            }
        }

    }

    public void OnHover()
    {
        if (isEnabled)
        {
            updateRenderer(hover);
        }
        if (!moving)
        {
            destination = highlightPosition;
            UpdateScaler(1.0f);
        }
        moving = true;
        gazeLookHandler.SetGazedAt(true, gameObject);
    }

    public void OnExit()
    {
        if (isEnabled)
        {
            if (isCurrentSelection)
            {
                updateRenderer(select);
            }
            else
            {
                updateRenderer(normal);
            }
        }
        if (!moving)
        {
            if (moveToPositionOnSelect && isCurrentSelection)
            {
                destination = moveToPosition;
            }
            else
            {
                destination = startingPosition;
            }
        }
        moving = true;
        gazeLookHandler.SetGazedAt(false, gameObject);

    }

    public void OnClick()
    {
        if (isEnabled) //this is if i've enabled the object
        {
            foreach (GUIElementReaction i in selectionSet)
            {
                i.CancelSelection();
            }
            isCurrentSelection = true;
            UpdateScaler(1.0f);
            if (moveToPositionOnSelect)
            {
                moveObjectToNewPosition();
            }
        } else
        {
            //i feel like we should indicate that it won't work until you download, but i don't know how?
        }
    }

    void MoveTowardsDestination()
    {
        t += Time.deltaTime * speed;
        if (t < 1)
        {
            transform.localPosition = Vector3.Lerp(startingPosition, destination, t);
            transform.localScale = Vector3.Lerp(startingScale, currentMoveToScaler, t);
        }
        else
        {
            //moving complete
            transform.localPosition = destination;
            moving = false;
            t = 0; 
            if (destination.Equals(moveToPosition))
            {
                UpdatePositions();
            }
        }
    }

    void UpdatePositions()
    {
        startingPosition = transform.localPosition;
        highlightPosition = transform.localPosition + new Vector3(0, 0, -0.25f);
        selectedPosition = transform.localPosition + new Vector3(0, 0, -0.1f);
        startingScale = transform.localScale;
    }

    public void CancelSelection()
    {
        isCurrentSelection = false;
        updateRenderer(normal);
    }

    public void moveObjectToNewPosition(bool calledAlready = default(bool))
    {
        destination = moveToPosition;
        moving = true;
        UpdateScaler(moveToScaler);
        speed = 0.7f;
        moveToPositionOnSelect = false; //we only wanna do this once, so once we trigger we can make this false
        //somehow need to lock clicking while this happens
        if (!calledAlready)
        {
            foreach (GUIElementReaction i in selectionSet)
            {
                i.moveObjectToNewPosition(true);
                newUI.GetComponent<RevealOnVisible>().ResetVisibility();
                newUI.GetComponent<RevealOnVisible>().beginReveal();
            }
        }

        
    }

    void UpdateScaler(float scaler)
    {
        currentMoveToScaler = new Vector3(transform.localScale.x * scaler, transform.localScale.y * scaler, transform.localScale.z * scaler);
    }

    void updateRenderer(Sprite sprite)
    {
        if (!isMovieSprite)
        {
           ren.sprite = sprite;
        }
    }

    void enableWhenDownloadsComplete()
    {
        if(DV.downloadingComplete)
        {
            isEnabled = true;
            updateRenderer(normal);
        }
    }
}
