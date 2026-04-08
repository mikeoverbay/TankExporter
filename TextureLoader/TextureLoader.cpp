#include <glew.h>
#include <GL/gl.h>
#include <cstdint>
#include <iostream>
#include <fstream>
#include <vector>
#include <thread>
#include <mutex>
#include <condition_variable>
#include <algorithm>
#define NOMINMAX
#include <windows.h>
#define TDBG(msg) OutputDebugStringA("[TL] " msg "\n")
#define TDBGF(fmt, ...) { char _b[256]; sprintf_s(_b, "[TL] " fmt "\n", __VA_ARGS__); OutputDebugStringA(_b); }

#ifndef GL_COMPRESSED_RED_RGTC1
#define GL_COMPRESSED_RED_RGTC1 0x8DBB
#endif
#ifndef GL_COMPRESSED_SIGNED_RED_RGTC1
#define GL_COMPRESSED_SIGNED_RED_RGTC1 0x8DBC
#endif
#ifndef GL_COMPRESSED_RG_RGTC2
#define GL_COMPRESSED_RG_RGTC2 0x8DBD
#endif
#ifndef GL_COMPRESSED_SIGNED_RG_RGTC2
#define GL_COMPRESSED_SIGNED_RG_RGTC2 0x8DBE
#endif
#ifndef GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT
#define GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT 0x8E8F
#endif
#ifndef GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT
#define GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT 0x8E8E
#endif
#ifndef GL_COMPRESSED_RGBA_BPTC_UNORM
#define GL_COMPRESSED_RGBA_BPTC_UNORM 0x8E8C
#endif
#ifndef GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM
#define GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM 0x8E8D
#endif

// DDS format constants
const uint32_t FOURCC_DXT1 = 0x31545844; // "DXT1"
const uint32_t FOURCC_DXT3 = 0x33545844; // "DXT3"
const uint32_t FOURCC_DXT5 = 0x35545844; // "DXT5"
const uint32_t FOURCC_DX10 = 0x30315844; // "DX10"
const uint32_t DDS_MAGIC = 0x20534444; // "DDS "

// Pixel format flags
const uint32_t DDPF_ALPHAPIXELS = 0x00000001;
const uint32_t DDPF_ALPHA = 0x00000002;
const uint32_t DDPF_FOURCC = 0x00000004;
const uint32_t DDPF_RGB = 0x00000040;
const uint32_t DDPF_YUV = 0x00000200;
const uint32_t DDPF_LUMINANCE = 0x00020000;

// DDS file header structure
struct DDSHeader {
    uint32_t dwSize;
    uint32_t dwFlags;
    uint32_t dwHeight;
    uint32_t dwWidth;
    uint32_t dwPitchOrLinearSize;
    uint32_t dwDepth;
    uint32_t dwMipMapCount;
    uint32_t dwReserved1[11];
    // DDS_PIXELFORMAT
    uint32_t dwPFSize;
    uint32_t dwPFFlags;
    uint32_t dwFourCC;
    uint32_t dwRGBBitCount;
    uint32_t dwRBitMask;
    uint32_t dwGBitMask;
    uint32_t dwBBitMask;
    uint32_t dwABitMask;
    // End DDS_PIXELFORMAT
    uint32_t dwCaps;
    uint32_t dwCaps2;
    uint32_t dwCaps3;
    uint32_t dwCaps4;
    uint32_t dwReserved2;
};

// DDS DX10 extended header structure
struct DDSHeaderDX10 {
    uint32_t dxgiFormat;
    uint32_t resourceDimension;
    uint32_t miscFlag;
    uint32_t arraySize;
    uint32_t miscFlags2;
};

enum DXGI_FORMAT {
    DXGI_FORMAT_UNKNOWN = 0,

    // Common uncompressed 32-bit
    DXGI_FORMAT_R8G8B8A8_UNORM = 28,
    DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
    DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
    DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
    DXGI_FORMAT_B8G8R8A8_UNORM = 87,
    DXGI_FORMAT_B8G8R8X8_UNORM = 88,

    // Legacy BCn (DXT1/3/5) + sRGB variants
    DXGI_FORMAT_BC1_UNORM = 71,
    DXGI_FORMAT_BC1_UNORM_SRGB = 72,
    DXGI_FORMAT_BC2_UNORM = 74,
    DXGI_FORMAT_BC2_UNORM_SRGB = 75,
    DXGI_FORMAT_BC3_UNORM = 77,
    DXGI_FORMAT_BC3_UNORM_SRGB = 78,

