# WoT Model File Format Reference

Reverse-engineered from `ModTankLoader.vb` — `build_primitive_data`, `openVisual`, `Get_ordered_names`.

---

## Overview — How the Two Files Work Together

A WoT tank component (hull, turret, gun, chassis) is defined by a pair of files:

```
Hull.model
  └─ references ──► Hull.visual_processed    (scene graph, materials, texture paths,
                                               geometry section name mapping)
                         │
                         └─ references ──► Hull.primitives_processed   (raw vertex +
                                                                          index data)
```

The `.visual_processed` tells you **what** geometry sections to load and **which textures**
to apply to each primitive group.
The `.primitives_processed` contains the actual **binary mesh data** for those sections.

The link between them is the **geometry section name**:

```
visual_processed  geometry.vertices  =  "animatedShape.vertices"
                                               │
primitives_processed  section name   =  "animatedShape.vertices"  ◄── same string
```

---

## .model File

A small XML/packed-section file. Contains one of two keys:

| Key | Description |
|---|---|
| `nodefullVisual` | Path to the skinned (animated) visual — append `.visual_processed` |
| `nodelessVisual` | Path to the static (LOD) visual — append `.visual_processed` |

The loader checks `lod0` first, falls back to `lod1`.

---

## .visual_processed File Format

BigWorld packed-section binary. The loader parses it into an XML dataset via `openXml_stream`.
The key tables extracted are:

### `geometry` table
Each row is one render set (one mesh group):

| Column | Description |
|---|---|
| `vertices` | Name of the vertex section in `.primitives_processed` e.g. `animatedShape.vertices` |
| `primitive` | Name of the index section in `.primitives_processed` e.g. `animatedShape.indices` |
| `stream` | Optional — name of `uv2` or `colour` extra stream section |
| `geometry_Id` | Integer ordering index |

### `material` table
Each row is one material, linked to a primitive group by order:

| Column | Description |
|---|---|
| `identifier` | Material name e.g. `tank_hull_01_skinned` |
| `fx` | Shader path e.g. `shaders/std_effects/PBS_tank_skinned.fx` |
| `diffuseMap` | Path to albedo/diffuse texture (AM.dds) |
| `normalMap` | Path to normal map texture (ANM.dds) |
| `specularMap` | Path to GMM texture (GMM.dds) — metallic/roughness/mask |
| `excludeMaskAndAOMap` | Path to AO texture (AO.dds) |
| `colorIdMap` | Path to camouflage colour ID map |

### `stream` table (optional)
Extra per-geometry streams. Each row has:

| Column | Description |
|---|---|
| `geometry_Id` | Links back to geometry row |
| `stream_Text` | Section name — contains `"uv2"` or `"colour"` to identify type |

---

## Render Set Example — GB152 AT FV230 Breaker Hull

```
Render Set 1: animatedShape  (skinned geometry — bone-weighted)
  vertices section  →  animatedShape.vertices
  indices  section  →  animatedShape.indices
  shader            →  PBS_tank_skinned.fx

  primitiveGroup[0]  →  material: tank_hull_01_skinned
      diffuseMap      →  AT_FV230_Breaker_hull_01_AM.dds
      normalMap       →  AT_FV230_Breaker_hull_01_ANM.dds
      GMM             →  AT_FV230_Breaker_hull_01_GMM.dds
      AO              →  AT_FV230_Breaker_hull_01_AO.dds
      colorIdMap      →  vehicles/russian/common/common_ID.dds

  primitiveGroup[1]  →  material: tank_equipment_01_skinned
      diffuseMap      →  AT_FV230_Breaker_equipment_AM.dds
      normalMap       →  AT_FV230_Breaker_equipment_ANM.dds
      GMM             →  AT_FV230_Breaker_equipment_GMM.dds
      AO              →  AT_FV230_Breaker_equipment_AO.dds

Render Set 2: hull_Shape  (static geometry — no bone weights)
  vertices section  →  hull_Shape.vertices
  indices  section  →  hull_Shape.indices
  shader            →  PBS_tank.fx

  primitiveGroup[0]  →  material: tank_hull_01         (same textures as above)
  primitiveGroup[1]  →  material: tank_equipment_01    (same textures as above)
```

---

## .primitives_processed File Format

---

### Overall Layout

```
[ 4-byte tag       ]  <- file start, read and discarded
[ Section data 1   ]  <- raw binary blobs, 4-byte aligned
[ Section data 2   ]
  ...
[ Section Table    ]  <- describes all sections
[ UInt32           ]  <- LAST 4 bytes: offset from EOF to section table
```

---

### File Footer — Section Table Pointer

Last 4 bytes = `Int32` distance back from `(FileLen - 4)` to the Section Table start:

```
seek position = FileLen - 4 - value_of_last_4_bytes
```

---

### Section Table

Up to 100 entries, read until `position >= FileLen - 4`:

```
 4 bytes  UInt32        Section data size (bytes)
16 bytes  UInt32 x4     Unused padding
 4 bytes  UInt32        Section name string length
 N bytes  Chars         Section name (ASCII, no null terminator)
 0-3 bytes              Padding to 4-byte boundary
```

