# Exhibit Preview Images

Reference images for each museum room exhibit. These can be used as:
- **UI thumbnails** in the `ExhibitInfoPanel` and `MainMenuController`
- **Loading screen previews** when transitioning between rooms
- **Wrist HUD map icons** in the `MuseumHUD`
- **Texture references** for pedestal display frames

## Images

| File | Room | Description |
|------|------|-------------|
| `Room1_NahumShrine_Iraq.png` | Room 1 – Iraq Section | Nahum Shrine 3D photogrammetry reconstruction with Assyrian carved reliefs |
| `Room2_PragueMonuments.png` | Room 2 – Prague Monuments | St. Vitus Cathedral Gothic facade photogrammetry scan |
| `Room3_ArchaeologicalShards.png` | Room 3 – Archaeological Shards | Terracotta pottery fragments with geometric patterns |
| `Room4_AerialPhotogrammetry.png` | Room 4 – Aerial Photogrammetry | Archaeological site point cloud mesh visualization |
| `Room5_IndianHeritage.png` | Room 5 – Indian Heritage Wing | Meenakshi-style Dravidian temple in sandstone and marble |
| `Corridor_MainHall.png` | Central Corridor | Grand museum corridor with marble floors and vaulted ceiling |

## Unity Import Settings (Recommended)

```
Texture Type:        Sprite (2D and UI) or Default
Max Size:            1024 (for UI) / 2048 (for environment)
Compression:         ASTC 6x6 (Quest) / DXT5 (Desktop)
Generate Mip Maps:   Yes (for environment) / No (for UI)
sRGB:                Yes
```

## Usage in Code

These textures are referenced by the `ExhibitData` ScriptableObject's `previewImage` field.
Assign them via the Inspector or use `Resources.Load<Sprite>()` if placed in a Resources folder.
