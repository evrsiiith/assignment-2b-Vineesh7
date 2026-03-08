using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Manages the overall room state: lighting and door conditions.
/// Enforces the conditional rule: door opens only if button is pressed while lights are on.
/// </summary>
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Room State")]
    [SerializeField] private bool lightsOn = true;

    public bool LightsOn
    {
        get => lightsOn;
        set
        {
            lightsOn = value;
            OnLightsStateChanged();
        }
    }

    [Header("References")]
    [SerializeField] private Light[] roomLights;
    [SerializeField] private ExitDoor exitDoor;
    [SerializeField] private Material emissiveCeilingMaterial;

    private bool buttonHasBeenPressed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Initialize lighting state
        UpdateLighting();
    }

    /// <summary>
    /// Called when the light switch is toggled.
    /// </summary>
    public void ToggleLights()
    {
        LightsOn = !LightsOn;
    }

    /// <summary>
    /// Called when the control panel button is pressed.
    /// The door opens only if lights are currently on.
    /// </summary>
    public void OnButtonPressed()
    {
        buttonHasBeenPressed = true;

        if (lightsOn)
        {
            TryOpenDoor();
        }
        else
        {
            Debug.Log("[RoomManager] Button pressed but lights are off — door remains closed.");
        }
    }

    private void TryOpenDoor()
    {
        if (exitDoor != null && buttonHasBeenPressed && lightsOn)
        {
            exitDoor.OpenDoor();
            Debug.Log("[RoomManager] Conditions met — opening door.");
        }
    }

    private void OnLightsStateChanged()
    {
        UpdateLighting();
    }

    private void UpdateLighting()
    {
        if (roomLights != null)
        {
            foreach (var light in roomLights)
            {
                if (light != null)
                    light.enabled = lightsOn;
            }
        }

        // Update emissive ceiling material if assigned
        if (emissiveCeilingMaterial != null)
        {
            if (lightsOn)
            {
                emissiveCeilingMaterial.EnableKeyword("_EMISSION");
                emissiveCeilingMaterial.SetColor("_EmissionColor", Color.white * 2f);
            }
            else
            {
                emissiveCeilingMaterial.DisableKeyword("_EMISSION");
                emissiveCeilingMaterial.SetColor("_EmissionColor", Color.black);
            }
        }

        // Adjust ambient lighting
        RenderSettings.ambientIntensity = lightsOn ? 1.0f : 0.15f;
    }
}