    // RGTC
    DXGI_FORMAT_BC4_UNORM = 80,
    DXGI_FORMAT_BC4_SNORM = 81,
    DXGI_FORMAT_BC5_UNORM = 83,
    DXGI_FORMAT_BC5_SNORM = 84,

    // BPTC
    DXGI_FORMAT_BC6H_UF16 = 95,
    DXGI_FORMAT_BC6H_SF16 = 96,
    DXGI_FORMAT_BC7_UNORM = 98,
    DXGI_FORMAT_BC7_UNORM_SRGB = 99,

    // Depth/stencil (optional)
    DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
    DXGI_FORMAT_D32_FLOAT = 40
};

// ---------------------------------------------------------
extern "C" __declspec(dllexport) int initGlew() {
    glewExperimental = GL_TRUE;
    GLenum err = glewInit();
    while (glGetError() != GL_NO_ERROR) {}  // glewInit generates GL_INVALID_ENUM on core profile
    if (err != GLEW_OK) {
        std::cerr << "GLEW initialization failed: " << glewGetErrorString(err) << std::endl;
        return -1;
    }
    if (!GLEW_ARB_texture_compression_bptc) {
        std::cerr << "BC7 (BPTC) not supported by your GPU/OpenGL driver." << std::endl;
        return 0;
    }
    std::cout << "CheckFunction: GLEW initialized successfully!" << std::endl;
    return 42;
}

// Shared: block size for compressed formats
static inline uint32_t GetBlockSize(GLuint format) {
    switch (format) {
        // 8-byte blocks
    case GL_COMPRESSED_RGBA_S3TC_DXT1_EXT:
    case GL_COMPRESSED_RED_RGTC1:
    case GL_COMPRESSED_SIGNED_RED_RGTC1:
        return 8;
        // 16-byte blocks
    case GL_COMPRESSED_RGBA_S3TC_DXT3_EXT:
    case GL_COMPRESSED_RGBA_S3TC_DXT5_EXT:
    case GL_COMPRESSED_RG_RGTC2:
    case GL_COMPRESSED_SIGNED_RG_RGTC2:
    case GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT:
    case GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT:
    case GL_COMPRESSED_RGBA_BPTC_UNORM:
    case GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM:
        return 16;
    default:
        return 16;
    }
}

