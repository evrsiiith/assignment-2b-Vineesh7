using UnityEngine;
using TMPro;

/// <summary>
/// Optional HUD that shows current room state for debugging.
/// Attach to a world-space Canvas in the scene.
/// </summary>
public class RoomStateHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;

    private void Update()
    {
        if (RoomManager.Instance == null || statusText == null) return;

        string lightsStatus = RoomManager.Instance.LightsOn ? "ON" : "OFF";
        ExitDoor door = FindObjectOfType<ExitDoor>();
        string doorStatus = (door != null && door.IsDoorOpen) ? "OPEN" : "CLOSED";

        statusText.text = $"Lights: {lightsStatus}\nDoor: {doorStatus}";
    }
}
