using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Procedurally builds the entire VR Control Room scene at runtime.
/// Creates walls, floor, ceiling, door, desk, light switch, button, crate,
/// overhead lights, and XR rig with ray and direct interactors.
/// </summary>
public class RoomBuilder : MonoBehaviour
{
    // Room dimensions: 5m x 5m x 3m
    private const float RoomWidth = 5f;
    private const float RoomDepth = 5f;
    private const float RoomHeight = 3f;
    private const float WallThickness = 0.15f;

    // Materials (created at runtime)
    private Material wallMaterial;
    private Material floorMaterial;
    private Material ceilingMaterial;
    private Material doorMaterial;
    private Material deskMaterial;
    private Material switchMaterial;
    private Material buttonMaterial;
    private Material crateMaterial;
    private Material switchPlateMaterial;
    private Material panelMaterial;

    private void Start()
    {
        CreateMaterials();
        BuildRoom();
    }

    private void CreateMaterials()
    {
        Shader standard = Shader.Find("Standard");

        wallMaterial = new Material(standard);
        wallMaterial.color = new Color(0.85f, 0.82f, 0.78f); // warm beige

        floorMaterial = new Material(standard);
        floorMaterial.color = new Color(0.35f, 0.25f, 0.18f); // dark wood

        ceilingMaterial = new Material(standard);
        ceilingMaterial.color = new Color(0.92f, 0.92f, 0.90f); // off-white

        doorMaterial = new Material(standard);
        doorMaterial.color = new Color(0.45f, 0.30f, 0.18f); // dark wood door

        deskMaterial = new Material(standard);
        deskMaterial.color = new Color(0.30f, 0.22f, 0.15f); // desk brown

        switchMaterial = new Material(standard);
        switchMaterial.color = Color.white;

        switchPlateMaterial = new Material(standard);
        switchPlateMaterial.color = new Color(0.9f, 0.9f, 0.85f);

        buttonMaterial = new Material(standard);
        buttonMaterial.color = Color.red;

        panelMaterial = new Material(standard);
        panelMaterial.color = new Color(0.3f, 0.3f, 0.35f); // dark grey metal

        crateMaterial = new Material(standard);
        crateMaterial.color = new Color(0.72f, 0.55f, 0.30f); // wooden crate
    }

    private void BuildRoom()
    {
        GameObject roomRoot = new GameObject("ControlRoom");

        // Floor
        CreateBox("Floor", roomRoot.transform,
            new Vector3(0, 0, 0),
            new Vector3(RoomWidth, WallThickness, RoomDepth),
            floorMaterial);

        // Ceiling
        CreateBox("Ceiling", roomRoot.transform,
            new Vector3(0, RoomHeight, 0),
            new Vector3(RoomWidth, WallThickness, RoomDepth),
            ceilingMaterial);

        // Back Wall (Z = -RoomDepth/2)
        CreateBox("BackWall", roomRoot.transform,
            new Vector3(0, RoomHeight / 2f, -RoomDepth / 2f),
            new Vector3(RoomWidth, RoomHeight, WallThickness),
            wallMaterial);

        // Left Wall (X = -RoomWidth/2)
        CreateBox("LeftWall", roomRoot.transform,
            new Vector3(-RoomWidth / 2f, RoomHeight / 2f, 0),
            new Vector3(WallThickness, RoomHeight, RoomDepth),
            wallMaterial);

        // Right Wall (X = +RoomWidth/2)
        CreateBox("RightWall", roomRoot.transform,
            new Vector3(RoomWidth / 2f, RoomHeight / 2f, 0),
            new Vector3(WallThickness, RoomHeight, RoomDepth),
            wallMaterial);

        // Front Wall with door opening (Z = +RoomDepth/2)
        // Left section
        float doorWidth = 1.0f;
        float doorHeight = 2.2f;
        float frontWallZ = RoomDepth / 2f;

        float leftSectionWidth = (RoomWidth - doorWidth) / 2f;
        CreateBox("FrontWallLeft", roomRoot.transform,
            new Vector3(-RoomWidth / 2f + leftSectionWidth / 2f, RoomHeight / 2f, frontWallZ),
            new Vector3(leftSectionWidth, RoomHeight, WallThickness),
            wallMaterial);

        // Right section
        CreateBox("FrontWallRight", roomRoot.transform,
            new Vector3(RoomWidth / 2f - leftSectionWidth / 2f, RoomHeight / 2f, frontWallZ),
            new Vector3(leftSectionWidth, RoomHeight, WallThickness),
            wallMaterial);

        // Top section above door
        float topSectionHeight = RoomHeight - doorHeight;
        CreateBox("FrontWallTop", roomRoot.transform,
            new Vector3(0, doorHeight + topSectionHeight / 2f, frontWallZ),
            new Vector3(doorWidth, topSectionHeight, WallThickness),
            wallMaterial);

        // Door frame
        CreateBox("DoorFrameLeft", roomRoot.transform,
            new Vector3(-doorWidth / 2f - 0.03f, doorHeight / 2f, frontWallZ),
            new Vector3(0.06f, doorHeight, WallThickness + 0.02f),
            deskMaterial);

        CreateBox("DoorFrameRight", roomRoot.transform,
            new Vector3(doorWidth / 2f + 0.03f, doorHeight / 2f, frontWallZ),
            new Vector3(0.06f, doorHeight, WallThickness + 0.02f),
            deskMaterial);

        CreateBox("DoorFrameTop", roomRoot.transform,
            new Vector3(0, doorHeight + 0.03f, frontWallZ),
            new Vector3(doorWidth + 0.12f, 0.06f, WallThickness + 0.02f),
            deskMaterial);

        // === EXIT DOOR ===
        BuildExitDoor(roomRoot.transform, frontWallZ, doorWidth, doorHeight);

        // === OVERHEAD LIGHTS ===
        Light[] lights = BuildOverheadLights(roomRoot.transform);

        // === DESK ===
        BuildDesk(roomRoot.transform);

        // === CONTROL PANEL BUTTON (on desk) ===
        BuildControlPanelButton(roomRoot.transform);

        // === LIGHT SWITCH (on left wall) ===
        BuildLightSwitch(roomRoot.transform);

        // === MOVEABLE CRATE ===
        BuildMoveableCrate(roomRoot.transform);

        // === ROOM MANAGER ===
        BuildRoomManager(roomRoot, lights);

        // === XR RIG ===
        BuildXRRig();
    }

