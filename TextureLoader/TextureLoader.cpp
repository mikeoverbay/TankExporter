#include <glew.h>
#include <GL/gl.h>
#include <cstdint>
#include <iostream>
#include <fstream>
#include <vector>
#include <thread>
#include <mutex>
#include <condition_variable>

// DDS format constants
const uint32_t FOURCC_DXT1 = 0x31545844; // "DXT1" in ASCII
const uint32_t FOURCC_DXT3 = 0x33545844; // "DXT3" in ASCII
const uint32_t FOURCC_DXT5 = 0x35545844; // "DXT5" in ASCII
const uint32_t FOURCC_DX10 = 0x30315844; // "DX10" in ASCII
const uint32_t DDS_MAGIC = 0x20534444;   // "DDS " in ASCII

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

// DXGI_FORMAT values used in DX10 DDS files
enum DXGI_FORMAT {
    DXGI_FORMAT_BC1_UNORM = 71, // DXT1
    DXGI_FORMAT_BC2_UNORM = 74, // DXT3
    DXGI_FORMAT_BC3_UNORM = 77, // DXT5
    // Add more as needed...
};

// init glew
extern "C" __declspec(dllexport) int initGlew() {
    // Initialize GLEW
    GLenum err = glewInit();
    if (err != GLEW_OK) {
        std::cerr << "GLEW initialization failed: " << glewGetErrorString(err) << std::endl;
        return -1; // Return -1 if GLEW initialization fails
    }

    // Echo successful GLEW initialization
    std::cout << "CheckFunction: GLEW initialized successfully!" << std::endl;

    // Additional checks or logs can be added here
    return 42; // Return a test value to indicate success
}