// ----------------------------------------------------------------------
// DDS loader from file
extern "C" __declspec(dllexport) GLuint LoadTextureDDS(const char* filePath) {
    std::ifstream file(filePath, std::ios::binary);
    if (!file.is_open()) {
        std::cerr << "Failed to open file: " << filePath << std::endl;
        return 0;
    }

    uint32_t magicNumber;
    file.read(reinterpret_cast<char*>(&magicNumber), sizeof(magicNumber));
    if (magicNumber != DDS_MAGIC) {
        std::cerr << "Not a valid DDS file." << std::endl;
        return 0;
    }

    DDSHeader header;
    file.read(reinterpret_cast<char*>(&header), sizeof(header));
    if (header.dwSize != 124 || header.dwPFSize != 32) {
        std::cerr << "Invalid DDS header size." << std::endl;
        return 0;
    }

    GLuint format = 0;
    GLuint internalFormat = 0;
    GLuint dataFormat = 0;
    GLuint dataType = 0;
    bool isCompressed = false;

    GLuint er = glGetError();

    DDSHeaderDX10 headerDX10 = { 0 };

    if (header.dwFourCC == FOURCC_DX10) {
        file.read(reinterpret_cast<char*>(&headerDX10), sizeof(headerDX10));
        switch (headerDX10.dxgiFormat) {
            // Compressed (DXT/S3TC + RGTC + BPTC)
        case DXGI_FORMAT_BC1_UNORM:       format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC1_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC2_UNORM:       format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC2_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC3_UNORM:       format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC3_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC4_UNORM:       format = GL_COMPRESSED_RED_RGTC1; isCompressed = true; break;
        case DXGI_FORMAT_BC4_SNORM:       format = GL_COMPRESSED_SIGNED_RED_RGTC1; isCompressed = true; break;
        case DXGI_FORMAT_BC5_UNORM:       format = GL_COMPRESSED_RG_RGTC2; isCompressed = true; break;
        case DXGI_FORMAT_BC5_SNORM:       format = GL_COMPRESSED_SIGNED_RG_RGTC2; isCompressed = true; break;
        case DXGI_FORMAT_BC6H_UF16:       format = GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT; isCompressed = true; break;
        case DXGI_FORMAT_BC6H_SF16:       format = GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT; isCompressed = true; break;
        case DXGI_FORMAT_BC7_UNORM:       format = GL_COMPRESSED_RGBA_BPTC_UNORM; isCompressed = true; break;
        case DXGI_FORMAT_BC7_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM; isCompressed = true; break;

            // Uncompressed
        case DXGI_FORMAT_R8G8B8A8_UNORM:      internalFormat = GL_RGBA8;       dataFormat = GL_RGBA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_R8G8B8A8_UNORM_SRGB: internalFormat = GL_SRGB8_ALPHA8; dataFormat = GL_RGBA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_B8G8R8A8_UNORM:      internalFormat = GL_RGBA8;       dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_B8G8R8X8_UNORM:      internalFormat = GL_RGBA8;       dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_R16G16B16A16_FLOAT:  internalFormat = GL_RGBA16F;     dataFormat = GL_RGBA; dataType = GL_HALF_FLOAT;   isCompressed = false; break;
        case DXGI_FORMAT_R32G32B32A32_FLOAT:  internalFormat = GL_RGBA32F;     dataFormat = GL_RGBA; dataType = GL_FLOAT;        isCompressed = false; break;

        default:
            std::cerr << "Unsupported DXGI_FORMAT: " << headerDX10.dxgiFormat << std::endl;
            return 0;
        }
    }
    else if (header.dwPFFlags & DDPF_FOURCC) {
        isCompressed = true;
        switch (header.dwFourCC) {
        case FOURCC_DXT1: format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; break;
        case FOURCC_DXT3: format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; break;
        case FOURCC_DXT5: format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; break;
        default:
            std::cerr << "Unsupported DDS compressed format." << std::endl;
            return 0;
        }
    }
    else if ((header.dwPFFlags & DDPF_RGB) && (header.dwRGBBitCount == 32)) {
        // BGRA
        if (header.dwRBitMask == 0x00FF0000 &&
            header.dwGBitMask == 0x0000FF00 &&
            header.dwBBitMask == 0x000000FF &&
            header.dwABitMask == 0xFF000000) {
            internalFormat = GL_RGBA8; dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false;
        }
        // RGBA
        else if (header.dwRBitMask == 0x000000FF &&
            header.dwGBitMask == 0x0000FF00 &&
            header.dwBBitMask == 0x00FF0000 &&
            header.dwABitMask == 0xFF000000) {
            internalFormat = GL_RGBA8; dataFormat = GL_RGBA; dataType = GL_UNSIGNED_BYTE; isCompressed = false;
        }
        // X8B8G8R8 (no alpha)
        else if (header.dwRBitMask == 0x00FF0000 &&
            header.dwGBitMask == 0x0000FF00 &&
            header.dwBBitMask == 0x000000FF &&
            header.dwABitMask == 0x00000000) {
            internalFormat = GL_RGB8;  dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false;
        }
        else {
            std::cerr << "Unsupported 32bpp channel mask." << std::endl;
            return 0;
        }
    }
    else {
        std::cerr << "Unsupported DDS format." << std::endl;
        return 0;
    }

    uint32_t width = header.dwWidth;
    uint32_t height = header.dwHeight;
    uint32_t mipMapCount = header.dwMipMapCount ? header.dwMipMapCount : 1;
    bool hasMipmaps = (header.dwMipMapCount > 1);

    size_t totalSize = 0;
    {
        uint32_t w = width, h = height;
        for (uint32_t i = 0; i < mipMapCount && (w || h); ++i) {
            if (isCompressed) totalSize += ((w + 3) / 4) * ((h + 3) / 4) * GetBlockSize(format);
            else              totalSize += w * h * 4;
            w = std::max(1U, w / 2);
            h = std::max(1U, h / 2);
        }
    }

    std::vector<char> tex(totalSize);
    file.read(tex.data(), totalSize);
    file.close();

    // flush + count stale GL errors before touching GL
    int flushed = 0;
    while (glGetError() != GL_NO_ERROR) { flushed++; }
    TDBGF("LoadTextureDDS: %s  w=%u h=%u mips=%u compressed=%d flushed=%d",
        filePath, width, height, mipMapCount, (int)isCompressed, flushed);

    GLuint texId = 0;
    glGenTextures(1, &texId);
    TDBGF("  glGenTextures -> %u", texId);
    glBindTexture(GL_TEXTURE_2D, texId);
    glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

    size_t offset = 0;
    uint32_t w = width, h = height;
    for (uint32_t level = 0; level < mipMapCount && (w || h); ++level) {
        if (isCompressed) {
            size_t size = ((w + 3) / 4) * ((h + 3) / 4) * GetBlockSize(format);
            glCompressedTexImage2D(GL_TEXTURE_2D, level, format, w, h, 0, size, tex.data() + offset);
            offset += size;
        }
        else {
            size_t size = w * h * 4;
            glTexImage2D(GL_TEXTURE_2D, level, internalFormat, w, h, 0, dataFormat, dataType, tex.data() + offset);
            offset += size;
        }
        w = std::max(1U, w / 2);
        h = std::max(1U, h / 2);
    }

    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, hasMipmaps ? GL_LINEAR_MIPMAP_LINEAR : GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_BASE_LEVEL, 0);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, mipMapCount - 1);
    if (!hasMipmaps && !isCompressed) glGenerateMipmap(GL_TEXTURE_2D);

    GLenum err = glGetError();
    if (err != GL_NO_ERROR) {
        TDBGF("  FAIL GL error: 0x%X", err);
        glDeleteTextures(1, &texId);
        return 0;
    }
    TDBGF("  SUCCESS texId=%u", texId);
    return texId;
}

