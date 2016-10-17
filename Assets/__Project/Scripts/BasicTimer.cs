using UnityEngine;
using System.Collections;

public class BasicTimer : MonoBehaviour
{
    public float targetTime = 60.0f;
    private bool timerCurrentlyOn = false;
    private bool timerIsDone = false;
    private bool pauseTimer = false;

    private float timerLen = 0.0f;

    void Update()
    {
        if (!pauseTimer)
        {
            if (timerCurrentlyOn)
            {
                targetTime -= Time.deltaTime;
                if (targetTime <= 0.0f)
                {
                    timerIsDone = true;
                    timerCurrentlyOn = false;
                }
            }
        }
    }
        
    public bool IsTimerFinished()
    {
        return timerIsDone;
    }

    public bool IsTimerTicking()
    {
        return timerCurrentlyOn;
    }

    public void StartTimer(float timerLength)
    {
        targetTime = timerLength;
        timerLen = timerLength;
        timerCurrentlyOn = true;
    }

    public void ResetOrCancelTimer()
    {
        timerCurrentlyOn = false;
        timerIsDone = false;
        targetTime = 60.0f;
    }

    public float timeRemaining()
    {
        return targetTime;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        pauseTimer = pauseStatus;
    }

    public float lerpVal()
    {
        return targetTime / timerLen;
    }
}