Section data positions calculated from start of file:

```
base_location = 4   (skip the file tag)
for each section:
    section_location  = base_location
    base_location    += section_size
    base_location    += base_location MOD 4   (align to 4 bytes)
```

---

### Known Section Name Types

| Name pattern | Content | Linked from visual_processed |
|---|---|---|
| `*.vertices` | Vertex buffer | `geometry.vertices` column |
| `*.indices` | Index buffer + primitive groups | `geometry.primitive` column |
| `*.uv2*` | Secondary UV coordinates | `stream` table, name contains `"uv2"` |
| `*.colour*` | Vertex colour data | `stream` table, name contains `"colour"` |

The prefix before the dot (e.g. `animatedShape`, `hull_Shape`) matches the render set name
in the `.visual_processed`.

---

### Vertex Section

#### Format Header (64 bytes)
ASCII string identifying vertex layout. Null-terminated, padded to 64 bytes.

```
Format string            Stride  Normals          Notes
-----------------------------------------------------------------
xyznuv                     32    Float32 x3       Basic, compute tangents
xyznuvtb                   32    Packed UInt32    Tangent + bitangent stored
BPVTxyznuv                 24    Packed 8_8_8     BPVT compressed
BPVTxyznuvtb               32    Packed 8_8_8     BPVT + tangent/bitangent
xyznuviiiwwtb              37    Packed UInt32    Skinned + tangent/bitangent
BPVTxyznuviiiww            32    Packed 8_8_8     BPVT skinned, no tangents
BPVTxyznuviiiwwtb          40    Packed 8_8_8     BPVT skinned + tangents
```

> **BPVT formats**: skip to byte **132** before reading vertex count (extra header block).

#### Vertex Count
```
4 bytes  UInt32   Number of vertices
```

#### Per-Vertex Layout

```
--- Position (all formats) ---
4  Float32   X
4  Float32   Y
4  Float32   Z

--- Normals: xyznuv only (real floats) ---
4  Float32   NX
4  Float32   NY
4  Float32   NZ

--- Normals: all other formats (packed) ---
4  UInt32    Packed normal
               BPVT formats = 8 bits per channel (8_8_8)
               Others       = 10_10_10_2

--- UVs (all formats) ---
4  Float32   U
4  Float32   V

--- Skinning: formats with "iiiww" in name ---
1  Byte      Bone index 1
1  Byte      Bone index 2
1  Byte      Bone index 3
1  Byte      Bone index 4
1  Byte      Bone weight 1
1  Byte      Bone weight 2
1  Byte      Bone weight 3
1  Byte      Bone weight 4

--- Tangents: formats with "tb" in name ---
4  UInt32    Packed tangent
4  UInt32    Packed bitangent
```

---

### Index Section

#### Index Header (64 bytes)
ASCII string, null-terminated, padded to 64 bytes.
- Contains `"list32"` → indices are **UInt32** (4 bytes each)
- Otherwise → indices are **UInt16** (2 bytes each)

#### After Header:
```
4 bytes   UInt32              nIndices     total number of indices
4 bytes   UInt32              nInd_groups  number of sub-meshes
N bytes   UInt16 or UInt32    raw index data  (nIndices x ind_scale bytes)

--- seek to: (nIndices x ind_scale) + 72 ---

Primitive Groups (one per sub-mesh, matches material order in visual_processed):
4 bytes   UInt32   startIndex_    first index in the index buffer
4 bytes   UInt32   nPrimitives_   number of triangles in this group
4 bytes   UInt32   startVertex_   first vertex in the vertex buffer
4 bytes   UInt32   nVertices_     number of vertices in this group
```

---

### UV2 Section
```
132 bytes  —        Header block (skipped)
  4 bytes  UInt32   UV2 entry count
  N bytes  Bytes    UV2 data  (section_size - 136 bytes)
```

---

### Colour Section
```
132 bytes  —        Header block (skipped)
  4 bytes  UInt32   Colour entry count
  N bytes  Bytes    RGBA colour data, 4 bytes per vertex  (section_size - 136 bytes)
```

---

## Full Load Chain Summary

```
1. Load .model
      └─ read nodefullVisual or nodelessVisual → path to .visual_processed

2. Load .visual_processed
      └─ parse packed-section binary into XML dataset
      └─ read geometry table:
            vertices  = section name to look up in .primitives_processed
            primitive = section name for index data
            stream    = optional uv2 / colour section names
      └─ read material table (one material per primitiveGroup, in order):
            diffuseMap, normalMap, specularMap, AO map, colorIdMap

3. Load .primitives_processed
      └─ read section table from end of file
      └─ match section names to geometry table entries from step 2
      └─ for each matched vertices section:
            read vertex format header (stride, normal type, skinned flag)
            read vertex count
            read vertex data
      └─ for each matched indices section:
            read index type (16 or 32 bit)
            read index count + group count
            read raw indices
            read primitive groups (startIndex, nPrimitives, startVertex, nVertices)
      └─ assign material[n] textures to primitiveGroup[n]
```

---

*Reverse-engineered by Coffee — Tank Exporter project.*
