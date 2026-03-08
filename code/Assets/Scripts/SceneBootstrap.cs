using UnityEngine;

/// <summary>
/// Attach this to an empty GameObject in the scene.
/// On Awake, it creates the full RoomBuilder which procedurally generates the room.
/// This is the entry point for the scene.
/// </summary>
public class SceneBootstrap : MonoBehaviour
{
    private void Awake()
    {
        // Check if room already exists
        if (FindObjectOfType<RoomManager>() != null)
        {
            Debug.Log("[SceneBootstrap] Room already exists, skipping build.");
            return;
        }

        // Add RoomBuilder to build the scene
        gameObject.AddComponent<RoomBuilder>();
    }
}
