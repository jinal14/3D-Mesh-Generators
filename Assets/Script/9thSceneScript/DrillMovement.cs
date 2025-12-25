using UnityEngine;
using System.Collections;

public class DrillMovement : MonoBehaviour
{
    [Header("References")]
    public Transform drill;       // Drill body (parent)
    public Transform needle;      // Needle (child of drill)
    public Transform cube;        // Cube to drill

    public GameObject originalCube;
    public GameObject holeCube;

    [Header("Settings")]
    public float moveSpeed = 1.5f;
    public float waitAtCubeTop = 1f;
    public float drillGap = 0.005f;

    Vector3 drillStartPos;
    float drillHeight;
    bool isRunning = false;

    void Start()
    {
        // Store starting world position
        drillStartPos = drill.position;

        // Find drill height dynamically from renderer
        var rend = drill.GetComponent<Renderer>();
        if (rend != null)
            drillHeight = rend.bounds.size.y;
        else
            drillHeight = 1f;

        // Hole setup
        if (originalCube != null) originalCube.SetActive(true);
        if (holeCube != null) holeCube.SetActive(false);
    }

    public void StartDrillMove()
    {
        if (!isRunning)
            StartCoroutine(DrillSequence());
    }

    IEnumerator DrillSequence()
    {
        isRunning = true;

        // ----------- CALCULATE DYNAMIC TARGET POSITION -----------
        // cube top in world Y
        float cubeTopY = cube.position.y + (cube.localScale.y / 2f);

        float targetDrillY = cubeTopY + (drillHeight / 2f) + drillGap;
        
        // target world position
        Vector3 targetPos = new Vector3(drillStartPos.x, targetDrillY, drillStartPos.z);

        // ----------- MOVE DRILL DOWN TO DYNAMIC TARGET -----------
        while (Mathf.Abs(drill.position.y - targetPos.y) > 0.001f)
        {
            drill.position = Vector3.MoveTowards(
                drill.position,
                targetPos,
                moveSpeed * Time.deltaTime);

            // Needle NEVER detaches because it's child — no movement code needed
            yield return null;
        }

        // ensure EXACT Y stop
        drill.position = targetPos;

        // ----------- WAIT ON TOP OF CUBE -----------
        yield return new WaitForSeconds(waitAtCubeTop);

        // ----------- HOLE CUBE SWAP (optional) -----------
        if (originalCube != null && holeCube != null)
        {
            originalCube.SetActive(false);
            holeCube.SetActive(true);
        }

        // ----------- RETURN DRILL + NEEDLE BACK TO START -----------
        while (Vector3.Distance(drill.position, drillStartPos) > 0.001f)
        {
            drill.position = Vector3.MoveTowards(
                drill.position,
                drillStartPos,
                moveSpeed * Time.deltaTime);

            yield return null;
        }

        drill.position = drillStartPos;

        isRunning = false;
    }
}
