# Project Settings Guide

## Critical Settings for Meta Quest

### Player Settings (Edit > Project Settings > Player)

| Setting | Value | Reason |
|---|---|---|
| Company Name | Your studio name | Quest app identity |
| Product Name | Virtual Museum | Display name on Quest |
| Minimum API Level | Android 10 (API 29) | Quest 2 minimum |
| Target Architecture | ARM64 | Quest uses ARM64 |
| Scripting Backend | IL2CPP | Required for ARM64 |
| Graphics API | Vulkan | Best Quest performance |
| Active Input Handling | Input System (New) | Required for XRI 3.0 |

### Quality Settings

| Setting | Value |
|---|---|
| VSync Count | Don't Sync (0) |
| Target Frame Rate | 72 (Quest 2) or 90 (Quest 3) |
| Max Queued Frames | 0 |
| Texture Quality | Full Resolution |
| Anisotropic Textures | Per Texture |

### XR Plug-in Management

| Setting | Value |
|---|---|
| OpenXR | Enabled |
| Meta Quest Feature Group | Enabled |
| Render Mode | Single Pass Instanced |

### Physics

| Setting | Value | Reason |
|---|---|---|
| Default Solver Iterations | 4 | Reduce for mobile |
| Default Solver Velocity Iterations | 1 | Reduce for mobile |
| Auto Simulation | Enabled | Required for triggers |

### Lighting (Window > Rendering > Lighting)

| Setting | Value |
|---|---|
| Lightmapper | Progressive GPU |
| Lightmap Resolution | 20 texels/unit |
| Lightmap Size | 1024 |
| Compress Lightmaps | Enabled |
| Ambient Mode | Baked |
