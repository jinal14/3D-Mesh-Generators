using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform bottomPoint;   // <---- BOTTOM POINT REFERENCE ADDED
    public float minDistance = 0.001f;

    private Vector3 lastPoint;
    private bool canDraw = false;

    void Start()
    {
        lineRenderer.positionCount = 0;
    }

    public void StartDrawing()
    {
        canDraw = true;
        AddPoint();  // first point at bottom
    }

    public void StopDrawing()
    {
        canDraw = false;
    }

    void Update()
    {
        if (!canDraw) return;

        float distance = Vector3.Distance(bottomPoint.position, lastPoint);

        if (distance > minDistance)
        {
            AddPoint();
        }
    }

    void AddPoint()
    {
        // draw from bottomPoint — NOT the main transform
        Vector3 drawPos = bottomPoint.position;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, drawPos);

        lastPoint = drawPos;
    }
}