using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLine : MonoBehaviour
{
    private LineRenderer line;
    private Transform surface;
    private Collider surfaceCollider;
    private int pointIndex = 0;
    private float minDistance = 0.01f;
    private Vector3 lastPos;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;
    }

    public void StartLine(Transform surface)
    {
        this.surface = surface;
        surfaceCollider = surface.GetComponent<Collider>();
        if (surfaceCollider == null)
        {
            Debug.LogWarning("Surface has no Collider — line cannot stick to it.");
        }

        // Add the first point (on surface)
        if (surfaceCollider != null)
        {
            Vector3 p = surfaceCollider.ClosestPoint(surface.position);
            AddPoint(p);
        }
        else
        {
            AddPoint(surface.position);
        }
    }

    private void Update()
    {
        if (surface == null) return;

        Vector3 targetPos = surface.position;

        if (surfaceCollider != null)
        {
            // Snap to actual surface
            targetPos = surfaceCollider.ClosestPoint(surface.position);
        }

        float distance = Vector3.Distance(lastPos, targetPos);

        if (distance >= minDistance)
        {
            AddPoint(targetPos);
        }
    }

    private void AddPoint(Vector3 position)
    {
        line.positionCount++;
        line.SetPosition(pointIndex, position);

        lastPos = position;
        pointIndex++;
    }
}