using UnityEngine;

public class CubeShrinkOnNeedle : MonoBehaviour
{
    public enum ShrinkAxis { X, Z }

    [Header("Shrink Settings")]
    public ShrinkAxis shrinkAxis;

    [Tooltip("For X: left/right | For Z: back/front")]
    public bool negativeSide = true;

    [Tooltip("Slow shrink value (0.05 – 0.2 recommended)")]
    public float shrinkSpeed = 0.1f;

    private const float EPS = 0.001f;
    private bool shrinkActive = false;

    public void StartShrink()
    {
        shrinkActive = true;
    }

    public void StopShrink()
    {
        shrinkActive = false;
    }

    private void Update()
    {
        if (!shrinkActive) return;

        Vector3 scale = transform.localScale;
        Vector3 pos = transform.position;

        float shrink = shrinkSpeed * Time.deltaTime;

        // 🔹 X-AXIS CUBES (LEFT / RIGHT)
        if (shrinkAxis == ShrinkAxis.X)
        {
            if (scale.x <= EPS)
            {
                DisableCube();
                return;
            }

            shrink = Mathf.Min(shrink, scale.x);

            scale.x -= shrink;
            pos.x += (negativeSide ? -1f : 1f) * (shrink / 2f);
        }
        // 🔹 Z-AXIS CUBES (BACK / FRONT) ✅ FIXED
        else
        {
            if (scale.z <= EPS)
            {
                DisableCube();
                return;
            }

            shrink = Mathf.Min(shrink, scale.z);

            // ✅ shrink Z scale (NOT X)
            scale.z -= shrink;

            // ✅ move only in Z
            pos.z += (negativeSide ? -1f : 1f) * (shrink / 2f);
        }

        transform.localScale = scale;
        transform.position = pos;
    }

    private void DisableCube()
    {
        shrinkActive = false;
        gameObject.SetActive(false);
    }
}