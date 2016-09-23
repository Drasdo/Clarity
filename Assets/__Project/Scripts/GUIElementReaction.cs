using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIElementReaction : MonoBehaviour {

    private SpriteRenderer ren;
    public string setName = "";
    public Sprite normal;
    public Sprite hover;
    public Sprite select;

    public bool moveToPositionOnSelect = false;
    public Vector3 moveToPosition;
    public float moveToScaler = 1;
    public bool triggerNewUI = false;
    public GUIElementReaction newUI;


    private bool moving;
    private Vector3 startingPosition;
    private Vector3 selectedPosition;
    private Vector3 highlightPosition;
    private Vector3 startingScale;
    private Vector3 destination;
    private Vector3 currentMoveToScaler;
    private float speed = 5.0f;

    [HideInInspector]
    public bool isCurrentSelection; //only public for access to other scripts, shouldn't be touched in editor
    private List<GUIElementReaction> selectionSet = new List<GUIElementReaction>();

    private float t;

    void Start()
    {
        ren = GetComponentInChildren<SpriteRenderer>();
        moving = false;
        UpdatePositions();
        UpdateScaler(1.0f);

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
        if(moving)
        {
            MoveTowardsDestination();
        }
    }

    public void OnHover()
    {
        ren.sprite = hover;
        destination = highlightPosition;
        UpdateScaler(1.0f);
        moving = true;
    }

    public void OnExit()
    {
        if (isCurrentSelection)
        {
            ren.sprite = select;
        } else
        {
            ren.sprite = normal;
        }
        if (moveToPositionOnSelect && isCurrentSelection)
        {
            destination = moveToPosition;
        }
        else
        {
            destination = startingPosition;
        }
        moving = true;
    }

    public void OnClick()
    {
        //ren.sprite = select;
        foreach(GUIElementReaction i in selectionSet)
        {
            i.CancelSelection();
        }
        isCurrentSelection = true;
        UpdateScaler(1.0f);
        if(moveToPositionOnSelect)
        {
            moveObjectToNewPosition();
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
            transform.localPosition = destination;
            moving = false;
            t = 0;
            if(destination.Equals(moveToPosition))
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
        ren.sprite = normal;
    }

    public void moveObjectToNewPosition(bool calledAlready = default(bool))
    {
        //ren.sprite = hover;
        destination = moveToPosition;
        moving = true;
        UpdateScaler(moveToScaler);
        speed = 2.0f;
        //somehow need to lock clicking while this happens
        if (!calledAlready)
        {
            foreach (GUIElementReaction i in selectionSet)
            {
                i.moveObjectToNewPosition(true);
                newUI.Appear();
            }
        }

        
    }

    void UpdateScaler(float scaler)
    {
        currentMoveToScaler = new Vector3(transform.localScale.x * scaler, transform.localScale.y * scaler, transform.localScale.z * scaler);
    }

    public void Appear()
    {
        //possiblywait a second?
        //TODO write this
    }
}
