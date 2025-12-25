using UnityEngine;

public class NeedleMoveX : MonoBehaviour
{
    [Header("References")]
    public Transform startPoint;   // Home position
    public Transform endPoint;     // Final cut position

    [Header("Movement")]
    public float moveSpeed = 0.4f;

    private bool move = false;

    void Start()
    {
        // Snap needle to start point dynamically
        if (startPoint != null)
            transform.position = startPoint.position;
    }

    void Update()
    {
        if (!move || startPoint == null || endPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            endPoint.position,
            moveSpeed * Time.deltaTime
        );

        // Stop when reached end
        if (Vector3.Distance(transform.position, endPoint.position) < 0.0001f)
        {
            move = false;
        }
    }

    // Called from UI Button
    public void StartNeedle()
    {
        // Reset to start every time
        if (startPoint != null)
            transform.position = startPoint.position;

        move = true;
    }
}