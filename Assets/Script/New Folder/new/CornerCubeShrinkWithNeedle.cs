using UnityEngine;

public class CubeShrinkOnNeedle22 : MonoBehaviour
{
    public enum ShrinkAxis { X, Z }

    [Header("Shrink Settings")]
    public ShrinkAxis shrinkAxis;

    [Tooltip("For X: left/right | For Z: back/front")]
    public bool negativeSide = true;

    public float shrinkSpeed = 0.2f;

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

        float delta = shrinkSpeed * Time.deltaTime;

        // 🔹 X-AXIS CUBES
        if (shrinkAxis == ShrinkAxis.X)
        {
            if (scale.x <= EPS)
            {
                DisableCube();
                return;
            }

            float shrink = Mathf.Min(delta, scale.x);

            scale.x -= shrink;
            pos.x += (negativeSide ? -1f : 1f) * (shrink / 2f);
        }
        // 🔹 Z-AXIS CUBES (KEY FIX)
        else
        {
            if (scale.x <= EPS)
            {
                DisableCube();
                return;
            }

            float shrink = Mathf.Min(delta, scale.x);

            // 👉 shrink X scale
            scale.x -= shrink;

            // 👉 move along Z direction
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