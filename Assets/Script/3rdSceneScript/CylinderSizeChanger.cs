using UnityEngine;

public class CylinderSizeChanger : MonoBehaviour
{
    public Transform cylinder;  // assign your cylinder here in inspector

    public void SetCylinderSizeMM(float diameterMM)
    {
        float diameterMeters = diameterMM / 1000f; // mm → meters

        cylinder.localScale = new Vector3(
            diameterMeters,   // scale.x = diameter
            0.025f,           // height always = 50 mm (0.05 m)
            diameterMeters    // scale.z = diameter
        );

        Debug.Log("Cylinder diameter: " + diameterMM + "mm , Height: 50mm");
    }
}