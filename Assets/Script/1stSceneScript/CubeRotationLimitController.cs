using UnityEngine;
using UnityEngine.UI;

public class CubeRotationLimitController : MonoBehaviour
{
    [Header("Assign Cube")]
    public Transform cube;

    [Header("Scrollbars")]
    public Scrollbar xScrollbar;
    public Scrollbar yScrollbar;
    public Scrollbar zScrollbar;

    [Header("Text Fields")]
    public Text xText;
    public Text yText;
    public Text zText;

    [Header("Lock Button")]
    public Button lockButton;

    private float xRot, yRot, zRot;
    private bool isLocked = false;

    void Start()
    {
        xScrollbar.onValueChanged.AddListener(OnXChanged);
        yScrollbar.onValueChanged.AddListener(OnYChanged);
        zScrollbar.onValueChanged.AddListener(OnZChanged);

        lockButton.onClick.AddListener(LockRotation);

        // Start Z at 35°  
        zRot = 35f;
        zScrollbar.value = 0f;
        zText.text = "35";
    }

    // ========================= X AXIS =========================
    void OnXChanged(float value)
    {
        if (isLocked)
        {
            xScrollbar.value = 90f / 360f;
            return;
        }

        // Limit scrollbar movement (0 → 90° = value 0 → 0.25)
        float maxValue = 90f / 360f;

        if (value > maxValue)
        {
            value = maxValue;
            xScrollbar.value = maxValue;  // PREVENT HANDLE FROM MOVING
        }

        xRot = value * 360f;
        xText.text = Mathf.RoundToInt(xRot).ToString();

        ApplyRotation();
    }

    // ========================= Y AXIS =========================
    void OnYChanged(float value)
    {
        if (isLocked)
        {
            yScrollbar.value = 35f / 360f;
            return;
        }

        float maxValue = 35f / 360f;

        if (value > maxValue)
        {
            value = maxValue;
            yScrollbar.value = maxValue;
        }

        yRot = value * 360f;
        yText.text = Mathf.RoundToInt(yRot).ToString();

        ApplyRotation();
    }

    // ========================= Z AXIS =========================
    void OnZChanged(float value)
    {
        if (isLocked)
        {
            zScrollbar.value = 1f;
            return;
        }

        // Z range 35° → 105° => scrollbar 0 → 1 maps to 35 → 105
        zRot = Mathf.Lerp(35f, 105f, value);

        zText.text = Mathf.RoundToInt(zRot).ToString();
        ApplyRotation();
    }

    // ======================= LOCK ROTATION =======================
    void LockRotation()
    {
        isLocked = true;

        // Lock at maximums
        xRot = 90f;
        yRot = 35f;
        zRot = 105f;

        xScrollbar.value = 90f / 360f;
        yScrollbar.value = 35f / 360f;
        zScrollbar.value = 1f;

        ApplyRotation();
    }

    void ApplyRotation()
    {
        cube.localRotation = Quaternion.Euler(xRot, yRot, zRot);
    }
}