// Function to load DDS texture from disk
extern "C" __declspec(dllexport) GLuint LoadTextureDDS(const char* filePath) {
    std::ifstream file(filePath, std::ios::binary);
    if (!file.is_open()) {
        std::cerr << "Failed to open file: " << filePath << std::endl;
        return 0;
    }

    // Read and verify the magic number
    uint32_t magicNumber;
    file.read(reinterpret_cast<char*>(&magicNumber), sizeof(magicNumber));
    if (magicNumber != DDS_MAGIC) {
        std::cerr << "Not a valid DDS file." << std::endl;
        return 0;
    }

    // Read the main DDS header
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

    DDSHeaderDX10 headerDX10 = { 0 };

    // Check for DX10 extended header
    if (header.dwFourCC == FOURCC_DX10) {
        file.read(reinterpret_cast<char*>(&headerDX10), sizeof(headerDX10));
        switch (headerDX10.dxgiFormat) {
        case DXGI_FORMAT_BC1_UNORM: format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC2_UNORM: format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC3_UNORM: format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; isCompressed = true; break;
        default:
            std::cerr << "Unsupported DXGI format in DX10 header." << std::endl;
            return 0;
        }
    }
    else if (header.dwPFFlags & DDPF_FOURCC) {
        // Handle legacy compressed formats
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
        // Handle uncompressed 32bpp BGRA
        if (header.dwRBitMask == 0x00FF0000 &&
            header.dwGBitMask == 0x0000FF00 &&
            header.dwBBitMask == 0x000000FF &&
            header.dwABitMask == 0xFF000000) {
            internalFormat = GL_RGBA8;
            dataFormat = GL_BGRA;
            dataType = GL_UNSIGNED_BYTE;
            isCompressed = false;
        }
        else {
            std::cerr << "Unsupported 32bpp format." << std::endl;
            return 0;
        }
    }
    else {
        std::cerr << "Unsupported DDS format." << std::endl;
        return 0;
    }

    // Calculate data size and read texture data
    uint32_t dataSize = 0;
    uint32_t width = header.dwWidth;
    uint32_t height = header.dwHeight;
    bool hasMipmaps = (header.dwMipMapCount > 1);
    uint32_t mipMapCount = header.dwMipMapCount ? header.dwMipMapCount : 1;

    if (isCompressed) {
        uint32_t blockSize = (format == GL_COMPRESSED_RGBA_S3TC_DXT1_EXT) ? 8 : 16;
        for (uint32_t i = 0; i < mipMapCount && (width || height); ++i) {
            uint32_t size = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
            dataSize += size;
            width = std::max(1U, width / 2);
            height = std::max(1U, height / 2);
        }
    }
    else {
        for (uint32_t i = 0; i < mipMapCount && (width || height); ++i) {
            uint32_t size = width * height * 4; // 4 bytes per pixel
            dataSize += size;
            width = std::max(1U, width / 2);
            height = std::max(1U, height / 2);
        }
    }

    std::vector<char> textureData(dataSize);
    file.read(textureData.data(), dataSize);
    file.close();

    GLuint textureID;
    glGenTextures(1, &textureID);
    glBindTexture(GL_TEXTURE_2D, textureID);

    // Upload texture data
    width = header.dwWidth;
    height = header.dwHeight;
    uint32_t offset = 0;
    for (uint32_t i = 0; i < mipMapCount && (width || height); ++i) {
        if (isCompressed) {
            uint32_t blockSize = (format == GL_COMPRESSED_RGBA_S3TC_DXT1_EXT) ? 8 : 16;
            uint32_t size = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
            glCompressedTexImage2D(GL_TEXTURE_2D, i, format, width, height, 0, size, textureData.data() + offset);
            offset += size;
        }
        else {
            uint32_t size = width * height * 4; // 4 bytes per pixel
            glTexImage2D(GL_TEXTURE_2D, i, internalFormat, width, height, 0, dataFormat, dataType, textureData.data() + offset);
            offset += size;
        }
        width = std::max(1U, width / 2);
        height = std::max(1U, height / 2);
    }

    // Set texture parameters
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, hasMipmaps ? GL_LINEAR_MIPMAP_LINEAR : GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

    if (!hasMipmaps) {
        // Generate mipmaps if they don't exist in the DDS file
        glGenerateMipmap(GL_TEXTURE_2D);
    }

    GLenum error = glGetError();
    if (error != GL_NO_ERROR) {
        std::cerr << "OpenGL Error: " << error << std::endl;
        glDeleteTextures(1, &textureID);
        return 0;
    }

    //std::cout << "Loaded DDS texture: " << filePath << ", Texture ID: " << textureID << std::endl;
    return textureID;
}

