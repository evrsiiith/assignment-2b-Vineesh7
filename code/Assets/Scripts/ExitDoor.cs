using UnityEngine;

/// <summary>
/// Exit door that transitions from Closed to Open when activated.
/// The door does not return to closed during the session.
/// Opens via a smooth rotation animation.
/// </summary>
public class ExitDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private AudioSource doorOpenSound;

    [Header("Door Pivot")]
    [SerializeField] private Transform doorPivot;

    private bool isOpen = false;
    private bool isOpening = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private float openProgress = 0f;

    private void Start()
    {
        Transform pivot = doorPivot != null ? doorPivot : transform;
        closedRotation = pivot.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    private void Update()
    {
        if (!isOpening) return;

        Transform pivot = doorPivot != null ? doorPivot : transform;

        openProgress += Time.deltaTime * openSpeed;
        pivot.localRotation = Quaternion.Slerp(closedRotation, openRotation, openProgress);

        if (openProgress >= 1f)
        {
            pivot.localRotation = openRotation;
            isOpening = false;
            isOpen = true;
            Debug.Log("[ExitDoor] Door is now fully open.");
        }
    }

    /// <summary>
    /// Opens the door. Once open, it stays open for the rest of the session.
    /// </summary>
    public void OpenDoor()
    {
        if (isOpen || isOpening)
        {
            Debug.Log("[ExitDoor] Door is already open or opening.");
            return;
        }

        isOpening = true;
        openProgress = 0f;

        if (doorOpenSound != null)
            doorOpenSound.Play();

        Debug.Log("[ExitDoor] Door opening...");
    }

    public bool IsDoorOpen => isOpen;
}
