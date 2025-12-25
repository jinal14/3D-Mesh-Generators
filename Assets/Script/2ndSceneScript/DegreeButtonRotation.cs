using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DegreeButtonRotation : MonoBehaviour
{
    [System.Serializable]
    public class AxisControls
    {
        public Text textField;
        public float value = 0;
        public Button plusButton;
        public Button minusButton;
        [HideInInspector] public bool isHolding = false;
    }

    [Header("Cube")]
    public Transform cube;

    [Header("Axis Controls")]
    public AxisControls X;
    public AxisControls Y;
    public AxisControls Z;

    private float initialHoldDelay = 0.35f; // delay before auto repeat starts
    private float minHoldSpeed = 0.05f;
    private float speedIncrease = 0.02f;

    void Start()
    {
        SetupAxis(X, ChangeX);
        SetupAxis(Y, ChangeY);
        SetupAxis(Z, ChangeZ);
    }

    // --------------- SET BUTTON EVENTS ---------------
    void SetupAxis(AxisControls axis, System.Action<int> action)
    {
        // CLICK = +1 or -1 ONLY
        axis.plusButton.onClick.AddListener(() => action(1));
        axis.minusButton.onClick.AddListener(() => action(-1));

        // HOLD events
        AddHoldEvents(axis.plusButton, () => StartHold(axis, action, 1), () => StopHold(axis));
        AddHoldEvents(axis.minusButton, () => StartHold(axis, action, -1), () => StopHold(axis));
    }

    void AddHoldEvents(Button btn, System.Action onDown, System.Action onUp)
    {
        EventTrigger trig = btn.gameObject.AddComponent<EventTrigger>();

        // Pointer Down
        EventTrigger.Entry down = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener((data) => { onDown(); });
        trig.triggers.Add(down);

        // Pointer Up
        EventTrigger.Entry up = new EventTrigger.Entry();
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener((data) => { onUp(); });
        trig.triggers.Add(up);
    }

    // --------------- HOLD SYSTEM ---------------
    void StartHold(AxisControls axis, System.Action<int> action, int direction)
    {
        axis.isHolding = true;
        StartCoroutine(HoldRoutine(axis, action, direction));
    }

    IEnumerator HoldRoutine(AxisControls axis, System.Action<int> action, int dir)
    {
        // WAIT FIRST → prevents double increment
        yield return new WaitForSeconds(initialHoldDelay);

        float speed = 0.1f;

        while (axis.isHolding)
        {
            action(dir);
            ApplyRotation();

            yield return new WaitForSeconds(speed);

            speed = Mathf.Max(minHoldSpeed, speed - speedIncrease);
        }
    }

    void StopHold(AxisControls axis)
    {
        axis.isHolding = false;
    }

    // --------------- AXIS CHANGE FUNCTIONS ---------------
    void ChangeX(int delta)
    {
        X.value = Mathf.Clamp(X.value + delta, 0, 360);
        X.textField.text = X.value.ToString();
        ApplyRotation();
    }

    void ChangeY(int delta)
    {
        Y.value = Mathf.Clamp(Y.value + delta, 0, 360);
        Y.textField.text = Y.value.ToString();
        ApplyRotation();
    }

    void ChangeZ(int delta)
    {
        Z.value = Mathf.Clamp(Z.value + delta, 0, 360);
        Z.textField.text = Z.value.ToString();
        ApplyRotation();
    }

    // --------------- APPLY ROTATION ---------------
    void ApplyRotation()
    {
        cube.localRotation = Quaternion.Euler(X.value, Y.value, Z.value);
    }
}