// Function to load DDS texture from memory
extern "C" __declspec(dllexport) GLuint LoadTextureFromMemory(const void* data, size_t dataSize) {
    if (!data || dataSize == 0) {
        std::cerr << "Invalid memory data." << std::endl;
        return 0; // Return 0 on failure
    }

    // Read the magic number from memory
    const uint32_t* magicNumber = static_cast<const uint32_t*>(data);
    if (*magicNumber != DDS_MAGIC) {
        std::cerr << "Not a valid DDS memory data." << std::endl;
        return 0;
    }

    // Read the DDS header from memory
    const DDSHeader* header = reinterpret_cast<const DDSHeader*>(static_cast<const char*>(data) + sizeof(DDS_MAGIC));
    if (header->dwSize != 124 || header->dwPFSize != 32) {
        std::cerr << "Invalid DDS header size in memory data." << std::endl;
        return 0;
    }

    GLuint format = 0;
    GLuint internalFormat = 0;
    GLuint dataFormat = 0;
    GLuint dataType = 0;
    bool isCompressed = false;
    const DDSHeaderDX10* headerDX10 = nullptr;

    // Check for DX10 extended header
    if (header->dwFourCC == FOURCC_DX10) {
        headerDX10 = reinterpret_cast<const DDSHeaderDX10*>(reinterpret_cast<const char*>(header) + sizeof(DDSHeader));
        switch (headerDX10->dxgiFormat) {
        case DXGI_FORMAT_BC1_UNORM: format = GL_COMPRESSED_RGBA_S3TC_DXT1_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC2_UNORM: format = GL_COMPRESSED_RGBA_S3TC_DXT3_EXT; isCompressed = true; break;
        case DXGI_FORMAT_BC3_UNORM: format = GL_COMPRESSED_RGBA_S3TC_DXT5_EXT; isCompressed = true; break;
        default:
            std::cerr << "Unsupported DXGI format in DX10 memory data." << std::endl;
            return 0;
        }
    }
    else if (header->dwPFFlags & DDPF_FOURCC) {
        // Handle legacy compressed formats
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
        // Handle uncompressed 32bpp BGRA
        if (header->dwRBitMask == 0x00FF0000 &&
            header->dwGBitMask == 0x0000FF00 &&
            header->dwBBitMask == 0x000000FF &&
            header->dwABitMask == 0xFF000000) {
            internalFormat = GL_RGBA8;
            dataFormat = GL_BGRA;
            dataType = GL_UNSIGNED_BYTE;
            isCompressed = false;
        }
        else {
            std::cerr << "Unsupported 32bpp format in memory data." << std::endl;
            return 0;
        }
    }
    else {
        std::cerr << "Unsupported DDS format in memory data." << std::endl;
        return 0;
    }

    // Calculate the header size
    size_t headerSize = sizeof(DDS_MAGIC) + sizeof(DDSHeader);
    if (header->dwFourCC == FOURCC_DX10) {
        headerSize += sizeof(DDSHeaderDX10);
    }

    // Pointer to the texture data
    const char* textureData = static_cast<const char*>(data) + headerSize;
    size_t textureDataSize = dataSize - headerSize;

    // Generate texture
    GLuint textureID;
    glGenTextures(1, &textureID);
    glBindTexture(GL_TEXTURE_2D, textureID);

    // Upload texture data
    uint32_t width = header->dwWidth;
    uint32_t height = header->dwHeight;
    uint32_t mipMapCount = header->dwMipMapCount ? header->dwMipMapCount : 1; // Ensure at least one mipmap level

    size_t offset = 0;
    for (uint32_t i = 0; i < mipMapCount && (width || height); ++i) {
        size_t mipSize = 0;
        if (isCompressed) {
            size_t blockSize = (format == GL_COMPRESSED_RGBA_S3TC_DXT1_EXT) ? 8 : 16;
            mipSize = ((width + 3) / 4) * ((height + 3) / 4) * blockSize;
            if (offset + mipSize > textureDataSize) {
                std::cerr << "Texture data overflow." << std::endl;
                glDeleteTextures(1, &textureID);
                return 0;
            }
            glCompressedTexImage2D(GL_TEXTURE_2D, i, format, width, height, 0, mipSize, textureData + offset);
        }
        else {
            mipSize = width * height * 4; // 4 bytes per pixel
            if (offset + mipSize > textureDataSize) {
                std::cerr << "Texture data overflow." << std::endl;
                glDeleteTextures(1, &textureID);
                return 0;
            }
            glTexImage2D(GL_TEXTURE_2D, i, internalFormat, width, height, 0, dataFormat, dataType, textureData + offset);
        }
        offset += mipSize;
        width = std::max(1U, width / 2);
        height = std::max(1U, height / 2);
    }

    // Set texture parameters
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, mipMapCount > 1 ? GL_LINEAR_MIPMAP_LINEAR : GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);

    if (mipMapCount <= 1) {
        // Generate mipmaps if they don't exist in the DDS data
        glGenerateMipmap(GL_TEXTURE_2D);
    }

    GLenum error = glGetError();
    if (error != GL_NO_ERROR) {
        std::cerr << "OpenGL Error: " << error << std::endl;
        glDeleteTextures(1, &textureID);
        return 0;
    }

    return textureID;
}
