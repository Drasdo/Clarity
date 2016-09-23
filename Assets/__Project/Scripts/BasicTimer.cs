using UnityEngine;
using System.Collections;

public class BasicTimer : MonoBehaviour
{
    public float targetTime = 60.0f;
    private bool timerCurrentlyOn = false;
    private bool timerIsDone = false;

    void Update()
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
        timerCurrentlyOn = true;
    }

    public void ResetOrCancelTimer()
    {
        timerCurrentlyOn = false;
        timerIsDone = false;
    }
}