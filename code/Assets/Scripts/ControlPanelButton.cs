using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Large push-button on the desk that activates the door mechanism.
/// User presses via trigger press or direct physical controller contact.
/// Door only opens if lights are on (conditional logic enforced by RoomManager).
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class ControlPanelButton : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Transform buttonCap;
    [SerializeField] private float pressDepth = 0.02f;
    [SerializeField] private float pressSpeed = 8f;
    [SerializeField] private AudioSource pressSound;

    [Header("Button Light")]
    [SerializeField] private Renderer buttonRenderer;
    [SerializeField] private Color defaultColor = Color.red;
    [SerializeField] private Color pressedColor = Color.green;

    private XRSimpleInteractable interactable;
    private Vector3 originalLocalPos;
    private Vector3 pressedLocalPos;
    private bool isPressed = false;
    private bool isAnimating = false;
    private float animTimer = 0f;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnPress);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnPress);
    }

    private void Start()
    {
        if (buttonCap != null)
        {
            originalLocalPos = buttonCap.localPosition;
            pressedLocalPos = originalLocalPos + Vector3.down * pressDepth;
        }

        SetButtonColor(defaultColor);
    }

    private void Update()
    {
        if (!isAnimating || buttonCap == null) return;

        animTimer += Time.deltaTime * pressSpeed;

        if (isPressed)
        {
            // Animate pressing down
            buttonCap.localPosition = Vector3.Lerp(originalLocalPos, pressedLocalPos, animTimer);
            if (animTimer >= 1f)
            {
                animTimer = 0f;
                isPressed = false; // start return
            }
        }
        else
        {
            // Animate releasing back up
            buttonCap.localPosition = Vector3.Lerp(pressedLocalPos, originalLocalPos, animTimer);
            if (animTimer >= 1f)
            {
                isAnimating = false;
                buttonCap.localPosition = originalLocalPos;
                SetButtonColor(defaultColor);
            }
        }
    }

    private void OnPress(SelectEnterEventArgs args)
    {
        if (pressSound != null)
            pressSound.Play();

        // Start press animation
        isPressed = true;
        isAnimating = true;
        animTimer = 0f;

        SetButtonColor(pressedColor);

        // Notify the room manager
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.OnButtonPressed();
        }

        Debug.Log("[ControlPanelButton] Button pressed.");
    }

    private void SetButtonColor(Color color)
    {
        if (buttonRenderer != null)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            buttonRenderer.GetPropertyBlock(block);
            block.SetColor("_Color", color);
            buttonRenderer.SetPropertyBlock(block);
        }
    }
}
