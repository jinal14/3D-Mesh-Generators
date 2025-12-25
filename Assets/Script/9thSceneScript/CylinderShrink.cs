using UnityEngine;
using System.Collections;
using System;

public class CylinderShrink : MonoBehaviour
{
    public Transform cylinder;
    public float minWorldHeight = 0.0f;
    [HideInInspector] public float drillSpeed;

    private bool isShrinking = false;

    public event Action OnShrinkFinished;

    void Start()
    {
        // Cache once (performance)
        startWorldHeight = cylinder.GetComponent<Renderer>().bounds.size.y;
    }

    private float startWorldHeight;

    public void ShrinkNow()
    {
        if (!isShrinking)
            StartCoroutine(ShrinkRoutine());
    }

    IEnumerator ShrinkRoutine()
    {
        isShrinking = true;

        Renderer rend = cylinder.GetComponent<Renderer>();

        while (true)
        {
            float currentHeight = rend.bounds.size.y;

            if (currentHeight <= minWorldHeight + 0.0001f)
                break;

            float shrinkAmount = drillSpeed * Time.deltaTime;

            float newWorldHeight = Mathf.Max(currentHeight - shrinkAmount, minWorldHeight);

            float scaleRatio = newWorldHeight / currentHeight;

            Vector3 s = cylinder.localScale;
            cylinder.localScale = new Vector3(s.x, s.y * scaleRatio, s.z);

            cylinder.position -= new Vector3(0, shrinkAmount * 0.5f, 0);

            yield return null;
        }

        // ✅ FINISH
        isShrinking = false;

        // 🔥 disable cylinder when fully shrunk
        cylinder.gameObject.SetActive(false);

        OnShrinkFinished?.Invoke();
    }
}