    private ExitDoor BuildExitDoor(Transform parent, float frontWallZ, float doorWidth, float doorHeight)
    {
        // Door pivot at hinge edge
        GameObject doorPivotObj = new GameObject("DoorPivot");
        doorPivotObj.transform.SetParent(parent);
        doorPivotObj.transform.localPosition = new Vector3(-doorWidth / 2f, 0, frontWallZ);

        // Door panel (offset so it rotates around the hinge)
        GameObject doorPanel = CreateBox("DoorPanel", doorPivotObj.transform,
            new Vector3(doorWidth / 2f, doorHeight / 2f, 0),
            new Vector3(doorWidth, doorHeight, 0.05f),
            doorMaterial);

        // Door handle
        CreateBox("DoorHandle", doorPanel.transform,
            new Vector3(0.35f, 0f, -0.05f),
            new Vector3(0.1f, 0.04f, 0.08f),
            panelMaterial);

        // Add ExitDoor script
        ExitDoor exitDoor = doorPivotObj.AddComponent<ExitDoor>();

        // Use serialized field assignment via reflection (or set via inspector in editor)
        // We'll set the doorPivot field
        SetPrivateField(exitDoor, "doorPivot", doorPivotObj.transform);
        SetPrivateField(exitDoor, "openAngle", -90f);
        SetPrivateField(exitDoor, "openSpeed", 1.5f);

        return exitDoor;
    }

    private Light[] BuildOverheadLights(Transform parent)
    {
        Light[] lights = new Light[2];

        for (int i = 0; i < 2; i++)
        {
            GameObject lightObj = new GameObject($"OverheadLight_{i}");
            lightObj.transform.SetParent(parent);
            float xPos = (i == 0) ? -1.2f : 1.2f;
            lightObj.transform.localPosition = new Vector3(xPos, RoomHeight - 0.1f, 0);

            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 8f;
            light.intensity = 1.5f;
            light.color = new Color(1f, 0.95f, 0.85f); // warm white
            light.shadows = LightShadows.Soft;
            lights[i] = light;

            // Light fixture visual
            CreateBox("LightFixture", lightObj.transform,
                Vector3.zero,
                new Vector3(0.4f, 0.05f, 0.4f),
                ceilingMaterial);
        }

        return lights;
    }

