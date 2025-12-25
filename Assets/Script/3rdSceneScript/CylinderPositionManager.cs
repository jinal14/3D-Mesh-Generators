using UnityEngine;
using System.Collections.Generic;

public class CylinderPositionManager : MonoBehaviour
{
    [Header("References")]
    public Transform mainCylinder;
    public GameObject smallPrefab;
    public Transform positionsParent;

    [Header("Settings")]
    public float smallHeightMeters = 0.05f;
    public float attachGap = 0.0f;

    // Your cylinder sizes in mm — FIXED LIST
    public int[] cylinderMM = new int[] { 1, 2, 4, 6, 8, 10, 12, 15, 16, 18, 20, 25 };

    Dictionary<int, GameObject> placed = new Dictionary<int, GameObject>();

    void Start()
    {
        // initialize storage for each mm cylinder
        foreach (int mm in cylinderMM)
            placed[mm] = null;
    }

    // Call this when clicking: PlaceCylinder(10), PlaceCylinder(6), etc.
    public void PlaceCylinder(int mmValue)
    {
        // check if valid mm
        if (!System.Array.Exists(cylinderMM, x => x == mmValue))
        {
            Debug.LogError("Invalid cylinder mm: " + mmValue);
            return;
        }

        // get slot index from mm list
        int slotIndex = System.Array.IndexOf(cylinderMM, mmValue); // 0 to 11
        int totalSlots = cylinderMM.Length;  // 12 slots

        // delete old cylinder if exists
        if (placed[mmValue] != null)
        {
            Destroy(placed[mmValue]);
        }

        // angle = slotIndex * (360 / totalSlots)
        float angleDeg = slotIndex * (360f / totalSlots);
        float angleRad = angleDeg * Mathf.Deg2Rad;

        // main cylinder parameters
        float mainRadius = GetMainRadius();
        float mainHeight = GetMainHeight();
        Vector3 mainPos = mainCylinder.position;

        // convert mm to meters
        float diameterM = mmValue / 1000f;
        float smallRadius = diameterM / 2f;

        float placementRadius = mainRadius + smallRadius + attachGap;

        // Y position (under the main cylinder)
        float yPos = mainPos.y
                    - (mainHeight / 2f)
                    - (smallHeightMeters / 2f);

        // compute position
        float x = mainPos.x + placementRadius * Mathf.Cos(angleRad);
        float z = mainPos.z + placementRadius * Mathf.Sin(angleRad);

        Vector3 spawnPos = new Vector3(x, yPos, z);

        // spawn cylinder
        GameObject go = Instantiate(smallPrefab, spawnPos, Quaternion.identity, positionsParent);
        go.name = "Cylinder_" + mmValue + "mm";

        // scale based on mm
        go.transform.localScale = new Vector3(diameterM, smallHeightMeters / 2f, diameterM);

        // rotate outward
        go.transform.rotation = Quaternion.Euler(0f, -angleDeg, 0f);

        placed[mmValue] = go;

        Debug.Log($"Placed {mmValue}mm at slot {slotIndex + 1}, angle {angleDeg}°");
    }

    float GetMainRadius()
    {
        return mainCylinder.localScale.x / 2f;
    }

    float GetMainHeight()
    {
        return mainCylinder.localScale.y * 2f;
    }
}
