using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Physics-enabled crate that the user can pick up using the grip button,
/// move through the scene, and release to drop under simulated gravity.
/// Uses XR Grab Interactable for native VR grab and release.
/// </summary>
[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class MoveableCrate : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private float throwForceMultiplier = 1.5f;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void Start()
    {
        // Ensure physics are configured correctly
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.mass = 5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;

        // Configure grab interactable
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.throwOnDetach = true;
        grabInteractable.throwSmoothingDuration = 0.25f;
        grabInteractable.throwVelocityScale = throwForceMultiplier;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("[MoveableCrate] Crate grabbed.");
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("[MoveableCrate] Crate released.");
    }
}
