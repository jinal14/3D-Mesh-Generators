using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CylinderAttachmentManager : MonoBehaviour
{
    [Header("References")]
    public Transform mainCylinder;        
    public GameObject smallPrefab;        // Prefab of the small cylinder (use Unity Cylinder primitive)
    public Transform previewParent;       // Optional parent transform where preview instance will appear (can be the same scene root)
    public Transform attachmentsParent;   // Parent object under which clones will be created
    public Button attachButton;           // Button to create the ring of small cylinders
    public Button clearButton;            // Button to clear attachments

    [Header("Dimensions (meters)")]
    
    
    public float fallbackMainDiameter = 0.8f; 
    public float fallbackMainHeight = 0.05f;  

    [Header("Small cylinder settings")]
    public float smallHeightMeters = 0.05f; 
    public float attachGap = 0.0f;          

    // Internal
    GameObject previewInstance;
    List<GameObject> spawned = new List<GameObject>();
    float selectedDiameterMM = 10f; // default selection

    void Start()
    {
        if (attachButton != null) attachButton.onClick.AddListener(OnAttachPressed);
        if (clearButton != null) clearButton.onClick.AddListener(ClearAttachments);
    }

    // Call this from your size buttons, pass mm value (1,2,4...25)
    public void ShowPreview(float diameterMM)
    {
        selectedDiameterMM = diameterMM;

        // create or reuse preview instance
        if (previewInstance == null)
        {
            previewInstance = Instantiate(smallPrefab, previewParent ? previewParent : transform);
            previewInstance.name = "SmallCylinderPreview";
            // keep preview separate so it doesn't get cleared
        }

        
        float diameterM = diameterMM / 1000f;
        float scaleX = diameterM;                         // diameter -> scale.x (because default diameter=1 unit)
        float scaleY = smallHeightMeters / 2f;            // scale.y = height / 2 (because default height = 2 units)
        previewInstance.transform.localScale = new Vector3(scaleX, scaleY, scaleX);

        
        Vector3 mainPos = mainCylinder ? mainCylinder.position : transform.position;
        float mainRadius = GetMainRadiusMeters();
        float mainHeight = GetMainHeightMeters();

       
        float previewDistance = mainRadius + (diameterM / 2f) + 0.02f; // small offset so it's visible
        Vector3 previewPos = mainPos + new Vector3(previewDistance, -mainHeight/2f + smallHeightMeters/2f, 0f);
        previewInstance.transform.position = previewPos;

        // ensure preview upright rotation
        previewInstance.transform.rotation = Quaternion.identity;
    }
    public void OnAttachPressed()
    {
        ClearAttachments(); // remove previous attachments

        float diameterM = selectedDiameterMM / 1000f;
        int N = Mathf.FloorToInt(360f / selectedDiameterMM);
        if (N < 1) N = 1;

        Vector3 mainPos = mainCylinder ? mainCylinder.position : transform.position;
        float mainRadius = GetMainRadiusMeters();
        float mainHeight = GetMainHeightMeters();

        float smallRadius = diameterM / 2f;

        // radial distance from center for small cylinder centers:
        float placementRadius = mainRadius + smallRadius + attachGap;

        // Y for bottom-aligned placement: main bottom Y + half small height
        // Y for attachment UNDER the main cylinder:
        float yPos = mainPos.y 
                     - (mainHeight / 2f)     // main cylinder bottom
                     - (smallHeightMeters / 2f);  // place small cylinder completely under it


        for (int i = 0; i < N; i++)
        {
            float angleDeg = i * (360f / N);
            float angleRad = angleDeg * Mathf.Deg2Rad;

            float x = mainPos.x + placementRadius * Mathf.Cos(angleRad);
            float z = mainPos.z + placementRadius * Mathf.Sin(angleRad);

            Vector3 spawnPos = new Vector3(x, yPos, z);
            GameObject go = Instantiate(smallPrefab, spawnPos, Quaternion.identity, attachmentsParent ? attachmentsParent : transform);
            go.name = "Small_" + selectedDiameterMM + "mm_" + i;

            // scale small cylinder correctly
            float scaleX = diameterM;
            float scaleY = smallHeightMeters / 2f;
            go.transform.localScale = new Vector3(scaleX, scaleY, scaleX);

            // rotate the small cylinder so it faces outward (optional)
            // If you want the cylinder's local forward to point outward, rotate Y by angleDeg + 90
            // Keep them upright; rotate around Y so their local 0 angle faces outward:
            go.transform.rotation = Quaternion.Euler(0f, -angleDeg, 0f);

            spawned.Add(go);
        }

        Debug.Log($"Attached {spawned.Count} clones of {selectedDiameterMM} mm around main cylinder.");
    }

    // Remove previously spawned attachments
    public void ClearAttachments()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] != null) DestroyImmediate(spawned[i]);
        }
        spawned.Clear();
    }
    
    // else use fallback fields.
    float GetMainRadiusMeters()
    {
        if (mainCylinder != null)
        {
            float mainDiameter = mainCylinder.localScale.x;
            return mainDiameter / 2f;
        }
        return fallbackMainDiameter / 2f;
    }

    float GetMainHeightMeters()
    {
        if (mainCylinder != null)
        {
            // Unity default cylinder height = 2 units, so actual height = 2 * localScale.y
            return 2f * mainCylinder.localScale.y;
        }
        return fallbackMainHeight;
    }
}
