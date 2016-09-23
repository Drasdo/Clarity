using UnityEngine;
using System.Collections;

public class MoveToFrontOfCamera : MonoBehaviour {

    public GameObject desiredLocation;
    public GameObject locationRepresenter;
    public float speed = 2.0f; //Speed at which the object rotates towards the camera.

    private bool currentlyMoving = false;
    private BasicTimer timer;
    private float timeForTimer = 2.0f;
    private float yAxisVal;

    // Use this for initialization
    void Start () {
        timer = gameObject.AddComponent<BasicTimer>();
        yAxisVal = transform.position.y;

    }
	
	// Update is called once per frame
	void Update () {
        //either we need to already be in the process of moving the objects, or check if we need to
        if (currentlyMoving || StartMoving())
        {
            transform.position = Vector3.Slerp(transform.position, desiredLocation.transform.position, speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, yAxisVal, transform.position.z);
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
            //object is in frame, we are cool
            //infact, if there was a timer, then we can probably just go ahead and cancel that.
            if(timer.IsTimerTicking())
            {
                timer.ResetOrCancelTimer();
            }
            return false;
        }
        else
        {
            //We are outside of frame! lets see if the timer is done
            if (timer.IsTimerFinished())
            {
                //Timer has finished, so we can now start moving these things
                //we are done with timer for now so reset the values
                timer.ResetOrCancelTimer();
                //set currently moving as well as return value so we know that is done for next time
                return currentlyMoving = true;    
            } else if (timer.IsTimerTicking()) //timer isn't finished, but maybe we are currently counting down?
            {
                //i don't know if I need to actually do anything here...
            } else //we should probably start the timer
            {
                timer.StartTimer(timeForTimer);
            }
        }
        return false;
    }

    bool CheckIfBackInPosition()
    {   
        //we want to ignore the Y axis because we aren't moving the object along the Y axis. Otherwise it will never be close.
        if (Vector3Compare.V3EqualWithoutY(transform.position, desiredLocation.transform.position, 0.01f, yAxisVal))
        {
            //We are back in the right position! isn't that nice. reset everything
            currentlyMoving = false;
            return true;
        }
        return false;
    }
}