    private void BuildDesk(Transform parent)
    {
        // Desk position: against back wall, slightly right
        Vector3 deskPos = new Vector3(1.0f, 0.4f, -2.0f);

        // Desk top
        CreateBox("DeskTop", parent,
            deskPos,
            new Vector3(1.2f, 0.06f, 0.6f),
            deskMaterial);

        float legHeight = 0.37f;
        float legSize = 0.06f;
        Vector3[] legOffsets = {
            new Vector3(-0.55f, -legHeight / 2f - 0.03f, -0.25f),
            new Vector3(0.55f, -legHeight / 2f - 0.03f, -0.25f),
            new Vector3(-0.55f, -legHeight / 2f - 0.03f, 0.25f),
            new Vector3(0.55f, -legHeight / 2f - 0.03f, 0.25f)
        };

        for (int i = 0; i < 4; i++)
        {
            CreateBox($"DeskLeg_{i}", parent,
                deskPos + legOffsets[i],
                new Vector3(legSize, legHeight, legSize),
                deskMaterial);
        }
    }

    private void BuildControlPanelButton(Transform parent)
    {
        // Panel on desk
        Vector3 panelPos = new Vector3(1.0f, 0.44f, -2.0f);

        GameObject panel = CreateBox("ControlPanel", parent,
            panelPos,
            new Vector3(0.3f, 0.02f, 0.2f),
            panelMaterial);

        // Button cap
        GameObject buttonObj = new GameObject("ControlPanelButton");
        buttonObj.transform.SetParent(parent);
        buttonObj.transform.localPosition = panelPos + new Vector3(0, 0.03f, 0);

        // Button visual (cylinder-like using a squashed cube)
        GameObject buttonCap = CreateBox("ButtonCap", buttonObj.transform,
            Vector3.zero,
            new Vector3(0.08f, 0.04f, 0.08f),
            buttonMaterial);

        // Add interaction components
        var interactable = buttonObj.AddComponent<XRSimpleInteractable>();
        var collider = buttonObj.AddComponent<SphereCollider>();
        collider.radius = 0.06f;

        // Add ControlPanelButton script
        ControlPanelButton buttonScript = buttonObj.AddComponent<ControlPanelButton>();
        SetPrivateField(buttonScript, "buttonCap", buttonCap.transform);
        SetPrivateField(buttonScript, "buttonRenderer", buttonCap.GetComponent<Renderer>());
    }

    private void BuildLightSwitch(Transform parent)
    {
        // On left wall, about 1.3m high
        Vector3 switchPos = new Vector3(-RoomWidth / 2f + WallThickness + 0.02f, 1.3f, -0.5f);

        // Switch plate
        GameObject switchPlate = CreateBox("SwitchPlate", parent,
            switchPos,
            new Vector3(0.08f, 0.12f, 0.01f),
            switchPlateMaterial);

        // Switch object
        GameObject switchObj = new GameObject("LightSwitch");
        switchObj.transform.SetParent(parent);
        switchObj.transform.localPosition = switchPos + new Vector3(0.01f, 0, 0.01f);

        // Switch handle
        GameObject switchHandle = CreateBox("SwitchHandle", switchObj.transform,
            Vector3.zero,
            new Vector3(0.03f, 0.05f, 0.02f),
            switchMaterial);

        // Add interaction components
        var interactable = switchObj.AddComponent<XRSimpleInteractable>();
        var collider = switchObj.AddComponent<BoxCollider>();
        collider.size = new Vector3(0.1f, 0.14f, 0.06f);

        // Add LightSwitch script
        LightSwitch switchScript = switchObj.AddComponent<LightSwitch>();
        SetPrivateField(switchScript, "switchHandle", switchHandle.transform);
    }

    private void BuildMoveableCrate(Transform parent)
    {
        Vector3 cratePos = new Vector3(-1.5f, 0.35f, 1.0f);

        GameObject crate = CreateBox("MoveableCrate", parent,
            cratePos,
            new Vector3(0.5f, 0.5f, 0.5f),
            crateMaterial);

        // Add physics
        Rigidbody rb = crate.AddComponent<Rigidbody>();
        rb.mass = 5f;
        rb.useGravity = true;

        // Add XR Grab Interactable
        XRGrabInteractable grabInteractable = crate.AddComponent<XRGrabInteractable>();
        grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
        grabInteractable.throwOnDetach = true;

        // Add MoveableCrate script
        crate.AddComponent<MoveableCrate>();

        // Add cross pattern detail lines on crate faces
        CreateBox("CrateDetail1", crate.transform,
            new Vector3(0, 0, 0.251f),
            new Vector3(0.48f, 0.02f, 0.001f),
            deskMaterial);

        CreateBox("CrateDetail2", crate.transform,
            new Vector3(0, 0, 0.251f),
            new Vector3(0.02f, 0.48f, 0.001f),
            deskMaterial);
    }

