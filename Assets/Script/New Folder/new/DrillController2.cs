using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrillController_SpiralCubes : MonoBehaviour
{
    public enum DrillState
    {
        Idle,
        MovingDown,
        NeedleInside,
        Waiting,
        MovingToCorner,
        Returning,
        Finished
    }

    [Header("References")]
    public Transform drillMachine;
    public Transform needle;
    public Transform drillBottomPoint;
    public BoxCollider targetCube;
    public Transform topPosition;

    [Header("Movement Speeds")]
    public float downSpeed = 0.5f;
    public float needleDownSpeed = 0.25f;
    public float cornerMoveSpeed = 0.25f;   // 🔥 Slow & smooth
    public float maxNeedleDepth = 0.3f;
    [Header("Corner Cube Settings")]
    public int maxCornerCubes = 45;

    [Header("Timing")]
    public float waitAtCorner = 2f;

    private DrillState state = DrillState.Idle;

    private float needleStartY;
    private float drillDepthY;

    private List<Transform> cornerCubes = new List<Transform>();
    private int currentCornerIndex = 0;

    private bool isWaiting = false;

    // ===================== START =====================

    private void Start()
    {
        if (drillMachine == null)
            drillMachine = transform;

        CollectCornerCubes();
    }

    private void Update()
    {
        switch (state)
        {
            case DrillState.MovingDown:
                MoveDownUntilCubeSurface();
                break;

            case DrillState.NeedleInside:
                MoveNeedleInsideCube();
                break;

            case DrillState.MovingToCorner:
                MoveToCornerCube();
                break;

            case DrillState.Returning:
                ReturnToTop();
                break;
        }
    }

    // ===================== PUBLIC =====================

    public void StartDrilling()
    {
        if (state != DrillState.Idle) return;

        currentCornerIndex = 0;
        state = DrillState.MovingDown;
    }

    // ===================== DOWN =====================

    private void MoveDownUntilCubeSurface()
    {
        drillMachine.position = Vector3.MoveTowards(
            drillMachine.position,
            drillMachine.position + Vector3.down,
            downSpeed * Time.deltaTime
        );

        if (Physics.Raycast(drillBottomPoint.position, Vector3.down, out RaycastHit hit, 0.05f))
        {
            if (hit.collider == targetCube)
            {
                float diff = drillBottomPoint.position.y - hit.point.y;
                drillMachine.position -= new Vector3(0, diff, 0);

                needleStartY = needle.position.y;
                state = DrillState.NeedleInside;
            }
        }
    }

    // ===================== NEEDLE =====================

    private void MoveNeedleInsideCube()
    {
        needle.position += Vector3.down * needleDownSpeed * Time.deltaTime;

        if (needleStartY - needle.position.y >= maxNeedleDepth)
        {
            drillDepthY = needle.position.y;
            StartCoroutine(WaitThenMoveNext());
        }
    }

    // ===================== MOVE TO CUBE =====================

    private void MoveToCornerCube()
    {
        if (currentCornerIndex >= cornerCubes.Count)
        {
            state = DrillState.Returning;
            return;
        }

        Transform cube = cornerCubes[currentCornerIndex];
        Vector3 current = drillMachine.position;

        Vector3 target = new Vector3(
            cube.position.x,
            current.y,
            cube.position.z
        );

        drillMachine.position = Vector3.MoveTowards(
            current,
            target,
            cornerMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
            new Vector3(current.x, 0, current.z),
            new Vector3(target.x, 0, target.z)) < 0.005f)
        {
            StartCoroutine(WaitThenMoveNext());
        }
    }

    // ===================== WAIT =====================

    private IEnumerator WaitThenMoveNext()
    {
        state = DrillState.Waiting;
        yield return new WaitForSeconds(waitAtCorner);

        currentCornerIndex++;

        if (currentCornerIndex < cornerCubes.Count)
        {
            state = DrillState.MovingToCorner;
        }
        else
        {
            state = DrillState.Returning;
        }
    }

    // ===================== RETURN =====================

    private void ReturnToTop()
    {
        drillMachine.position = Vector3.MoveTowards(
            drillMachine.position,
            topPosition.position,
            downSpeed * Time.deltaTime
        );

        if (Vector3.Distance(drillMachine.position, topPosition.position) < 0.01f)
        {
            state = DrillState.Finished;
            Debug.Log("Spiral corner drilling finished");
        }
    }

    // ===================== COLLECT CUBES =====================

    private void CollectCornerCubes()
    {
        cornerCubes.Clear();

        for (int i = 1; i <= maxCornerCubes; i++)
        {
            GameObject cube = GameObject.Find("CornerCube" + i);

            if (cube != null)
            {
                cornerCubes.Add(cube.transform);
            }
            else
            {
                Debug.LogWarning("Missing CornerCube" + i);
            }
        }

        Debug.Log("Corner Cubes Found: " + cornerCubes.Count);
    }


    // ===================== GIZMOS =====================

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        if (cornerCubes == null) return;

        foreach (Transform t in cornerCubes)
        {
            if (t != null)
                Gizmos.DrawSphere(t.position, 0.02f);
        }
    }
#endif
}
