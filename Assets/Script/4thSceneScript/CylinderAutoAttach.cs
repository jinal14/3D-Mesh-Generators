using UnityEngine;

public class CylinderAutoAttach : MonoBehaviour
{
    [Header("References")]
    public Transform mainCylinder;
    public GameObject smallPrefab;
    public Transform positionsParent;

    [Header("Materials for 12 cylinders (1 to 25 mm)")]
    public Material[] cylinderMaterials = new Material[12];

    [Header("Settings")]
    public float smallHeightMeters = 0.05f;
    public float attachGap = 0f;

    public int[] cylinderMM = new int[]
    {
        1, 2, 4, 6, 8, 10, 12, 15, 16, 18, 20, 25
    };

    void Start()
    {
        AttachAllOnStart();
    }

    void AttachAllOnStart()
    {
        int totalSlots = cylinderMM.Length;  
        float angleStep = 360f / totalSlots;

        Vector3 mainPos = mainCylinder.position;
        float mainRadius = GetMainRadius();   // FIXED
        float mainHeight = GetMainHeight();

        float yPos = mainPos.y - (mainHeight / 2f) - (smallHeightMeters / 2f);

        for (int i = 0; i < totalSlots; i++)
        {
            int mmValue = cylinderMM[i];

            float diameterM = mmValue / 1000f;
            float smallRadius = diameterM / 2f;

            float angleDeg = i * angleStep;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            // ⭐ FIXED — cylinders now stay inside perfectly
            float placementRadius = mainRadius - smallRadius - 0.001f;

            float x = mainPos.x + placementRadius * Mathf.Cos(angleRad);
            float z = mainPos.z + placementRadius * Mathf.Sin(angleRad);

            Vector3 spawnPos = new Vector3(x, yPos, z);

            GameObject obj = Instantiate(
                smallPrefab,
                spawnPos,
                Quaternion.identity,
                positionsParent
            );

            obj.name = "Cylinder_" + mmValue + "mm";

            obj.transform.localScale = new Vector3(
                diameterM,
                smallHeightMeters / 2f,
                diameterM
            );

            obj.transform.rotation = Quaternion.Euler(0f, -angleDeg, 0f);

            ApplyMaterialToCylinder(obj, i);
        }

        Debug.Log("All cylinders attached + colored successfully.");
    }

    void ApplyMaterialToCylinder(GameObject obj, int index)
    {
        if (cylinderMaterials.Length >= 12 && cylinderMaterials[index] != null)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            rend.material = cylinderMaterials[index];
        }
        else
        {
            Debug.LogWarning("Missing material for index: " + index);
        }
    }

    // ⭐ FIXED – Real mesh radius
    float GetMainRadius()
    {
        Renderer rend = mainCylinder.GetComponent<Renderer>();
        return rend.bounds.extents.x;  
    }

    float GetMainHeight()
    {
        return mainCylinder.localScale.y * 2f;
    }
}
