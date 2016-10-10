using UnityEngine;
using System.Collections;

public class MoveToFrontOfCamera : MonoBehaviour {

    public GameObject locationRepresenter;
    public float speed = 2.0f; //Speed at which the object rotates towards the camera.

    private GameObject moveToHere;
    private bool currentlyMoving = false;
    private bool finishedReveal = false;
    private BasicTimer timer;
    private float timeForTimer = 2.0f;
    private float yAxisVal;

    private Quaternion desiredRotation;

    // Use this for initialization
    void Start () {
        timer = gameObject.AddComponent<BasicTimer>();
        yAxisVal = transform.position.y;
        moveToHere = GameObject.FindGameObjectWithTag("SelectionOption");
        desiredRotation = Quaternion.identity;
    }
	
	// Update is called once per frame
	void Update () {
        //either we need to already be in the process of moving the objects, or check if we need to
        StartMoving();
        if (currentlyMoving)
        {
            transform.rotation = Quaternion.Slerp(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Quaternion.Euler(0, desiredRotation.eulerAngles.y, 0), speed * Time.deltaTime);
            //transform.position = new Vector3(transform.position.x, yAxisVal, transform.position.z);
            //now need to see if we are close enough to stop
            if (currentlyMoving)
            {
                CheckIfBackInPosition();
            }
        }
    }

    bool StartMoving()
    {
        if (locationRepresenter.GetComponent<Renderer>().isVisible)
        {
            if (finishedReveal)
            {
                //object is in frame, we are cool

                //infact, if there was a timer, then we can probably just go ahead and cancel that.
                if (timer.IsTimerTicking())
                {
                    timer.ResetOrCancelTimer();
                }
                return false;
            }
        }
        else
        {
            if (finishedReveal)
            {
                //We are outside of frame! lets see if the timer is done
                if (timer.IsTimerFinished())
                {
                    //Timer has finished, so we can now start moving these things
                    //we are done with timer for now so reset the values
                    //timer.ResetOrCancelTimer();
                    //set currently moving as well as return value so we know that is done for next time
                    desiredRotation = moveToHere.transform.rotation;
                    return currentlyMoving = true;
                }
                else if (timer.IsTimerTicking()) //timer isn't finished, but maybe we are currently counting down?
                {
                    desiredRotation = Quaternion.identity;
                }
                else //we should probably start the timer
                {
                    timer.StartTimer(timeForTimer);
                }
            }
        }
        return false;
    }

    bool CheckIfBackInPosition()
    {   
        //we want to ignore the Y axis because we aren't moving the object along the Y axis. Otherwise it will never be close.
        if(((transform.rotation.eulerAngles.y < desiredRotation.eulerAngles.y) ? (desiredRotation.eulerAngles.y - transform.rotation.eulerAngles.y) : (transform.rotation.eulerAngles.y - desiredRotation.eulerAngles.y)) <= 1f)
        //if (Mathf.Approximately(transform.rotation.eulerAngles.y, desiredRotation.eulerAngles.y))
        {
            //We are back in the right position! isn't that nice. reset everything
            currentlyMoving = false;
            timer.ResetOrCancelTimer();
            desiredRotation = Quaternion.identity;
            return true;
        }
        return false;
    }

    public void childDisabled()
    {
        currentlyMoving = false;
        transform.localRotation = Quaternion.identity;
        finishedReveal = false;
        timer.ResetOrCancelTimer();
    }

    public void finishedRevealing()
    {
        finishedReveal = true;
    }

    void OnEnable()
    {
        currentlyMoving = false;
    }
}
