using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Wall-mounted toggle switch that controls overhead room lighting.
/// User points at switch and presses trigger to toggle lights on/off.
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class LightSwitch : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Transform switchHandle;
    [SerializeField] private float onAngle = 30f;
    [SerializeField] private float offAngle = -30f;
    [SerializeField] private AudioSource toggleSound;

    private XRSimpleInteractable interactable;
    private bool isOn = true;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnToggle);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnToggle);
    }

    private void Start()
    {
        UpdateSwitchVisual();
    }

    private void OnToggle(SelectEnterEventArgs args)
    {
        isOn = !isOn;
        UpdateSwitchVisual();

        if (toggleSound != null)
            toggleSound.Play();

        // Notify the room manager
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.ToggleLights();
        }

        Debug.Log($"[LightSwitch] Toggled to: {(isOn ? "ON" : "OFF")}");
    }

    private void UpdateSwitchVisual()
    {
        if (switchHandle != null)
        {
            float angle = isOn ? onAngle : offAngle;
            switchHandle.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }
    }
}
