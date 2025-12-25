using UnityEngine;

public class NeedleTrigger : MonoBehaviour
{
    private CubeShrinkOnNeedle currentCube;

    private void OnTriggerEnter(Collider other)
    {
        CubeShrinkOnNeedle cube = other.GetComponent<CubeShrinkOnNeedle>();
        if (cube == null) return;

        if (currentCube != null)
            currentCube.StopShrink();

        currentCube = cube;
        currentCube.StartShrink();
    }

    private void OnTriggerExit(Collider other)
    {
        CubeShrinkOnNeedle cube = other.GetComponent<CubeShrinkOnNeedle>();
        if (cube == null) return;

        cube.StopShrink();

        if (cube == currentCube)
            currentCube = null;
    }
}