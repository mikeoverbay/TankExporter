#pragma once
#include <stdint.h>

#ifdef FBX_READER_EXPORTS
#define FBX_API extern "C" __declspec(dllexport)
#else
#define FBX_API extern "C" __declspec(dllimport)
#endif

// -----------------------------------------------------------------------
//  Vertex layout — matches the VB.NET vertice_ class fields used by the
//  writer.  All Aspose.3D export transforms are already reversed here:
//    normal X  : un-negated  (exporter multiplied by -1)
//    UV0 v     : 1 - stored  (exporter wrote Y = 1 - v)
//    UV2 v2    : -stored     (exporter wrote Y = -v2)
//    bone data : byte 0-255  (exporter stored as 0-1 float in color channels)
// -----------------------------------------------------------------------
#pragma pack(push, 1)
struct FbxVertex
{
    float   x, y, z;
    float   nx, ny, nz;
    float   u,  v;
    float   u2, v2;
    uint8_t index_1,  index_2,  index_3,  index_4;
    uint8_t weight_1, weight_2, weight_3, weight_4;
};
#pragma pack(pop)

struct FbxFace { uint32_t v1, v2, v3; };

typedef void* FbxScene;

FBX_API FbxScene    fbx_load           (const char* path);
FBX_API void        fbx_free           (FbxScene scene);
FBX_API int         fbx_mesh_count     (FbxScene scene);
FBX_API const char* fbx_mesh_name      (FbxScene scene, int idx);
FBX_API const char* fbx_mesh_diffuse   (FbxScene scene, int idx);
FBX_API const char* fbx_mesh_normal_tex(FbxScene scene, int idx);
FBX_API int         fbx_mesh_vert_count(FbxScene scene, int idx);
FBX_API int         fbx_mesh_face_count(FbxScene scene, int idx);
FBX_API int         fbx_mesh_has_uv2   (FbxScene scene, int idx);
FBX_API int         fbx_mesh_color_ch  (FbxScene scene, int idx);
FBX_API void        fbx_mesh_get_verts (FbxScene scene, int idx, FbxVertex* out);
FBX_API void        fbx_mesh_get_faces (FbxScene scene, int idx, FbxFace*   out);
// Returns a pointer to 16 doubles — the column-major (OpenGL) 4x4 local transform
// matrix for mesh[idx], built from the Model node's Lcl Translation/Rotation/Scaling.
// Indices 12,13,14 hold the translation.  Valid until fbx_free().
FBX_API const double* fbx_mesh_matrix  (FbxScene scene, int idx);
