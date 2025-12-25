using System.Collections.Generic;
using UnityEngine;

public class DrillController : MonoBehaviour
{
    public enum DrillState { Idle, MovingDown, NeedleInside, DrillingSpiral, Returning, Finished }

    [Header("References")]
    public Transform drillMachine;           // Full drill body
    public Transform needle;                 // Needle transform
    public Transform drillBottomPoint;       // Empty object at the bottom of the drill
    public BoxCollider targetCube;           // Cube collider
    // public SliceGenerator sliceGenerator;    // Optional slice generator

    [Header("Positions")]
    public Transform topPosition;            // Original top position of the drill

    [Header("Tool Settings")]
    public float toolDiameter = 0.2f;
    [Range(0.1f, 1f)]
    public float stepOverFactor = 0.5f;

    [Header("Speeds")]
    public float downSpeed = 0.6f;           // Body down movement
    public float needleDownSpeed = 0.3f;     // Needle movement inside cube
    public float maxNeedleDepth = 0.3f;      // Depth needle moves inside cube
    public float spiralMoveSpeed = 0.9f;     // Spiral cutting movement speed
    public float waypointThreshold = 0.01f;  // Distance threshold

    private DrillState state = DrillState.Idle;
    private float needleStartY;
    private float drillDepthY;
    private bool drillBodyStopped = false;

    private List<Vector3> spiralWaypoints;
    private int currentWaypoint = 0;

    [HideInInspector]
    public bool IsDrilling = false;

    private void Start()
    {
        if (drillMachine == null)
            drillMachine = this.transform;

     

        state = DrillState.Idle;
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

            case DrillState.DrillingSpiral:
                FollowSpiralPath();
                break;

            case DrillState.Returning:
                ReturnToTopPosition();
                break;
        }
    }

    public void StartDrilling()
    {
        if (state != DrillState.Idle) return;
        state = DrillState.MovingDown;
    }



    private void MoveDownUntilCubeSurface()
    {
        if (drillBodyStopped) return;

        Vector3 pos = drillMachine.position;
        pos.y -= downSpeed * Time.deltaTime;
        drillMachine.position = pos;

        if (Physics.Raycast(drillBottomPoint.position, Vector3.down, out RaycastHit hit, 2f))
        {
            if (hit.collider == targetCube)
            {
                float surfaceY = hit.point.y;
                float diff = drillBottomPoint.position.y - surfaceY;

                drillMachine.position -= new Vector3(0, diff, 0);

                drillBodyStopped = true;
                needleStartY = needle.position.y;

                state = DrillState.NeedleInside;

                Debug.Log("Drill body stopped at cube surface. Needle can now move inside.");
            }
        }
    }

    private void MoveNeedleInsideCube()
    {
        Vector3 np = needle.position;
        np.y -= needleDownSpeed * Time.deltaTime;
        needle.position = np;

        if (needleStartY - needle.position.y >= maxNeedleDepth)
        {
            drillDepthY = needle.position.y;

            Debug.Log("Needle reached depth → Start spiral milling");

            GenerateRectangularSpiral();
            currentWaypoint = 0;

            state = DrillState.DrillingSpiral;
        }
    }

    private void FollowSpiralPath()
    {
        if (spiralWaypoints == null || spiralWaypoints.Count == 0)
        {
            FinishDrilling();
            return;
        }

        if (currentWaypoint >= spiralWaypoints.Count)
        {
            FinishDrilling();
            return;
        }

        Vector3 target = spiralWaypoints[currentWaypoint];
        Vector3 currentPos = drillMachine.position;

        // Only move in X/Z plane, keep Y the same
        Vector3 targetPos = new Vector3(target.x, drillMachine.position.y, target.z);

        drillMachine.position = Vector3.MoveTowards(
            currentPos,
            targetPos,
            spiralMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
                new Vector3(currentPos.x, 0, currentPos.z),
                new Vector3(targetPos.x, 0, targetPos.z)) <= waypointThreshold)
        {
            
            // if (sliceGenerator != null)
            //     sliceGenerator.GenerateDynamicSliceAt(targetPos, Vector3.zero);

            currentWaypoint++;
        }
    }


    private void FinishDrilling()
    {
        state = DrillState.Returning;
        Debug.Log("Drilling Finished Successfully");
    }

    private void GenerateRectangularSpiral()
    {
        spiralWaypoints = new List<Vector3>();

        Bounds b = targetCube.bounds;

        float padding = toolDiameter * 0.5f;

        float minX = b.min.x + padding;
        float maxX = b.max.x - padding;
        float minZ = b.min.z + padding;
        float maxZ = b.max.z - padding;

        float stepOver = toolDiameter * stepOverFactor;

        while (minX < maxX - 0.001f && minZ < maxZ - 0.001f)
        {
            spiralWaypoints.Add(new Vector3(minX, drillDepthY, minZ));
            spiralWaypoints.Add(new Vector3(maxX, drillDepthY, minZ));
            spiralWaypoints.Add(new Vector3(maxX, drillDepthY, maxZ));
            spiralWaypoints.Add(new Vector3(minX, drillDepthY, maxZ));

            minX += stepOver;
            maxX -= stepOver;
            minZ += stepOver;
            maxZ -= stepOver;
        }

        // Final center point
        spiralWaypoints.Add(new Vector3(b.center.x, drillDepthY, b.center.z));
    }

    private void ReturnToTopPosition()
    {
        if (Vector3.Distance(drillMachine.position, topPosition.position) > 0.001f)
        {
            drillMachine.position = Vector3.MoveTowards(
                drillMachine.position,
                topPosition.position,
                downSpeed * Time.deltaTime
            );
        }
        else
        {
            state = DrillState.Finished;
            Debug.Log("Drill returned to top position. Drilling complete.");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (spiralWaypoints == null) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < spiralWaypoints.Count - 1; i++)
        {
            Gizmos.DrawLine(spiralWaypoints[i], spiralWaypoints[i + 1]);
        }
    }
#endif
}
