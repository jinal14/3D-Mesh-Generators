using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class GCodeCubeMover : MonoBehaviour
{
    [Header("Drawing")]
    public LineRenderer pathLine;
    public float drawPointDistance = 0.05f;

    [Header("Movement")]
    public float scale = 0.5f;
    public float moveSpeed = 2f;

    [Header("Plane Cutting")]
    public PlaneGridGenerator planeGrid;
    public enum PlaneCutMode { DisableInner, DisableOuter }
    public PlaneCutMode planeCutMode = PlaneCutMode.DisableInner;

    // Internal
    private Vector3 startPosition;
    private Vector3 currentPos;

    private Vector3 firstGCodePoint;
    private bool firstPointSet;

    private List<Vector3> gcodePath = new List<Vector3>();
    private Vector3 lastDrawPoint;

    // =========================================================

    void Start()
    {
        startPosition = transform.position;
        currentPos = Vector3.zero;

        if (pathLine)
            pathLine.positionCount = 0;

        // 🔽 SELECT SHAPE
        // string[] gcode = TriangleGCode();
        // string[] gcode = HexagonGCode();
        string[] gcode = CircleGCode();
        // string[] gcode = StarGCode();
        // string[] gcode = AmoebaGCode();

        BuildGCodePath(gcode);
        StartCoroutine(MoveAndDraw());
    }

    // ================= BUILD PATH =================

    void BuildGCodePath(string[] gcode)
    {
        gcodePath.Clear();
        firstPointSet = false;
        currentPos = Vector3.zero;

        foreach (string cmd in gcode)
        {
            if (!cmd.StartsWith("G0") && !cmd.StartsWith("G1"))
                continue;

            Vector3 localPoint = ParsePosition(cmd);

            if (!firstPointSet)
            {
                firstGCodePoint = localPoint;
                firstPointSet = true;
            }

            Vector3 normalized = localPoint - firstGCodePoint;
            Vector3 worldTarget = startPosition + normalized;

            gcodePath.Add(worldTarget);
        }
        // 🔒 Ensure polygon is closed
        if (gcodePath.Count > 2 &&
            Vector3.Distance(gcodePath[0], gcodePath[gcodePath.Count - 1]) > 0.001f)
        {
            gcodePath.Add(gcodePath[0]);
        }

    }

    // ================= PARSE G-CODE =================

    Vector3 ParsePosition(string cmd)
    {
        float x = currentPos.x;
        float z = currentPos.z;

        string[] parts = cmd.Split(' ');
        foreach (string p in parts)
        {
            if (p.StartsWith("X"))
                x = float.Parse(p.Substring(1), CultureInfo.InvariantCulture);
            if (p.StartsWith("Y"))
                z = float.Parse(p.Substring(1), CultureInfo.InvariantCulture);
        }

        currentPos = new Vector3(x * scale, 0f, z * scale);
        return currentPos;
    }

    // ================= MOVE + DRAW =================

    IEnumerator MoveAndDraw()
    {
        lastDrawPoint = transform.position;
        AddDrawPoint(lastDrawPoint);

        foreach (Vector3 target in gcodePath)
        {
            while (Vector3.Distance(transform.position, target) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    target,
                    moveSpeed * Time.deltaTime
                );

                TryDraw();
                yield return null;
            }
        }

        // ✅ CUT PLANE AFTER DRAWING FINISHES
        ApplyPlaneCut();
    }

    void TryDraw()
    {
        if (Vector3.Distance(lastDrawPoint, transform.position) >= drawPointDistance)
        {
            AddDrawPoint(transform.position);
            lastDrawPoint = transform.position;
        }
    }

    void AddDrawPoint(Vector3 point)
    {
        if (!pathLine) return;

        pathLine.positionCount++;
        pathLine.SetPosition(pathLine.positionCount - 1, point);
    }

    // ================= PLANE CUTTING =================
    Bounds CalculatePolygonBounds()
    {
        Bounds b = new Bounds(gcodePath[0], Vector3.zero);

        foreach (Vector3 p in gcodePath)
            b.Encapsulate(p);

        // Small padding for safety
        b.Expand(0.01f);

        return b;
    }

    void ApplyPlaneCut()
    {
        if (!planeGrid || gcodePath.Count < 3) return;

        Bounds polyBounds = CalculatePolygonBounds();

        foreach (PlaneCell cell in planeGrid.cells)
        {
            Vector3 cellPos = cell.transform.position;

            // 🔹 Early reject (VERY IMPORTANT)
            if (!polyBounds.Contains(cellPos))
            {
                if (planeCutMode == PlaneCutMode.DisableOuter)
                    cell.DisableCell();

                continue;
            }

            bool inside = IsInsidePolygon(cellPos);

            if (planeCutMode == PlaneCutMode.DisableInner && inside)
                cell.DisableCell();

            if (planeCutMode == PlaneCutMode.DisableOuter && !inside)
                cell.DisableCell();
        }
    }


    // ================= POINT IN POLYGON =================
    // Ray-casting algorithm (XZ plane)

    bool IsInsidePolygon(Vector3 worldPos)
    {
        int count = gcodePath.Count;
        bool inside = false;

        float px = worldPos.x;
        float pz = worldPos.z;

        for (int i = 0, j = count - 1; i < count; j = i++)
        {
            Vector3 pi = gcodePath[i];
            Vector3 pj = gcodePath[j];

            bool intersect =
                ((pi.z > pz) != (pj.z > pz)) &&
                (px < (pj.x - pi.x) * (pz - pi.z) / (pj.z - pi.z + 0.00001f) + pi.x);

            if (intersect)
                inside = !inside;
        }

        return inside;
    }


    // ================= SHAPES =================

    string[] TriangleGCode()
    {
        return new string[]
        {
            "G0 X0 Y0",
            "G1 X4 Y0",
            "G1 X2 Y3",
            "G1 X0 Y0"
        };
    }

    string[] HexagonGCode()
    {
        return new string[]
        {
            "G0 X2 Y0",
            "G1 X4 Y1",
            "G1 X4 Y3",
            "G1 X2 Y4",
            "G1 X0 Y3",
            "G1 X0 Y1",
            "G1 X2 Y0"
        };
    }

    string[] CircleGCode()
    {
        int points = 100;
        float radius = 4f;
        List<string> gcode = new List<string>();

        for (int i = 0; i <= points; i++)
        {
            float angle = (2f * Mathf.PI / points) * i;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            string cmd = (i == 0 ? "G0" : "G1") +
                         $" X{x.ToString("F3", CultureInfo.InvariantCulture)}" +
                         $" Y{y.ToString("F3", CultureInfo.InvariantCulture)}";

            gcode.Add(cmd);
        }

        return gcode.ToArray();
    }

    string[] StarGCode()
    {
        return new string[]
        {
            "G0 X0 Y3",
            "G1 X2 Y3",
            "G1 X3 Y0",
            "G1 X4 Y3",
            "G1 X6 Y3",
            "G1 X4.5 Y5",
            "G1 X5.5 Y8",
            "G1 X3 Y6",
            "G1 X0.5 Y8",
            "G1 X1.5 Y5",
            "G1 X0 Y3"
        };
    }

    string[] AmoebaGCode()
    {
        return new string[]
        {
            "G0 X0 Y0",
            "G1 X1.2 Y0.4",
            "G1 X2.1 Y1.6",
            "G1 X1.5 Y3.1",
            "G1 X2.8 Y4.2",
            "G1 X1.6 Y5.3",
            "G1 X0.3 Y4.6",
            "G1 X-1.2 Y5.1",
            "G1 X-2.6 Y3.9",
            "G1 X-1.8 Y2.4",
            "G1 X-3.1 Y1.1",
            "G1 X-1.9 Y0.2",
            "G1 X-0.6 Y-0.8",
            "G1 X0.9 Y-0.4",
            "G1 X0 Y0"
        };
    }
}
