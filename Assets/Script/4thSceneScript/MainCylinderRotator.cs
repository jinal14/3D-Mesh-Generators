using UnityEngine;
using System.Collections;

public class MainCylinderRotator : MonoBehaviour
{
    public Transform mainCylinder;         // Your large cylinder
    public float rotationSpeed = 3f;       // Smooth rotation speed

    // Order of cylinders as placed (12 objects)
    int[] cylinderMM = new int[]
    {
        1, 2, 4, 6, 8, 10, 12, 15, 16, 18, 20, 25
    };

    float angleStep;   // 360 / 12 = 30 degrees
    bool isRotating = false;

    void Start()
    {
        angleStep = 360f / cylinderMM.Length;
    }

    // Call this from button -> RotateToIndex( index )
    public void RotateToMM(int mmValue)
    {
        // Find the index of the selected mm cylinder
        int index = System.Array.IndexOf(cylinderMM, mmValue);

        if (index != -1)
            RotateToIndex(index);
        else
            Debug.LogWarning("MM Value not found in list!");
    }

    // Rotate so that selected index comes to front (0°)
    public void RotateToIndex(int index)
    {
        if (!isRotating)
            StartCoroutine(RotateSmoothly(index));
    }

    IEnumerator RotateSmoothly(int index)
    {
        isRotating = true;

        float targetAngle = index * angleStep;     // Each cylinder sits at +30° position
        targetAngle = -targetAngle;                // Negative because rotation is reverse

        float currentY = mainCylinder.eulerAngles.y;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;

            float newY = Mathf.LerpAngle(currentY, targetAngle, t);

            mainCylinder.rotation = Quaternion.Euler(0f, newY, 0f);

            yield return null;
        }

        // Final snap for accuracy
        mainCylinder.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        isRotating = false;
    }
}
