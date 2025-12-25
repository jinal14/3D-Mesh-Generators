using System.Collections;
using UnityEngine;

public class DynamicSurfaceMovement : MonoBehaviour
{
    public Transform surface;        // moves only in X and Z
    public Transform mainPoint;      // moves only in Y
    public GameObject[] cubes;       // cube1, cube2, cube3, cube4 (CHILD of surface)

    public float moveSpeed = 2f;

    float surfaceY = 0.52f;
    float mainPointUpY = 0.75f;
    float mainPointDownY = 0.57f;

    Vector3[] cubeWorldPos;          // store ORIGINAL world positions before movement
    Vector3 startSurfacePos;         // home/return position


    void Start()
    {
        startSurfacePos = surface.position;

        // store cubes' world positions before the surface moves
        cubeWorldPos = new Vector3[cubes.Length];
        for (int i = 0; i < cubes.Length; i++)
        {
            cubeWorldPos[i] = cubes[i].transform.position;
        }

        StartCoroutine(MoveSequence());
    }


    IEnumerator MoveSequence()
    {
        // MainPoint starting Y (up)
        mainPoint.position = new Vector3(mainPoint.position.x, mainPointUpY, mainPoint.position.z);
        yield return new WaitForSeconds(1);

        // === Move to each cube target ===
        for (int i = 0; i < cubeWorldPos.Length; i++)
        {
            Vector3 target = cubeWorldPos[i];

            // Move surface X only
            yield return MoveSurfaceX(target.x);
            yield return new WaitForSeconds(1);

            // Move surface Z only
            yield return MoveSurfaceZ(target.z);
            yield return new WaitForSeconds(1);

            // Drill at this location (auto-detect cube)
            yield return DrillAtCurrentPosition();
        }

        // === Return movement ===
        yield return MoveSurfaceZ(startSurfacePos.z);
        yield return new WaitForSeconds(1);

        yield return MoveSurfaceX(startSurfacePos.x);
        yield return new WaitForSeconds(1);
    }


    IEnumerator MoveSurfaceX(float targetX)
    {
        while (Mathf.Abs(surface.position.x - targetX) > 0.001f)
        {
            Vector3 p = surface.position;
            p.x = Mathf.MoveTowards(p.x, targetX, moveSpeed * Time.deltaTime);
            p.y = surfaceY;          // keep Y locked
            surface.position = p;

            yield return null;
        }
    }

    IEnumerator MoveSurfaceZ(float targetZ)
    {
        while (Mathf.Abs(surface.position.z - targetZ) > 0.001f)
        {
            Vector3 p = surface.position;
            p.z = Mathf.MoveTowards(p.z, targetZ, moveSpeed * Time.deltaTime);
            p.y = surfaceY;          // keep Y locked
            surface.position = p;

            yield return null;
        }
    }

    IEnumerator DrillAtCurrentPosition()
    {
        // Move drill down
        yield return MoveMainPointY(mainPointDownY);

        // Find closest cube in WORLD position
        GameObject closestCube = null;
        float minDist = Mathf.Infinity;

        for (int i = 0; i < cubes.Length; i++)
        {
            float dist = Vector3.Distance(mainPoint.position, cubes[i].transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closestCube = cubes[i];
            }
        }

        // Activate only the correct cube
        if (closestCube != null)
        {
            closestCube.SetActive(true);
        }

        yield return new WaitForSeconds(2);

        // Move drill up
        yield return MoveMainPointY(mainPointUpY);
        yield return new WaitForSeconds(1);
    }

    IEnumerator MoveMainPointY(float targetY)
    {
        while (Mathf.Abs(mainPoint.position.y - targetY) > 0.001f)
        {
            Vector3 np = mainPoint.position;
            np.y = Mathf.MoveTowards(np.y, targetY, moveSpeed * Time.deltaTime);
            mainPoint.position = np;

            yield return null;
        }
    }
}
