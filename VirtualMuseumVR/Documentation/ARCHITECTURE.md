# Architecture — System Design

## System Overview

```mermaid
graph TD
    VMM["VirtualMuseumManager<br/>(Singleton)"]
    GE["GameEvents<br/>(Static Event Bus)"]

    VMM --> GE

    subgraph Core
        RM["RoomManager"]
        ED["ExhibitData SOs"]
        EDB["ExhibitDatabase"]
    end

    subgraph Interaction
        MGI["MonumentGrabInteractable"]
        MI["MonumentInspector"]
        ETZ["ExhibitTriggerZone"]
        PP["PassthroughPortal"]
    end

    subgraph Locomotion
        MTM["MuseumTeleportManager"]
        CS["ComfortSettings"]
        GTC["GuidedTourController"]
    end

    subgraph UI
        EIP["ExhibitInfoPanel"]
        HUD["MuseumHUD"]
        WI["WaypointIndicator"]
        MMC["MainMenuController"]
    end

    subgraph Optimization
        LOD["LODController"]
        TQM["TextureQualityManager"]
        OCM["OcclusionCullingManager"]
        PM["PerformanceMonitor"]
    end

    subgraph Audio
        AAM["AmbientAudioManager"]
        SAT["SpatialAudioTrigger"]
    end

    GE --> Core
    GE --> Interaction
    GE --> Locomotion
    GE --> UI
    GE --> Optimization
    GE --> Audio
```

---

## Event Flow

All inter-system communication uses `GameEvents.cs` (static event bus):

```mermaid
sequenceDiagram
    participant Player
    participant ETZ as ExhibitTriggerZone
    participant GE as GameEvents
    participant EIP as ExhibitInfoPanel
    participant SAT as SpatialAudio
    participant LC as LightingController

    Player->>ETZ: Enters trigger zone
    ETZ->>GE: RaiseExhibitProximityEntered(id)
    GE->>EIP: Show info panel
    GE->>SAT: Play narration
    GE->>LC: Activate spotlight

    Player->>ETZ: Exits trigger zone
    ETZ->>GE: RaiseExhibitProximityExited(id)
    GE->>EIP: Hide info panel
    GE->>SAT: Stop narration
    GE->>LC: Deactivate spotlight
```

---

## Grab & Inspect Flow

```mermaid
sequenceDiagram
    participant Player
    participant MGI as MonumentGrab
    participant GE as GameEvents
    participant MI as MonumentInspector
    participant VMM as MuseumManager

    Player->>MGI: Grab monument
    MGI->>MGI: Scale to inspection size
    MGI->>MGI: Send haptic pulse
    MGI->>GE: RaiseExhibitGrabbed(id)
    GE->>VMM: CurrentExhibitId = id

    Player->>MI: Press inspect button
    MI->>MI: Enter orbit mode
    MI->>MI: Enable spotlight
    MI->>GE: RaiseInspectionStarted(id)
    GE->>VMM: State = Inspecting

    Player->>MI: Press inspect again
    MI->>GE: RaiseInspectionEnded(id)
    GE->>VMM: State = previous

    Player->>MGI: Release monument
    MGI->>MGI: Lerp back to pedestal
    MGI->>GE: RaiseExhibitReleased(id)
```

---

## Room Transition Flow

```mermaid
sequenceDiagram
    participant Player
    participant MTM as TeleportManager
    participant VMM as MuseumManager
    participant RM as RoomManager
    participant OCM as OcclusionManager
    participant LC as LightingController
    participant AAM as AmbientAudio

    Player->>MTM: Teleport to doorway
    MTM->>VMM: TransitionToRoom(targetId)
    VMM->>RM: Fade to black
    RM->>RM: Deactivate old room
    RM->>RM: Activate new room
    RM->>OCM: Update visible renderers
    RM->>LC: Transition lighting preset
    RM->>AAM: Crossfade ambient audio
    RM->>RM: Fade from black
    VMM->>VMM: State = FreeRoam
```

---

## Data Model

```mermaid
classDiagram
    class ExhibitData {
        +string exhibitId
        +string title
        +string description
        +string era
        +MuseumRoom room
        +AcquisitionMethod method
        +int originalPolygonCount
        +int optimizedPolygonCount
        +AudioClip narrationClip
        +float inspectionScale
    }

    class RoomDefinition {
        +string roomId
        +string displayName
        +GameObject roomGameObject
        +Transform spawnPoint
        +Color ambientLightColor
    }

    class TeleportWaypoint {
        +string waypointId
        +Transform anchor
        +string targetRoomId
        +bool isRoomDoorway
        +int tourOrder
    }

    ExhibitData "many" --> "1" MuseumRoom
    RoomDefinition "1" --> "many" ExhibitData
    TeleportWaypoint "many" --> "1" RoomDefinition
```

---

## Namespace Structure

```
VirtualMuseumVR/
├── Core/           — VirtualMuseumManager, RoomManager, ExhibitData, GameEvents
├── Interaction/    — MonumentGrabInteractable, MonumentInspector, ExhibitTriggerZone, PassthroughPortal
├── Locomotion/     — MuseumTeleportManager, ComfortSettings, GuidedTourController
├── UI/             — ExhibitInfoPanel, MuseumHUD, WaypointIndicator, MainMenuController
├── Environment/    — MuseumArchitectureBuilder, LightingController, PedestalController, AmbientParticleSystem
├── Optimization/   — LODController, TextureQualityManager, OcclusionCullingManager, PerformanceMonitor
├── Audio/          — AmbientAudioManager, SpatialAudioTrigger
├── Data/           — ExhibitDatabase
└── Editor/         — MuseumSetupWizard, ModelImportProcessor, ExhibitDataEditor
```
