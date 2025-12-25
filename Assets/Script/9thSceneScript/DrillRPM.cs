using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class DrillRPM : MonoBehaviour
{
    public float rpm = 100f;
    public float minRPM = 0f;
    public float maxRPM = 3000f;

    public bool isRunning = false;

    public Text rpmText;
    public Text speedText;

    public float radius = 0.1f;

    private float degreesPerSecond = 0f;
    private float linearSpeed = 0f;

    // Hold functionality
    private bool isHolding = false;
    public float holdSpeed = 0.1f; // repeat speed while holding

    // Buttons
    public Button increaseBtn;
    public Button decreaseBtn;

    void Start()
    {
        UpdateAllTexts();

        // Add hold system dynamically
        AddHoldEvent(increaseBtn, IncreaseRPM);
        AddHoldEvent(decreaseBtn, DecreaseRPM);
    }

    void Update()
    {
        if (isRunning)
        {
            degreesPerSecond = (rpm * 360f) / 60f;

            transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f);

            float radPerSecond = rpm * (2f * Mathf.PI / 60f);
            linearSpeed = radPerSecond * radius;
        }
        else
        {
            linearSpeed = 0f;
        }

        UpdateAllTexts();
    }

    public void IncreaseRPM()
    {
        rpm += 5f;
        rpm = Mathf.Clamp(rpm, minRPM, maxRPM);
        UpdateAllTexts();
    }

    public void DecreaseRPM()
    {
        rpm -= 5f;
        rpm = Mathf.Clamp(rpm, minRPM, maxRPM);
        UpdateAllTexts();
    }
    void AddHoldEvent(Button btn, Action action)
    {
        btn.onClick.AddListener(() => StopHold()); // avoid double trigger

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

    IEnumerator HoldButton(Action action)
    {
        isHolding = true;

        // Small delay before fast increase
        yield return new WaitForSeconds(0.3f);

        // Repeat continuously while holding
        while (isHolding)
        {
            action.Invoke(); // run increase or decrease
            yield return new WaitForSeconds(holdSpeed);
        }
    }

    void StopHold()
    {
        isHolding = false;
    }

    public void StartDrill()
    {
        isRunning = true;
    }

    public void StopDrill()
    {
        isRunning = false;
    }

    void UpdateAllTexts()
    {
        if (rpmText != null)
            rpmText.text = rpm.ToString("0000");

        if (speedText != null)
            speedText.text = linearSpeed.ToString("00.00") + " m/s";
    }
}
