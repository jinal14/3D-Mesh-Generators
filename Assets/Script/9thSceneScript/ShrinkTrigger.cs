using UnityEngine;

public class ShrinkTrigger : MonoBehaviour
{
    public CylinderShrink shrinkScript;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Needle"))
        {
            triggered = true;
            shrinkScript.ShrinkNow();
        }
    }
}