using UnityEngine;
using UnityEngine.UI;

public class CubeRotationController : MonoBehaviour
{
    [Header("Assign Cube")]
    public Transform cube;

    [Header("Scrollbars (X, Y, Z)")]
    public Scrollbar xScrollbar;
    public Scrollbar yScrollbar;
    public Scrollbar zScrollbar;

    [Header("Text Fields For Degrees")]
    public Text xText;
    public Text yText;
    public Text zText;

    private float xRotation;
    private float yRotation;
    private float zRotation;

    void Start()
    {
        // Assign listeners
        xScrollbar.onValueChanged.AddListener(OnXChanged);
        yScrollbar.onValueChanged.AddListener(OnYChanged);
        zScrollbar.onValueChanged.AddListener(OnZChanged);
    }

    void OnXChanged(float value)
    {
        xRotation = value * 360f;
        xText.text = Mathf.RoundToInt(xRotation).ToString();
        ApplyRotation();
    }

    void OnYChanged(float value)
    {
        yRotation = value * 360f;
        yText.text = Mathf.RoundToInt(yRotation).ToString();
        ApplyRotation();
    }

    void OnZChanged(float value)
    {
        zRotation = value * 360f;
        zText.text = Mathf.RoundToInt(zRotation).ToString();
        ApplyRotation();
    }

    void ApplyRotation()
    {
        if (cube != null)
        {
            cube.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }
    }
}