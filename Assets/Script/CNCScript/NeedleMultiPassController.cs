using UnityEngine;

public class NeedleMultiPassController : MonoBehaviour
{
    [System.Serializable]
    public class CutPass
    {
        public CylinderShrinkWithNeedleMovement cylinder;
    }

    [Header("Movement Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Speed")]
    public float moveSpeed = 0.4f;
    public float scaleSpeed = 0.001f;

    [Header("Passes")]
    public CutPass[] passes;

    [Header("Tool Geometry")]
    public float totalScaleIncrease = 0.001f;

    private int passIndex = 0;
    private float baseScaleY;
    private float stepScaleY;

    private float lockedBackY;   // 🔒 World-space back face

    private enum State { Idle, ScaleUp, Cutting, Retract }
    private State state = State.Idle;

    void Start()
    {
        transform.position = startPoint.position;

        baseScaleY = transform.localScale.y;
        stepScaleY = totalScaleIncrease / Mathf.Max(1, passes.Length - 1);

        LockBackFace();
    }

    public void StartMachining()
    {
        passIndex = 0;
        state = State.Cutting;
        passes[passIndex].cylinder.StartCut();
    }

    void Update()
    {
        switch (state)
        {
            case State.ScaleUp:
                ScaleFromFrontOnly(baseScaleY + stepScaleY * passIndex);

                if (Mathf.Abs(transform.localScale.y -
                    (baseScaleY + stepScaleY * passIndex)) < 0.0001f)
                {
                    passes[passIndex].cylinder.StartCut();
                    state = State.Cutting;
                }
                break;

            case State.Cutting:
                Move(endPoint.position);

                if (Reached(endPoint.position))
                {
                    passes[passIndex].cylinder.StopCut();
                    state = State.Retract;
                }
                break;

            case State.Retract:
                Move(startPoint.position);

                if (Reached(startPoint.position))
                {
                    passIndex++;

                    if (passIndex < passes.Length)
                    {
                        LockBackFace();   // 🔒 lock again before growing
                        state = State.ScaleUp;
                    }
                    else
                    {
                        state = State.Idle;
                    }
                }
                break;
        }
    }

    // ──────────────────────────────
    void Move(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    bool Reached(Vector3 target)
    {
        return Vector3.Distance(transform.position, target) < 0.0001f;
    }

    // ──────────────────────────────
    // 🔒 LOCK BACK FACE IN WORLD SPACE
    void LockBackFace()
    {
        float halfHeight = transform.localScale.y * 0.5f;
        lockedBackY = transform.position.y - halfHeight;
    }

    // 🔧 SCALE ONLY TOWARD FRONT
    void ScaleFromFrontOnly(float targetScaleY)
    {
        Vector3 scale = transform.localScale;

        scale.y = Mathf.MoveTowards(
            scale.y,
            targetScaleY,
            scaleSpeed * Time.deltaTime
        );

        transform.localScale = scale;

        float halfHeight = scale.y * 0.5f;

        // Keep back fixed, grow forward
        transform.position = new Vector3(
            transform.position.x,
            lockedBackY + halfHeight,
            transform.position.z
        );
    }
}