// ----------------------------------------------------------------------
// DDS loader from memory
extern "C" __declspec(dllexport) GLuint LoadTextureFromMemory(const void* data, size_t dataSize) {
    if (!data || dataSize == 0) {
        std::cerr << "Invalid memory data." << std::endl;
        return 0;
    }

    const uint32_t* magic = static_cast<const uint32_t*>(data);
    if (*magic != DDS_MAGIC) {
        std::cerr << "Not a valid DDS memory data." << std::endl;
        return 0;
    }

    const DDSHeader* header = reinterpret_cast<const DDSHeader*>(
        static_cast<const char*>(data) + sizeof(DDS_MAGIC)
        );
    if (header->dwSize != 124 || header->dwPFSize != 32) {
        std::cerr << "Invalid DDS header size in memory data." << std::endl;
        return 0;
    }

    GLuint format = 0, internalFormat = 0, dataFormat = 0, dataType = 0;
    bool isCompressed = false;

    const DDSHeaderDX10* headerDX10 = nullptr;
    size_t headerSize = sizeof(DDS_MAGIC) + sizeof(DDSHeader);

    if (header->dwFourCC == FOURCC_DX10) {
        headerDX10 = reinterpret_cast<const DDSHeaderDX10*>(
            reinterpret_cast<const char*>(header) + sizeof(DDSHeader)
            );
        headerSize += sizeof(DDSHeaderDX10);

        switch (headerDX10->dxgiFormat) {
            // Compressed
        case DXGI_FORMAT_BC1_UNORM:       format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC1_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT1_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC2_UNORM:       format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC2_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT3_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC3_UNORM:       format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC3_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_S3TC_DXT5_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC4_UNORM:       format = GL_COMPRESSED_RED_RGTC1; isCompressed = true; break;
        case DXGI_FORMAT_BC4_SNORM:       format = GL_COMPRESSED_SIGNED_RED_RGTC1; isCompressed = true; break;
        case DXGI_FORMAT_BC5_UNORM:       format = GL_COMPRESSED_RG_RGTC2; isCompressed = true; break;
        case DXGI_FORMAT_BC5_SNORM:       format = GL_COMPRESSED_SIGNED_RG_RGTC2; isCompressed = true; break;
        case DXGI_FORMAT_BC6H_UF16:       format = GL_COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT; isCompressed = true; break;
        case DXGI_FORMAT_BC6H_SF16:       format = GL_COMPRESSED_RGB_BPTC_SIGNED_FLOAT; isCompressed = true; break;
        case DXGI_FORMAT_BC7_UNORM:       format = GL_COMPRESSED_RGBA_BPTC_UNORM; isCompressed = true; break;
        case DXGI_FORMAT_BC7_UNORM_SRGB:  format = GL_COMPRESSED_SRGB_ALPHA_BPTC_UNORM; isCompressed = true; break;

            // Uncompressed
        case DXGI_FORMAT_R8G8B8A8_UNORM:      internalFormat = GL_RGBA8;        dataFormat = GL_RGBA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_R8G8B8A8_UNORM_SRGB: internalFormat = GL_SRGB8_ALPHA8; dataFormat = GL_RGBA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_B8G8R8A8_UNORM:      internalFormat = GL_RGBA8;        dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_B8G8R8X8_UNORM:      internalFormat = GL_RGBA8;        dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false; break;
        case DXGI_FORMAT_R16G16B16A16_FLOAT:  internalFormat = GL_RGBA16F;      dataFormat = GL_RGBA; dataType = GL_HALF_FLOAT;    isCompressed = false; break;
        case DXGI_FORMAT_R32G32B32A32_FLOAT:  internalFormat = GL_RGBA32F;      dataFormat = GL_RGBA; dataType = GL_FLOAT;         isCompressed = false; break;

        default:
            std::cerr << "Unsupported DXGI format in DX10 memory data: " << headerDX10->dxgiFormat << std::endl;
            return 0;
        }
    }
    else if (header->dwPFFlags & DDPF_FOURCC) {
        isCompressed = true;
        switch (header->dwFourCC) {
        case FOURCC_DXT1: format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; break;
        case FOURCC_DXT3: format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; break;
        case FOURCC_DXT5: format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; break;
        default:
            std::cerr << "Unsupported DDS compressed format in memory data." << std::endl;
            return 0;
        }
    }
    else if ((header->dwPFFlags & DDPF_RGB) && (header->dwRGBBitCount == 32)) {
        // BGRA
        if (header->dwRBitMask == 0x00FF0000 &&
            header->dwGBitMask == 0x0000FF00 &&
            header->dwBBitMask == 0x000000FF &&
            header->dwABitMask == 0xFF000000) {
            internalFormat = GL_RGBA8; dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false;
        }
        // RGBA
        else if (header->dwRBitMask == 0x000000FF &&
            header->dwGBitMask == 0x0000FF00 &&
            header->dwBBitMask == 0x00FF0000 &&
            header->dwABitMask == 0xFF000000) {
            internalFormat = GL_RGBA8; dataFormat = GL_RGBA; dataType = GL_UNSIGNED_BYTE; isCompressed = false;
        }
        // X8B8G8R8 (no alpha)
        else if (header->dwRBitMask == 0x00FF0000 &&
            header->dwGBitMask == 0x0000FF00 &&
            header->dwBBitMask == 0x000000FF &&
            header->dwABitMask == 0x00000000) {
            internalFormat = GL_RGB8; dataFormat = GL_BGRA; dataType = GL_UNSIGNED_BYTE; isCompressed = false;
        }
        else {
            std::cerr << "Unsupported 32bpp channel mask: "
                << std::hex
                << "R=" << header->dwRBitMask << " "
                << "G=" << header->dwGBitMask << " "
                << "B=" << header->dwBBitMask << " "
                << "A=" << header->dwABitMask
                << std::dec << std::endl;
            return 0;
        }
    }
    else {
        std::cerr << "Unsupported DDS format in memory data." << std::endl;
        return 0;
    }

    // Data pointer just past headers
    const char* textureData = static_cast<const char*>(data) + headerSize;
    size_t textureDataSize = dataSize - headerSize;

    GLuint texId;
    glGenTextures(1, &texId);
    glBindTexture(GL_TEXTURE_2D, texId);
    glPixelStorei(GL_UNPACK_ALIGNMENT, 1);

    uint32_t width = header->dwWidth;
    uint32_t height = header->dwHeight;
    uint32_t mipMapCount = header->dwMipMapCount ? header->dwMipMapCount : 1;

    size_t offset = 0;
    for (uint32_t level = 0; level < mipMapCount && (width || height); ++level) {
        size_t mipSize = 0;
        if (isCompressed) {
            mipSize = ((width + 3) / 4) * ((height + 3) / 4) * GetBlockSize(format);
            if (offset + mipSize > textureDataSize) { std::cerr << "Texture data overflow." << std::endl; glDeleteTextures(1, &texId); return 0; }
            glCompressedTexImage2D(GL_TEXTURE_2D, level, format, width, height, 0, mipSize, textureData + offset);
        }
        else {
            mipSize = width * height * 4;
            if (offset + mipSize > textureDataSize) { std::cerr << "Texture data overflow." << std::endl; glDeleteTextures(1, &texId); return 0; }
            glTexImage2D(GL_TEXTURE_2D, level, internalFormat, width, height, 0, dataFormat, dataType, textureData + offset);
        }
        offset += mipSize;
        width = std::max(1U, width / 2);
        height = std::max(1U, height / 2);
    }

    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, (mipMapCount > 1) ? GL_LINEAR_MIPMAP_LINEAR : GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_BASE_LEVEL, 0);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAX_LEVEL, mipMapCount - 1);
    if (mipMapCount <= 1 && !isCompressed) glGenerateMipmap(GL_TEXTURE_2D);

    GLenum err = glGetError();
    if (err != GL_NO_ERROR) {
        std::cerr << "OpenGL Error: " << err << std::endl;
        glDeleteTextures(1, &texId);
        return 0;
    }
    return texId;
}
