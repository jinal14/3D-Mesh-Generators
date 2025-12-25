using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AxisRotationController : MonoBehaviour
{
    public Transform cube;

    [Header("X Axis UI")]
    public Button xPlusButton;
    public Button xMinusButton;
    public Text xValueText;

    [Header("Y Axis UI")]
    public Button yPlusButton;
    public Button yMinusButton;
    public Text yValueText;

    [Header("Z Axis UI")]
    public Button zPlusButton;
    public Button zMinusButton;
    public Text zValueText;

    private int xValue = 0;
    private int yValue = 0;
    private int zValue = 35;

    private bool isHolding = false;
    private float holdSpeed = 0.15f; // faster repeat

    private void Start()
    {
        // X Axis
        xPlusButton.onClick.AddListener(() => ChangeValue_X(1));
        xMinusButton.onClick.AddListener(() => ChangeValue_X(-1));
        AddHoldEvent(xPlusButton, () => ChangeValue_X(1));
        AddHoldEvent(xMinusButton, () => ChangeValue_X(-1));

        // Y Axis
        yPlusButton.onClick.AddListener(() => ChangeValue_Y(1));
        yMinusButton.onClick.AddListener(() => ChangeValue_Y(-1));
        AddHoldEvent(yPlusButton, () => ChangeValue_Y(1));
        AddHoldEvent(yMinusButton, () => ChangeValue_Y(-1));

        // Z Axis
        zPlusButton.onClick.AddListener(() => ChangeValue_Z(1));
        zMinusButton.onClick.AddListener(() => ChangeValue_Z(-1));
        AddHoldEvent(zPlusButton, () => ChangeValue_Z(1));
        AddHoldEvent(zMinusButton, () => ChangeValue_Z(-1));

        UpdateUI();
        ApplyRotation();
    }

    // ------------------------------- X AXIS -----------------------------------
    void ChangeValue_X(int amount)
    {
        xValue += amount;
        xValue = Mathf.Clamp(xValue, 0, 90);   // LIMIT 0 to 90
        UpdateUI();
        ApplyRotation();
    }

    // ------------------------------- Y AXIS -----------------------------------
    void ChangeValue_Y(int amount)
    {
        yValue += amount;
        yValue = Mathf.Clamp(yValue, 0, 35);   // LIMIT 0 to 35
        UpdateUI();
        ApplyRotation();
    }

    // ------------------------------- Z AXIS -----------------------------------
    void ChangeValue_Z(int amount)
    {
        zValue += amount;
        zValue = Mathf.Clamp(zValue, 35, 105); // LIMIT 35 to 105
        UpdateUI();
        ApplyRotation();
    }

    // ------------------------------- APPLY ROTATION ----------------------------
    void ApplyRotation()
    {
        cube.localRotation = Quaternion.Euler(xValue, yValue, zValue);
    }

    void UpdateUI()
    {
        xValueText.text = xValue.ToString();
        yValueText.text = yValue.ToString();
        zValueText.text = zValue.ToString();
    }

    // ------------------------------- HOLD LOGIC --------------------------------
    void AddHoldEvent(Button btn, System.Action action)
    {
        btn.onClick.AddListener(() => StopHold());  // prevent double action

        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

        // Pointer Down
        EventTrigger.Entry pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => { StartCoroutine(HoldButton(action)); });
        trigger.triggers.Add(pointerDown);

        // Pointer Up
        EventTrigger.Entry pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => { StopHold(); });
        trigger.triggers.Add(pointerUp);
    }

    System.Collections.IEnumerator HoldButton(System.Action action)
    {
        isHolding = true;
        yield return new WaitForSeconds(0.3f); // wait before fast repeat

        while (isHolding)
        {
            action.Invoke();
            yield return new WaitForSeconds(holdSpeed);
        }
    }

    void StopHold()
    {
        isHolding = false;
    }
}
