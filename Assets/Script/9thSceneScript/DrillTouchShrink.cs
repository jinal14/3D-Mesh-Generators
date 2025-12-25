using UnityEngine;
using System.Collections;

public class DrillTouchShrink : MonoBehaviour
{
    public Transform drill;           // drill object
    public Transform topPosition;     // drill's original top position

    public CylinderShrink shrinkScript;   // reference to shrink script

    public float masterSpeed = 0.05f;     // <<< Change only this speed.` 

    private bool drillMoving = false;

    void Start()
    {
        // Give shrink script the same speed
        shrinkScript.drillSpeed = masterSpeed;

        // Listen for shrink finish event
        shrinkScript.OnShrinkFinished += StopDrillAndReturn;
    }

    public void StartDrill()
    {
        drillMoving = true;
    }

    void Update()
    {
        if (drillMoving)
        {
            // Move drill in world space
            drill.position += Vector3.down * masterSpeed * Time.deltaTime;
        }
    }

    private void StopDrillAndReturn()
    {
        // Stop drill movement
        drillMoving = false;

        // Start return sequence
        StartCoroutine(ReturnAfterDelay());
    }

    private IEnumerator ReturnAfterDelay()
    {
        // Wait 2 seconds before returning
        yield return new WaitForSeconds(2f);

        // Move drill back up slowly
        while (Vector3.Distance(drill.position, topPosition.position) > 0.001f)
        {
            drill.position = Vector3.MoveTowards(
                drill.position,
                topPosition.position,
                masterSpeed * Time.deltaTime
            );

            yield return null;
        }
    }
}