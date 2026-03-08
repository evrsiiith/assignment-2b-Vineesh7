using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Provides visual and haptic feedback when hovering over interactive objects.
/// Objects glow when hovered, and the controller vibrates.
/// </summary>
[RequireComponent(typeof(XRBaseInteractable))]
public class InteractionFeedback : MonoBehaviour
{
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 0.5f, 1f);
    [SerializeField] private float hoverEmissionIntensity = 0.3f;
    [SerializeField] private float hapticIntensity = 0.2f;
    [SerializeField] private float hapticDuration = 0.1f;

    private XRBaseInteractable interactable;
    private Renderer objectRenderer;
    private Color originalColor;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        objectRenderer = GetComponentInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    private void OnEnable()
    {
        interactable.hoverEntered.AddListener(OnHoverEnter);
        interactable.hoverExited.AddListener(OnHoverExit);
    }

    private void OnDisable()
    {
        interactable.hoverEntered.RemoveListener(OnHoverEnter);
        interactable.hoverExited.RemoveListener(OnHoverExit);
    }

    private void Start()
    {
        if (objectRenderer != null)
        {
            objectRenderer.GetPropertyBlock(propBlock);
            originalColor = objectRenderer.material.color;
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        // Visual feedback
        if (objectRenderer != null)
        {
            propBlock.SetColor("_EmissionColor", hoverColor * hoverEmissionIntensity);
            objectRenderer.SetPropertyBlock(propBlock);
            objectRenderer.material.EnableKeyword("_EMISSION");
        }

        // Haptic feedback
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            controllerInteractor.xrController?.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (objectRenderer != null)
        {
            propBlock.SetColor("_EmissionColor", Color.black);
            objectRenderer.SetPropertyBlock(propBlock);
            objectRenderer.material.DisableKeyword("_EMISSION");
        }
    }
}