    private void BuildRoomManager(GameObject roomRoot, Light[] lights)
    {
        RoomManager rm = roomRoot.AddComponent<RoomManager>();

        // Find the ExitDoor
        ExitDoor exitDoor = roomRoot.GetComponentInChildren<ExitDoor>();

        SetPrivateField(rm, "roomLights", lights);
        SetPrivateField(rm, "exitDoor", exitDoor);
        SetPrivateField(rm, "lightsOn", true);
    }

    private void BuildXRRig()
    {
        // Create XR Origin (Action-based)
        GameObject xrOrigin = new GameObject("XR Origin");
        xrOrigin.transform.position = new Vector3(0, 0, -1.0f);

        var xrOriginComp = xrOrigin.AddComponent<Unity.XR.CoreUtils.XROrigin>();

        // Camera Offset
        GameObject cameraOffset = new GameObject("Camera Offset");
        cameraOffset.transform.SetParent(xrOrigin.transform);
        cameraOffset.transform.localPosition = Vector3.zero;

        // Main Camera
        GameObject cameraObj = new GameObject("Main Camera");
        cameraObj.transform.SetParent(cameraOffset.transform);
        cameraObj.transform.localPosition = new Vector3(0, 1.7f, 0);
        cameraObj.tag = "MainCamera";

        Camera cam = cameraObj.AddComponent<Camera>();
        cam.nearClipPlane = 0.01f;
        cam.fieldOfView = 90f;

        cameraObj.AddComponent<AudioListener>();
        var trackedPoseDriver = cameraObj.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();

        xrOriginComp.Camera = cam;
        xrOriginComp.CameraFloorOffsetObject = cameraOffset;

        // Left Controller
        BuildController("LeftHand Controller", cameraOffset.transform, true);

        // Right Controller
        BuildController("RightHand Controller", cameraOffset.transform, false);

        // Locomotion System
        GameObject locomotion = new GameObject("Locomotion System");
        locomotion.transform.SetParent(xrOrigin.transform);
        var locoSystem = locomotion.AddComponent<LocomotionSystem>();
        locoSystem.xrOrigin = xrOriginComp;

        // Continuous Move Provider
        var moveProvider = locomotion.AddComponent<ActionBasedContinuousMoveProvider>();
        moveProvider.system = locoSystem;

        // Continuous Turn Provider
        var turnProvider = locomotion.AddComponent<ActionBasedContinuousTurnProvider>();
        turnProvider.system = locoSystem;
        turnProvider.turnSpeed = 60f;
    }

    private void BuildController(string name, Transform parent, bool isLeft)
    {
        GameObject controller = new GameObject(name);
        controller.transform.SetParent(parent);
        controller.transform.localPosition = new Vector3(isLeft ? -0.2f : 0.2f, 1.3f, 0.4f);

        var actionController = controller.AddComponent<ActionBasedController>();
        var trackedPoseDriver = controller.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();

        // Ray Interactor
        GameObject rayInteractorObj = new GameObject("Ray Interactor");
        rayInteractorObj.transform.SetParent(controller.transform);
        rayInteractorObj.transform.localPosition = Vector3.zero;

        var rayInteractor = rayInteractorObj.AddComponent<XRRayInteractor>();
        var lineRenderer = rayInteractorObj.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.005f;
        var lineVisual = rayInteractorObj.AddComponent<XRInteractorLineVisual>();
        lineVisual.lineLength = 5f;

        // Direct Interactor (for grabbing)
        GameObject directInteractorObj = new GameObject("Direct Interactor");
        directInteractorObj.transform.SetParent(controller.transform);
        directInteractorObj.transform.localPosition = Vector3.zero;

        var directInteractor = directInteractorObj.AddComponent<XRDirectInteractor>();
        var directCollider = directInteractorObj.AddComponent<SphereCollider>();
        directCollider.isTrigger = true;
        directCollider.radius = 0.1f;
    }

    // Utility methods

    private GameObject CreateBox(string name, Transform parent, Vector3 localPos, Vector3 size, Material mat)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = size;
        obj.transform.localRotation = Quaternion.identity;

        if (mat != null)
        {
            obj.GetComponent<Renderer>().material = mat;
        }

        return obj;
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public);

        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"[RoomBuilder] Field '{fieldName}' not found on {target.GetType().Name}");
        }
    }
}
