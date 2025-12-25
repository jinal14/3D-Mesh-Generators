using UnityEngine;
using System.Collections;

public class VMCTargetMover : MonoBehaviour
{
    public Transform mainPoint;

    [Header("Target cubes order")]
    public Transform[] cubeTargets;   // Cube1, Cube2, Cube3, Cube4...

    [Header("Settings")]
    public float moveSpeed = 1.5f;
    public float waitBeforeMove = 1f;
    public float waitAfterDrill = 2f;

    public float upY = 0.75f;     // UP height
    public float drillY = 0.579f; // DOWN height

    [Header("Final Return Position")]
    public Vector3 finalReturnPos = new Vector3(0.222f, 0.512f, -8.364f);

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        for (int i = 0; i < cubeTargets.Length; i++)
        {
            Transform target = cubeTargets[i];

            // 1️⃣ WAIT BEFORE NEXT MOVE
            yield return new WaitForSeconds(waitBeforeMove);

            // 2️⃣ MOVE X first
            yield return StartCoroutine(MoveAxisX(target.position.x));

            // 3️⃣ MOVE Z second
            yield return StartCoroutine(MoveAxisZ(target.position.z));

            // 4️⃣ MOVE down (drill)
            yield return StartCoroutine(MoveAxisY(drillY));

            // 5️⃣ Activate the cube
            target.gameObject.SetActive(true);

            // 6️⃣ Wait after drill
            yield return new WaitForSeconds(waitAfterDrill);

            // 7️⃣ Move UP again
            yield return StartCoroutine(MoveAxisY(upY));
        }

        // 🔥 AFTER ALL CUBES — RETURN TO START POSITION

        Debug.Log("Returning to final position...");

        // Wait before return
        yield return new WaitForSeconds(waitBeforeMove);

        // 1️⃣ Move X to return X
        yield return StartCoroutine(MoveAxisX(finalReturnPos.x));

        // 2️⃣ Move Z to return Z
        yield return StartCoroutine(MoveAxisZ(finalReturnPos.z));

        // 3️⃣ Move Y to return Y
        yield return StartCoroutine(MoveAxisY(finalReturnPos.y));

        Debug.Log("✔ Returned to starting position.");
    }

    // ------------------------- MOVE AXIS FUNCTIONS -------------------------

    IEnumerator MoveAxisX(float targetX)
    {
        Vector3 start = mainPoint.position;
        Vector3 end = new Vector3(targetX, start.y, start.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            mainPoint.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    IEnumerator MoveAxisZ(float targetZ)
    {
        Vector3 start = mainPoint.position;
        Vector3 end = new Vector3(start.x, start.y, targetZ);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            mainPoint.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    IEnumerator MoveAxisY(float targetY)
    {
        Vector3 start = mainPoint.position;
        Vector3 end = new Vector3(start.x, targetY, start.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            mainPoint.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }
}
