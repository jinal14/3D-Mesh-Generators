using UnityEngine;
using System.Collections;

public class RingRotator : MonoBehaviour
{
    public Transform positionsParent;      // Parent of the 12 cylinders
    public float rotateSpeed = 3f;

    public int[] cylinderMM = new int[]
    {
        1, 2, 4, 6, 8, 10, 12, 15, 16, 18, 20, 25
    };

    float angleStep;
    bool isRotating = false;

    void Start()
    {
        angleStep = 360f / cylinderMM.Length;
    }

    public void RotateToMM(int mmValue)
    {
        int index = System.Array.IndexOf(cylinderMM, mmValue);
        if (index != -1)
            RotateToIndex(index);
    }

    public void RotateToIndex(int index)
    {
        if (!isRotating)
            StartCoroutine(RotateSmooth(index));
    }

    IEnumerator RotateSmooth(int index)
    {
        isRotating = true;

        float targetAngle = index * angleStep;
        targetAngle = -targetAngle;

        float currentY = positionsParent.localEulerAngles.y;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * rotateSpeed;

            float newY = Mathf.LerpAngle(currentY, targetAngle, t);
            positionsParent.localRotation = Quaternion.Euler(0, newY, 0);

            yield return null;
        }

        positionsParent.localRotation = Quaternion.Euler(0, targetAngle, 0);

        isRotating = false;
    }
}