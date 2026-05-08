# Optimization Guide — Meta Quest Performance

## Target Performance

| Metric | Quest 2 | Quest 3 |
|---|---|---|
| Frame rate | 72 Hz | 90/120 Hz |
| Resolution | 1832×1920 per eye | 2064×2208 per eye |
| Available RAM | ~3.5 GB | ~5.5 GB |
| GPU | Adreno 650 | Adreno 740 |

---

## Polygon Budget

| Category | Budget | Script |
|---|---|---|
| Per exhibit (close) | 10,000 polys | LODController.cs |
| Per exhibit (medium) | 5,000 polys | LODController.cs |
| Per exhibit (far) | 1,000 polys | LODController.cs |
| Room environment | 20,000 polys | Static geometry |
| Total visible scene | < 200,000 polys | OcclusionCullingManager.cs |

---

## Texture Optimization

### Compression (ASTC)
All textures auto-compressed to ASTC via `ModelImportProcessor.cs`:
- **ASTC 6×6**: Best quality-to-size ratio for Quest
- **Mipmap streaming**: Enabled globally via `TextureQualityManager.cs`
- **Memory budget**: 512 MB default, auto-downgrade if exceeded

### Resolution Guidelines
| Object Size | Max Texture | Quest Override |
|---|---|---|
| Small (< 0.5m) | 1024px | 1024px |
| Medium (0.5-2m) | 2048px | 2048px |
| Large (> 2m) | 4096px | 2048px |
| Split parts | 4096px each | 2048px each |

---

## Draw Call Reduction

1. **Shared materials**: Use `PhotogrammetryLit.shader` across all exhibits
2. **Static batching**: All room geometry marked as Static
3. **GPU instancing**: Enabled in shader for stereo rendering
4. **Texture atlasing**: Combine small object textures

---

## Lighting

Following the paper's recommendation — **Baked Lighting Only**:

1. Mark all room geometry as **Static**
2. Set room lights to **Baked** mode
3. Use **Progressive GPU Lightmapper**
4. Only dynamic lights: exhibit accent spotlights (1-2 per room)
5. `LightingController.cs` manages per-room ambient settings

---

## Occlusion Culling

`OcclusionCullingManager.cs` implements room-based manual occlusion:
- Only the current room + corridor are rendered
- All other rooms have renderers disabled
- Triggered automatically on room transitions
- Reduces draw calls by ~75%

---

## Performance Monitor

`PerformanceMonitor.cs` provides:
- Real-time FPS counter (debug builds only)
- Auto-quality reduction if FPS < 60
- Memory usage warnings
- Session performance reports

### Quality Levels
| Level | Texture Size | Mipmap Bias | Particles |
|---|---|---|---|
| High (0) | 4096px | 0 | Full |
| Medium (1) | 2048px | 1 | Reduced |
| Low (2) | 1024px | 2 | Disabled |

---

## Build Settings

### Critical Android Settings
```
Graphics API: Vulkan (not OpenGL ES)
Minimum API Level: Android 10 (API 29)
Target Architecture: ARM64
Scripting Backend: IL2CPP
Rendering: Single Pass Instanced
VSync: Disabled (XR manages timing)
```

### Profiling on Device
1. Install **OVR Metrics Tool** on Quest
2. Enable overlay: Settings > Developer > Performance HUD
3. Monitor: FPS, GPU utilization, draw calls, memory
