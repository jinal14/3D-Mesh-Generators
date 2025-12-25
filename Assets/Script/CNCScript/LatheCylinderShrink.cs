using UnityEngine;

public class CylinderShrinkWithNeedleMovement : MonoBehaviour
{
    public Transform needle;
    public float minYScale = 0.4f;

    private bool cutting;
    private float lastNeedleX;
    private float lockedTopY;

    void Start()
    {
        lockedTopY = transform.localPosition.y + (transform.localScale.y * 1.7f);
    }

    // 🔑 MUST BE LateUpdate
    void LateUpdate()
    {
        if (!cutting) return;

        float currentX = needle.position.x;
        float deltaX = lastNeedleX - currentX;
        lastNeedleX = currentX;

        if (deltaX <= 0f) return;

        Vector3 scale = transform.localScale;
        scale.y -= deltaX;
        scale.y = Mathf.Max(scale.y, minYScale);
        transform.localScale = scale;

        // 🔒 Keep one side fixed
        Vector3 pos = transform.localPosition;
        pos.y = lockedTopY - (scale.y * 1.7f);
        transform.localPosition = pos;
    }

    public void StartCut()
    {
        cutting = true;
        lastNeedleX = needle.position.x;
    }

    public void StopCut()
    {
        cutting = false;
    }
}