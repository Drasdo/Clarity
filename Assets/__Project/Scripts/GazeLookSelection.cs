using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class GazeLookSelection : MonoBehaviour
{
    public float timerDuration = 2f; // How long to look at Menu Item before taking action
    private float timerDuration2 = 0.0f; //sizing up dtimer
    private float lookTimer = 0f; // This value will count down from the duration
    private float lookTimer2 = 0f;
    private float startingSizeUp = 0.21f;
    private float startingSizeDown = 0.21f;
    private EventTrigger currentGazeObject; // My renderer so I can set _Cutoff value
    private bool isLookedAt = false; // Is player looking at me?
    public bool hasBeenClicked = false;
    private Renderer retMaterial; //Renderer for the reticle

    void Start()
    {
        retMaterial = GetComponent<Renderer>();
        timerDuration2 = timerDuration / 10;
        timerDuration -= timerDuration2;
    }

    void Update()
    {
        if (isLookedAt && !hasBeenClicked) // are we looking at something?
        {
            if (lookTimer / timerDuration2 < timerDuration2) //before a 1/10th of the duration has passed, size up to the right size
            {
                lookTimer += Time.deltaTime;
                retMaterial.material.SetFloat("_InnerDiameter", Mathf.Lerp(0f, startingSizeUp, lookTimer / timerDuration2));
                startingSizeDown = startingSizeUp;
            }
            else if (lookTimer2 / timerDuration < timerDuration)
            {
                lookTimer2 += Time.deltaTime;
                retMaterial.material.SetFloat("_InnerDiameter", Mathf.Lerp(startingSizeDown, 0f, lookTimer2 / timerDuration));
            }

            if (lookTimer2 > timerDuration)
            {
                lookTimer = 0f; // Reset timer
                lookTimer2 = 0f;
                Debug.Log("BUTTON HAS BEEN SELECTED!"); // Do something
                currentGazeObject.OnPointerClick(null);
                hasBeenClicked = true;
            }
        }
        else
        {
            lookTimer = 0f; // Reset Timer
            lookTimer2 = 0f;
            //retMaterial.material.SetFloat("_InnerDiameter", startingSize);
        }
    }

    // Google Cardboard Gaze
    public void SetGazedAt(bool gazedAt, GameObject currentGUI)
    {
        hasBeenClicked = false;
        isLookedAt = gazedAt; // Set the local bool to the one passed from Event Trigger
        currentGazeObject = currentGUI.GetComponent<EventTrigger>();
    }
}