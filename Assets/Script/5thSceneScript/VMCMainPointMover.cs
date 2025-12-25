using UnityEngine;
using System.Collections;

public class VMCMainPointMover : MonoBehaviour
{
    public Transform mainPoint;

    public float moveSpeed = 1.5f;
    public float waitTime = 2f;

    // Target positions
    public float targetX = 0.464f;
    public float targetZ = -8.627f;
    public float targetY = 0.579f;

    public float upY = 0.75f;

    public GameObject cube1;
    public GameObject cube2;
    public GameObject cube3;   // NEW
    public GameObject cube4;   // NEW

    public float nextX = -0.037f;

    void Start()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {

        // Move X
        yield return StartCoroutine(MoveAxis('x', targetX));
        yield return new WaitForSeconds(waitTime);

        // Move Z
        yield return StartCoroutine(MoveAxis('z', targetZ));
        yield return new WaitForSeconds(waitTime);

        // Move Y
        yield return StartCoroutine(MoveAxis('y', targetY));

        cube1.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Move Up
        yield return StartCoroutine(MoveAxis('y', upY));

        // Move X to -0.037
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(MoveAxis('x', nextX));

        yield return new WaitForSeconds(waitTime);

        // Move Down
        yield return StartCoroutine(MoveAxis('y', targetY));

        cube2.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Move Up
        yield return StartCoroutine(MoveAxis('y', upY));

        
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(MoveAxis('z', -8.092f));

        // Wait 1 sec then move down
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(MoveAxis('y', 0.579f));

        // Activate cube3
        cube3.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Move Up again
        yield return StartCoroutine(MoveAxis('y', upY));
        yield return new WaitForSeconds(waitTime);

        // Move to x = 0.470
        yield return StartCoroutine(MoveAxis('x', 0.470f));

        // Move down to y = 0.579
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(MoveAxis('y', 0.579f));

        // Activate cube4
        cube4.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Move Up to 0.75
        yield return StartCoroutine(MoveAxis('y', upY));

        // Move to z = -8.345
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(MoveAxis('z', -8.345f));

        // Move to x = 0.222
        yield return new WaitForSeconds(waitTime);
        yield return StartCoroutine(MoveAxis('x', 0.222f));

        Debug.Log("✔ Full Sequence Complete Up To Cube 4!");
    }

    IEnumerator MoveAxis(char axis, float target)
    {
        Vector3 startPos = mainPoint.position;
        Vector3 endPos = mainPoint.position;

        if (axis == 'x') endPos.x = target;
        if (axis == 'y') endPos.y = target;
        if (axis == 'z') endPos.z = target;

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            mainPoint.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        mainPoint.position = endPos;
        mainPoint.position = endPos;
        mainPoint.position = endPos;
    }
}
