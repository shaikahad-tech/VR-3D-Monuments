# Materials Guide

## Photogrammetry Material Setup

1. Create a new Material: `Assets > Create > Material`
2. Set Shader to: **VirtualMuseum/PhotogrammetryLit**
3. Assign your photogrammetry texture to **Albedo (Photogrammetry Texture)**
4. Optional: Assign a normal map for surface detail
5. Optional: Enable **Detail Texture** for close-up micro-detail

## Material Settings for Quest

- **No specular/metallic**: The PhotogrammetryLit shader is diffuse-only for performance
- **Single material per model**: Avoid multi-material meshes
- **Shared materials**: Reuse materials across similar objects for batching

## Split-Texture Models

For large models split into parts (per the paper's methodology):
1. Each part gets its own material with its own texture
2. All parts share the same shader
3. Use `TextureQualityManager` to manage memory across parts

## Pedestal Materials

- Use URP/Lit with low metallic for pedestal geometry
- Enable **Emission** for the glow ring (set `_EmissionColor` via script)
