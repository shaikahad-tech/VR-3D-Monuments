# Photogrammetry Pipeline Guide

**Based on the IBMR (Image-Based Modeling and Rendering) workflow from Pavelka & Raeva (2019)**

---

## Overview

The paper describes a 5-step pipeline for transforming real-world monuments into VR-ready 3D models:

```
Photos → Point Cloud → Dense Mesh → Optimized Mesh → VR Integration
```

---

## Step 1: Data Acquisition

### Ground-Based Photography
- **Camera**: DSLR with 50mm+ lens (avoid wide-angle distortion)
- **Photos**: 100-500+ per object, 60-80% overlap between shots
- **Technique**: Walk around the object in concentric circles at multiple heights
- **Lighting**: Overcast sky preferred (avoids harsh shadows baked into texture)

### Aerial Photography
- **Drone**: DJI Mavic series or senseFly eBee (as used in the paper)
- **Flight**: Grid pattern with 75%+ overlap
- **Altitude**: 50-100m for large structures
- **GCP**: Place ground control points for georeferencing

### Combined Approach
- Used for Stafford Castle in the paper
- Aerial for overview + ground for detailed facades
- Align both datasets in reconstruction software

---

## Step 2: 3D Reconstruction

### Recommended Software
| Software | Best For | Paper Reference |
|---|---|---|
| Agisoft Metashape | General photogrammetry | Primary tool in paper |
| RealityCapture | Speed, large datasets | Alternative |
| Meshroom | Open-source option | Free alternative |

### Reconstruction Steps
1. **Import photos** → align cameras (sparse point cloud)
2. **Build dense point cloud** → millions of points
3. **Generate mesh** → polygon mesh from point cloud
4. **Build texture** → project photos onto mesh UV coordinates

### Quality Settings for VR
- Dense cloud: **High** quality (not Ultra — diminishing returns)
- Mesh: **High** quality, face count determined by source
- Texture: **4096x4096** or **8192x8192** per texture part

---

## Step 3: Optimization (CRITICAL for Quest)

### Polygon Decimation
The paper emphasizes decimating from ~100,000 to ~10,000 polygons per model:

| LOD Level | Polygon Count | Use Case |
|---|---|---|
| Source | 100,000+ | Archive only |
| LOD 0 (VR High) | 10,000 | Close inspection (< 2m) |
| LOD 1 (VR Medium) | 5,000 | Medium distance (2-5m) |
| LOD 2 (VR Low) | 1,000 | Far distance (> 5m) |

### Tools for Decimation
- **Agisoft Metashape**: Build Mesh → Face Count parameter
- **Blender**: Decimate modifier (Collapse or Planar)
- **MeshLab**: Quadric Edge Collapse Decimation
- **Instant Meshes**: Automatic retopology (open source)

### Texture Optimization
The paper notes that large objects should be **split into parts** to maintain resolution:

- **Small objects** (< 1m): Single 4K texture
- **Medium objects** (1-5m): Single 4K or split into 2× 4K
- **Large objects** (> 5m): Split into 4-8 parts, each with 4K texture
- **Compression**: Export as PNG, Unity converts to ASTC for Quest

### Export Format
- **Mesh**: `.FBX` (preferred) or `.OBJ`
- **Textures**: `.PNG` or `.TGA` (lossless)
- **Coordinate System**: Y-up, meters scale
- **UV**: Single UV set for diffuse, second UV for lightmap (auto-generated in Unity)

---

## Step 4: Model Preparation in Blender (Optional)

If further cleanup is needed before Unity import:

1. **Clean geometry**: Remove floating vertices, fill holes
2. **Decimate**: Apply Decimate modifier
3. **UV optimization**: Reproject UVs if needed
4. **Scale**: Ensure real-world scale (1 unit = 1 meter)
5. **Orientation**: Y-up, front-facing Z+
6. **Export**: FBX with embedded textures

---

## Step 5: Unity Integration

The `ModelImportProcessor.cs` script automatically handles:
- ✅ Enable Read/Write for runtime access
- ✅ Generate lightmap UVs
- ✅ Optimize mesh vertices
- ✅ Set ASTC texture compression for Android
- ✅ Enable mipmap streaming
- ✅ Mesh compression (Medium)

### Manual Steps
1. Create `ExhibitData` ScriptableObject
2. Set polygon counts (original and optimized)
3. Assign acquisition method
4. Place on pedestal with `MonumentGrabInteractable`

---

## Quest Performance Budget

| Resource | Budget | Notes |
|---|---|---|
| Total scene polygons | 100,000-200,000 | All visible models combined |
| Draw calls | < 100 | Use batching, shared materials |
| Texture memory | < 512 MB | Use mipmap streaming |
| Target frame rate | 72 Hz (Quest 2), 90 Hz (Quest 3) | Never drop below |
