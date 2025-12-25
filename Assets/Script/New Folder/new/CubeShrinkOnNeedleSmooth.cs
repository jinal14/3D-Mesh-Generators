using UnityEngine;

public class CubeShrinkOnNeedle2 : MonoBehaviour
{
    public enum ShrinkAxis { X, Z }

    [Header("Shrink Settings")]
    public ShrinkAxis shrinkAxis;

    [Tooltip("For X: left/right | For Z: back/front")]
    public bool negativeSide = true;

    [Header("Needle Reference")]
    public Transform needle;   // 🔥 ADD THIS

    [Tooltip("Multiplier to match needle feed rate")]
    public float shrinkMultiplier = 1f; // fine-tune if needed

    private const float EPS = 0.001f;
    private bool shrinkActive = false;

    private float lastNeedleY;

    public void StartShrink()
    {
        if (needle == null) return;

        shrinkActive = true;
        lastNeedleY = needle.position.y; // 🔥 store start position
    }

    public void StopShrink()
    {
        shrinkActive = false;
    }

    private void Update()
    {
        if (!shrinkActive || needle == null) return;

        // 🔥 needle movement this frame
        float needleDelta = Mathf.Abs(lastNeedleY - needle.position.y);
        lastNeedleY = needle.position.y;

        if (needleDelta <= 0f) return; // needle not moving → no shrink

        float shrink = needleDelta * shrinkMultiplier;

        Vector3 scale = transform.localScale;
        Vector3 pos = transform.position;

        // 🔹 X-AXIS CUBES
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
        // 🔹 Z-AXIS CUBES
        else
        {
            if (scale.z <= EPS)
            {
                DisableCube();
                return;
            }

            shrink = Mathf.Min(shrink, scale.z);
            scale.z -= shrink;
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
