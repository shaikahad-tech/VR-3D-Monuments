# Setup Guide — Virtual Museum VR

## Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| Unity Hub | Latest | Project management |
| Unity Editor | 2022.3 LTS | Game engine |
| Android SDK | API 29+ | Quest build target |
| Meta Quest Developer Hub | Latest | Device deployment |

---

## Step 1: Create Unity Project

1. Open Unity Hub → **New Project**
2. Select **3D (URP)** template
3. Name: `VirtualMuseumVR`
4. Click **Create Project**

## Step 2: Import Project Files

1. Copy the entire `Assets/_Project/` folder into your project's `Assets/` directory
2. Replace `Packages/manifest.json` with the provided version
3. Unity will auto-install required packages (this may take a few minutes)

## Step 3: Import XRI Starter Assets

1. Go to **Window > Package Manager**
2. Find **XR Interaction Toolkit**
3. Under **Samples**, click **Import** next to **Starter Assets**
4. Under **Samples**, click **Import** next to **XR Device Simulator**

## Step 4: Configure Build Target

1. Go to **File > Build Settings**
2. Select **Android**
3. Click **Switch Platform**
4. Under **Player Settings > XR Plug-in Management**:
   - Check **OpenXR**
   - Under OpenXR, add **Meta Quest Touch Pro Controller Profile**
   - Set **Render Mode** to **Single Pass Instanced**

## Step 5: Run Setup Wizard

1. Go to **Tools > Virtual Museum > Setup Wizard**
2. Follow the 5-step wizard:
   - Step 1: Project Settings (Vulkan, quality presets)
   - Step 2: Scene Hierarchy (manager objects)
   - Step 3: XR Rig (player controller)
   - Step 4: Museum Layout (rooms, corridor, pedestals)
   - Step 5: Completion

## Step 6: Generate or Import Monuments

**Option A: Automated Procedural Monuments (New)**
Use the built-in catalog builder to instantly generate complex testing models:
1. Go to **Tools > Virtual Museum > Build Monument Catalog**
2. Click **Generate Complete Catalog**
3. Open `Assets/_Project/Exhibits/` to see the generated folders, prefabs, and data.

**Option B: Manual Import**
1. Export models as `.FBX` from Agisoft Metashape
2. Drag files into `Assets/_Project/Models/`
3. Decimate to ~10,000 polygons using Unity tools

## Step 7: Create Exhibit Data

For each model:
1. Right-click in Project → **Create > Virtual Museum > Exhibit Data**
2. Fill in metadata: title, era, location, polygon counts, etc.
3. Assign the narration audio clip (if available)
4. Set the room assignment

## Step 8: Place Exhibits on Pedestals

1. Drag your model prefab onto a pedestal's **ExhibitSpawnPoint**
2. Add the `MonumentGrabInteractable` component to the model
3. Assign the `ExhibitData` ScriptableObject
4. Add an `ExhibitTriggerZone` as a child of the pedestal
5. Add an `ExhibitInfoPanel` near the pedestal

## Step 9: Configure XR Rig & NavMesh (CRITICAL)

To enable the new smooth locomotion and interaction features:
1. **NavMesh:** Go to **Window > AI > Navigation**, mark museum floors as `Navigation Static`, and click **Bake**.
2. **Player Rig:** Ensure your XR Origin has `ContinuousMovementProvider` and `SnapTurnProvider` attached.
3. **Input Actions:** Assign `moveAction` (Left Joystick) and `turnAction` (Right Joystick) in the Inspector.

## Step 10: Bake Lighting

1. Mark all room geometry as **Static** in the Inspector
2. Set all room lights to **Baked** mode
3. Go to **Window > Rendering > Lighting**
4. Click **Generate Lighting**

## Step 10: Build & Deploy

1. Connect Quest via USB (enable developer mode first)
2. Go to **File > Build Settings > Build And Run**
3. Target: **Meta Quest 2/3/Pro**

---

## Testing Without a Headset

Use the **XR Device Simulator** to test in the Unity Editor:
1. Enable it in **Project Settings > XR Plug-in Management > XR Interaction Toolkit**
2. Check **Use XR Device Simulator in Scenes**
3. Press Play — use mouse + keyboard to simulate VR controls
