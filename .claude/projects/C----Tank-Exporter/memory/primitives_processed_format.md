---
name: Primitives_Processed File Format
description: Binary format structure for WoT tank geometry data (vertices, indices, UVs, colors)
type: reference
---

# .primitives_processed Binary File Format

## Overall Structure

```
[Header: 4 bytes]
[Section 1 Data]
[Section 2 Data]
...
[Section N Data]
[Section Table]
[Section Table Offset: 4 bytes (Int32)]
```

## Reading Process

1. Read last 4 bytes to get `sec_table_start` offset
2. Seek to `(file_length - 4 - sec_table_start)` to start reading section table
3. Parse section metadata
4. Load section data at their stored locations

---

## Section Table Format (At End of File)

Repeats for each section (max 100 sections):

```
UInt32 section_size
UInt32 [unused]
UInt32 [unused]
UInt32 [unused]
UInt32 [unused]
UInt32 section_name_length
Char[section_name_length] section_name
Char[padding] (align to 4-byte boundary)
```

**Section Types Identified by Name:**
- `vertices` - Vertex position, normal, UV, bones, weights
- `indices` - Triangle indices and primitive group definitions
- `uv2` - Secondary UV coordinates (lightmap UVs)
- `colour` - Per-vertex color data

---

## Vertex Data Section Format

**Header (64 bytes - null terminated string):**
- Format descriptor string that determines vertex layout

**Supported Vertex Format Types:**

| Format String | Stride | Has Real Normals | Skinned |
|---|---|---|---|
| `xyznuv` | 32 | Yes | No |
| `BPVTxyznuv` | 24 | No (packed) | Yes |
| `xyznuviiiwwtb` | 37 | No (packed) | Yes |
| `BPVTxyznuviiiww` | 32 | No (packed) | Yes |
| `BPVTxyznuviiiwwtb` | 40 | No (packed) | Yes |
| `xyznuvtb` | 32 | No (packed) | No |
| `BPVTxyznuvtb` | 32 | No (packed) | No |

**Data Layout:**
```
UInt32 vertex_count
[For each vertex:]
  Float32 x, y, z                    (position, 12 bytes)

  IF realNormals (format "xyznuv"):
    Float32 nx, ny, nz               (normal, 12 bytes)
  ELSE:
    UInt32 packed_normal             (packed normal, 4 bytes)
    [If BPVT mode: unpack as 8-8-8 signed integers]
    [Else: unpack as 10-10-10 bits + sign]

  Float32 u, v                       (primary UV, 8 bytes)

  IF format contains "iiiww" (bone data):
    Byte index_1, index_2,
         index_3, index_4             (bone indices, 4 bytes)
    Byte weight_1, weight_2,
         weight_3, weight_4           (bone weights, 4 bytes)

  IF format contains "tb" (tangent/bitangent):
    UInt32 tangent                   (packed, 4 bytes)
    UInt32 bitangent                 (packed, 4 bytes)
```

**Color/Tangent Data (optional):**
- Offset 132 bytes into section
- UInt32 count
- Remaining bytes: actual data

---

## Index/Primitive Data Section Format

**Header (64 bytes - null terminated):**
- Contains "list32" for 32-bit indices
- Otherwise assume 16-bit indices

**Data Layout:**
```
UInt32 nIndices_              (total triangle indices)
UInt32 nInd_groups            (number of primitive groups)
[padding to offset 72]
[Index data - UInt16[] or UInt32[]]

[For each primitive group:]
  UInt32 startIndex_          (offset into index buffer)
  UInt32 nPrimitives_         (number of triangles)
  UInt32 startVertex_         (vertex buffer offset)
  UInt32 nVertices_           (vertex count for this group)
```

**Index Scale:**
- 16-bit indices: `ind_scale = 2` (offset into indices is `count * 2`)
- 32-bit indices: `ind_scale = 4` (offset into indices is `count * 4`)

---

## Color Data Section Format (Optional)

```
[132 bytes - header/padding]
UInt32 color_count
Byte[color_count * 4] color_data    (RGBA per vertex, 1 byte each)
```

---

## UV2 Data Section Format (Optional)

```
[132 bytes - header/padding]
UInt32 uv2_count
Float32[uv2_count * 2] uv2_data     (U, V per vertex)
```

---

## Key Implementation Details

**Alignment:**
- All sections padded to 4-byte boundaries
- `location += (location % 4)` to align

**Vertex Processing:**
- Read vertices into `tbuf()` array
- Group vertices by primitive group using `startVertex_` and `nVertices_`
- Indices are global; subtract `startVertex_` for group-relative indices

**Normal Unpacking:**
- BPVT mode: `unpackNormal_8_8_8()` - 8-bit signed per channel
- Standard mode: `unpackNormal()` - 10-10-10 bit packing

**Bone Skinning:**
- Indices 0-3 point to bone/node indices
- Weights are normalized (0-255 → 0.0-1.0)
- Used for skeletal animation of tracks/wheels

**Multiple Render Sets:**
- File contains multiple geometry groups (chassis, tracks, hull, turret, gun)
- Each group maintains separate vertices, indices, and optional UV2/color data
- Loaded in order: vertices and indices are required, UV2/colors optional

