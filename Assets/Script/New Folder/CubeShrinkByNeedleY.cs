using UnityEngine;

public class CubeShrinkByNeedleHeight : MonoBehaviour
{
    [Header("References")]
    public Transform needleBottomPoint;
    public Transform needle; // full needle object

    private float needleHeight;
    private float initialCubeHeight;
    private float targetCubeHeight;

    private float lastNeedleY;
    private bool active = false;

    private const float EPS = 0.0001f;

    private void Start()
    {
        // Needle height from renderer
        Renderer r = needle.GetComponent<Renderer>();
        needleHeight = r.bounds.size.y;

        initialCubeHeight = transform.localScale.y;
        targetCubeHeight = Mathf.Max(initialCubeHeight - needleHeight, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == needleBottomPoint)
        {
            active = true;
            lastNeedleY = needleBottomPoint.position.y;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == needleBottomPoint)
        {
            active = false;
        }
    }

    private void Update()
    {
        if (!active) return;

        float currentY = needleBottomPoint.position.y;
        float deltaY = lastNeedleY - currentY; // positive when needle goes down
        lastNeedleY = currentY;

        if (deltaY <= 0f) return;

        Vector3 scale = transform.localScale;
        Vector3 pos = transform.position;

        if (scale.y <= targetCubeHeight + EPS)
        {
            DisableCube();
            return;
        }

        float allowedShrink = scale.y - targetCubeHeight;
        float shrink = Mathf.Min(deltaY, allowedShrink);

        // shrink Y
        scale.y -= shrink;

        // move cube down to keep bottom fixed
        pos.y -= shrink / 2f;

        transform.localScale = scale;
        transform.position = pos;
    }

    private void DisableCube()
    {
        active = false;
        gameObject.SetActive(false);
    }
}