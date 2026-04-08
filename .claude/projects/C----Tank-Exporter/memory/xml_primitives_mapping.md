---
name: XML to Primitives Mapping
description: How XML visual files define geometry sections that must be matched to primitives_processed sections
type: reference
---

# XML to Primitives_Processed File Mapping

## Overview

The `.model.xml` visual file **declares** which geometry sections are needed, while the `.primitives_processed` binary file **contains** the actual data. The loader must establish a mapping between them.

## Hull XML Example (Simple Case)

```xml
<renderSet>
  <treatAsWorldSpaceObject>false</treatAsWorldSpaceObject>
  <node>Scene Root</node>
  <geometry>
    <vertices>vertices</vertices>
    <primitive>indices</primitive>
    <primitiveGroup>
      <material>tank_hull_01</material>
      ...
    </primitiveGroup>
  </geometry>
</renderSet>
```

**Declares:**
- Section name: `vertices` (vertex geometry data)
- Section name: `indices` (index/triangle data)

## Chassis XML Example (Complex Case)

The chassis XML references multiple geometry sections:
- `track_RShape.vertices`, `track_RShape.uv2`, `track_RShape.indices`
- `track_LShape.vertices`, `track_LShape.uv2`, `track_LShape.indices`
- `chassis_LShape.vertices`, `chassis_LShape.indices`
- `chassis_RShape.vertices`, `chassis_RShape.indices`

## Attachment Points (Hit Points)

The XML also defines named attachment points that aren't geometry but are transform locations:

**Hull Example:**
- `HP_Fire_1`, `HP_Fire_2` - Fire/damage points
- `HP_TrackUp_LFront`, `HP_TrackUp_RFront` - Upper track contact points
- `HP_TrackUp_LRear`, `HP_TrackUp_RRear` - Rear upper track points
- `HP_Track_Exhaus_1`, `HP_Track_Exhaus_2` - Exhaust attachment points
- `HP_turretJoint` - Turret mounting point

These are stored as `<node>` elements with `<identifier>` and `<transform>` (4x4 matrix).

## Loading Process

1. **Parse XML visual file** to get:
   - Expected section names for each render set
   - Expected order of sections
   - Create `ordered_names[]` array with expected geometry section names

2. **Read primitives_processed binary**:
   - Parse section table at end
   - Get actual section names from the binary file

3. **Match sections**:
   ```
   For each expected name in ordered_names:
     Find matching section in primitives_processed
     Load that section's data
     Store in ordered_names[index].vert_data or indi_data
   ```

4. **Result**:
   - `ordered_names[]` now contains both metadata (from XML) and actual binary data (from primitives_processed)
   - Sections are in the order expected by the render pipeline

## Critical Mapping Rules

- **Section names must match exactly** between XML declaration and primitives_processed file
- **Order matters** - XML defines the order geometry will be processed
- **Multiple geometries per component** - Hull might reference one geometry, chassis references multiple (left/right tracks, left/right chassis)
- **Optional sections** - UV2 and color sections may be absent
- **Named sections** - Some are generic (`vertices`, `indices`), others are asset-specific (`track_RShape.vertices`)

## XML Part Types (Identified by Name)

Code categorizes parts based on filename patterns:

```
If InStr(filename, "Chassis") > 0 Then xmlget_mode = 1
If InStr(filename, "Hull") > 0 Then xmlget_mode = 2
If InStr(filename, "Turret") > 0 Then xmlget_mode = 3
If InStr(filename, "Gun_") > 0 Then xmlget_mode = 4
If InStr(filename, "segment") > 0 Then xmlget_mode = 5
```

This mode determines how the section names are stored and correlated (prevents mixing section names between different part types).

