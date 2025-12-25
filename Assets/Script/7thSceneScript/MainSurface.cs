using System.Collections;
using UnityEngine;

public class SurfaceMovement : MonoBehaviour
{
    public Transform surface;      // moves only in X and Z
    public Transform mainPoint;    // moves in Y like a drilling head
    public GameObject cube1, cube2, cube3, cube4;

    public float moveSpeed = 2f;

    private float surfaceY = 0.52f;
    private float mainPointTopY = 0.75f;
    private float mainPointDownY = 0.57f;

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        // Set start position
        surface.position = new Vector3(0.21f, surfaceY, -8.36f);
        mainPoint.position = new Vector3(mainPoint.position.x, mainPointTopY, mainPoint.position.z);

        yield return new WaitForSeconds(1);

        // Move X
        yield return MoveSurfaceTo(new Vector3(-0.019f, surfaceY, -8.36f));
        yield return new WaitForSeconds(1);

        // Move Z
        yield return MoveSurfaceTo(new Vector3(-0.019f, surfaceY, -8.12f));
        yield return new WaitForSeconds(1);

        // ---- CUBE 1 ----
        yield return DrillAndActivate(cube1);

        // Move X for cube2
        yield return MoveSurfaceTo(new Vector3(0.468f, surfaceY, -8.12f));

        // ---- CUBE 2 ----
        yield return DrillAndActivate(cube2);

        // Move Z for cube3
        yield return MoveSurfaceTo(new Vector3(0.468f, surfaceY, -8.633f));
        yield return new WaitForSeconds(1);

        // ---- CUBE 3 ----
        yield return DrillAndActivate(cube3);

        // Move X for cube4
        yield return MoveSurfaceTo(new Vector3(-0.038f, surfaceY, -8.633f));
        yield return new WaitForSeconds(1);

        // ---- CUBE 4 ----
        yield return DrillAndActivate(cube4);

        // Return sequence
        yield return MoveSurfaceTo(new Vector3(-0.038f, surfaceY, -8.36f));
        yield return new WaitForSeconds(1);

        yield return MoveSurfaceTo(new Vector3(0.21f, surfaceY, -8.36f));
        yield return new WaitForSeconds(1);

        Debug.Log("SCRIPT COMPLETED SUCCESSFULLY!");
    }

    // Moves surface only in X and Z
    IEnumerator MoveSurfaceTo(Vector3 targetPos)
    {
        while (Vector3.Distance(surface.position, targetPos) > 0.001f)
        {
            surface.position = Vector3.Lerp(surface.position, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }
        surface.position = targetPos;
    }

    // NEW: Drill movement + activate cube at correct time
    IEnumerator DrillAndActivate(GameObject cube)
    {
        // MainPoint DOWN (start drilling)
        yield return MoveMainPointY(mainPointDownY);

        // 🔥 Activate cube exactly at down position
        cube.SetActive(true);

        yield return new WaitForSeconds(2);

        // MainPoint UP
        yield return MoveMainPointY(mainPointTopY);

        yield return new WaitForSeconds(1);
    }

    // Move mainPoint only in Y
    IEnumerator MoveMainPointY(float targetY)
    {
        Vector3 targetPos = new Vector3(mainPoint.position.x, targetY, mainPoint.position.z);

        while (Vector3.Distance(mainPoint.position, targetPos) > 0.001f)
        {
            mainPoint.position = Vector3.Lerp(mainPoint.position, targetPos, Time.deltaTime * moveSpeed);
            yield return null;
        }

        mainPoint.position = targetPos;
    }
}
