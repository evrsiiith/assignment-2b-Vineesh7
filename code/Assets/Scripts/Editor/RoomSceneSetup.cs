using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor menu to quickly set up the VR Control Room scene.
/// Use: Window > VR Control Room > Setup Scene
/// </summary>
public class RoomSceneSetup : EditorWindow
{
    [MenuItem("Window/VR Control Room/Setup Scene")]
    public static void SetupScene()
    {
        // Create a new scene or use current
        if (EditorUtility.DisplayDialog("VR Control Room Setup",
            "This will set up the Control Room scene.\n\n" +
            "The scene will be procedurally generated at runtime.\n" +
            "A SceneBootstrap object will be created in the current scene.\n\n" +
            "Continue?", "Setup", "Cancel"))
        {
            CreateSceneBootstrap();
        }
    }

    private static void CreateSceneBootstrap()
    {
        // Check if bootstrap already exists
        SceneBootstrap existing = Object.FindObjectOfType<SceneBootstrap>();
        if (existing != null)
        {
            Debug.Log("[RoomSceneSetup] SceneBootstrap already exists in scene.");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        // Create bootstrap
        GameObject bootstrap = new GameObject("SceneBootstrap");
        bootstrap.AddComponent<SceneBootstrap>();

        // Create a dim directional light for minimal ambient
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 0.3f;
        light.color = new Color(1f, 0.96f, 0.84f);
        light.shadows = LightShadows.Soft;
        lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

        // Set render settings
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.2f, 0.2f, 0.25f);
        RenderSettings.skybox = null;

        // Mark scene dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        Selection.activeGameObject = bootstrap;
        Debug.Log("[RoomSceneSetup] Scene setup complete. Press Play to see the generated room.");
    }

    [MenuItem("Window/VR Control Room/About")]
    public static void ShowAbout()
    {
        EditorUtility.DisplayDialog("VR Interactive Control Room",
            "VR Interactive Control Room — Unity 3D Scene\n\n" +
            "Features:\n" +
            "• Light Switch — Toggle room lighting\n" +
            "• Control Panel Button — Activates door mechanism\n" +
            "• Exit Door — Opens when button pressed with lights on\n" +
            "• Moveable Crate — Physics-enabled grab and release\n\n" +
            "Conditional Logic:\n" +
            "Door opens only when button is pressed AND lights are ON.\n\n" +
            "Requirements: Unity XR Interaction Toolkit, OpenXR", "OK");
    }
}
