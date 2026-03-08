[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/FDtpdb9Z)

# VR Interactive Control Room — Unity 3D

## Overview

A Virtual Reality scene implemented in Unity 3D featuring a small interior control room (5m × 5m × 3m) that the user can explore and interact with using a VR headset and controllers. The scene demonstrates interactive object behaviour, user-driven state transitions, and conditional logic within a VR environment.

## Interactive Objects

| Object                   | Interaction            | Description                                                   |
| ------------------------ | ---------------------- | ------------------------------------------------------------- |
| **Light Switch**         | Toggle (Trigger)       | Wall-mounted toggle switch controlling overhead room lighting |
| **Control Panel Button** | Press (Trigger/Direct) | Push-button on desk that activates the door mechanism         |
| **Exit Door**            | Automatic              | Hinged door on far wall — opens when conditions are met       |
| **Moveable Crate**       | Grab & Release (Grip)  | Physics-enabled wooden crate — pick up and reposition         |

## Conditional Logic

The **Exit Door** opens **if and only if** the Control Panel Button is pressed **while the room lighting is ON**.

- If the button is pressed while lights are OFF → door stays closed
- Both conditions (button pressed + lights active) must be satisfied simultaneously

## State Tracking

- **Lighting State**: Toggles between ON / OFF via the Light Switch
- **Door State**: Transitions from CLOSED → OPEN (one-way, does not reclose during session)

## Project Structure

```
Assets/
├── Scenes/
│   └── ControlRoom.unity        # Main scene (contains SceneBootstrap)
├── Scripts/
│   ├── SceneBootstrap.cs         # Entry point — triggers room generation
│   ├── RoomBuilder.cs            # Procedurally builds the entire room + XR rig
│   ├── RoomManager.cs            # Central state manager (lights, door logic)
│   ├── LightSwitch.cs            # Toggle interaction for room lights
│   ├── ControlPanelButton.cs     # Push-button interaction for door
│   ├── ExitDoor.cs               # Door open animation controller
│   ├── MoveableCrate.cs          # XR Grab Interactable physics crate
│   ├── InteractionFeedback.cs    # Hover glow + haptic feedback
│   ├── RoomStateHUD.cs           # Optional debug HUD
│   └── Editor/
│       └── RoomSceneSetup.cs     # Editor menu for quick setup
├── Materials/                    # (Generated at runtime)
├── Prefabs/
└── XR/
```

## Setup Instructions

### Prerequisites

- **Unity 2022.3 LTS** or later (2023.x also supported)
- **XR Interaction Toolkit** 2.5+ (installed via Package Manager)
- **OpenXR Plugin** (for PC VR — Meta Quest Link or SteamVR)
- **XR Plugin Management** (installed via Package Manager)
- 6DoF VR headset with two motion controllers

### Step-by-Step Setup

1. **Open Project in Unity**
   - Open Unity Hub → Add → Select this project folder
   - Unity will import packages from `Packages/manifest.json`

2. **Install XR Packages** (if not auto-resolved)
   - Window → Package Manager
   - Install: XR Interaction Toolkit, XR Plugin Management, OpenXR
   - When prompted, import XR Interaction Toolkit **Starter Assets**

3. **Configure XR**
   - Edit → Project Settings → XR Plug-in Management
   - Enable **OpenXR** under PC (or your target platform)
   - Under OpenXR, add your headset's interaction profile (e.g., Oculus Touch)

4. **Open the Scene**
   - Open `Assets/Scenes/ControlRoom.unity`
   - Or use menu: **Window → VR Control Room → Setup Scene** (in any scene)

5. **Play**
   - Connect your VR headset
   - Press Play in Unity Editor
   - The room is procedurally generated at runtime by `SceneBootstrap` → `RoomBuilder`

### Controls

| Action              | Controller Input                 |
| ------------------- | -------------------------------- |
| Toggle Light Switch | Point ray at switch + Trigger    |
| Press Button        | Point ray at button + Trigger    |
| Grab Crate          | Reach crate + Grip button (hold) |
| Release Crate       | Release Grip button              |
| Move                | Left thumbstick                  |
| Turn                | Right thumbstick                 |

## Technical Details

- **Architecture**: The scene is procedurally generated at runtime. `RoomBuilder.cs` creates all geometry, materials, physics, interactions, and the XR rig programmatically.
- **XR Toolkit**: Uses `XRSimpleInteractable` for toggle/button, `XRGrabInteractable` for the crate, `XRRayInteractor` for pointing, and `XRDirectInteractor` for grabbing.
- **State Management**: `RoomManager` singleton tracks lights and door state, enforcing the conditional logic.
- **No Persistent State**: Each session resets to initial state on launch (lights on, door closed, crate at original position).

## Submission Structure

```
assignment-2b-Vineesh7/
├── code/
│   └── Assets/              # Unity Assets folder (scripts, scenes, materials)
├── docs/
│   └── Report.md            # Development report with metrics, challenges, reflection
├── Assets/                  # Original Unity project Assets (for opening in Unity)
├── Packages/
├── ProjectSettings/
└── README.md
```

- **`code/`** — Contains the `Assets` folder from the Unity project
- **`docs/`** — Contains the development report (metrics, requirements spec, challenges, reflection)
