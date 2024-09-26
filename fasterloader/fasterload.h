#pragma once
#ifndef TEXTURELOADER_H
#define TEXTURELOADER_H
#

#include <glew.h>  // Include GLEW for OpenGL function definitions
#include <stdint.h>   // For int32_t

#ifdef __cplusplus
extern "C" {
#endif

    // Enum to represent texture loading errors
    typedef enum {
        TEXTURE_NO_ERROR = 0,
        TEXTURE_ERROR_FILE_NOT_FOUND,
        TEXTURE_ERROR_INVALID_FORMAT,
        TEXTURE_ERROR_GL_CONTEXT,
        TEXTURE_ERROR_LOADING_FAILED
    } TextureError;

    // Function to check for OpenGL errors
    __declspec(dllexport) TextureError CheckGLError();

    // Function to load a DDS texture from a file
    // Returns the OpenGL texture ID and sets the error code
    __declspec(dllexport) GLuint FasterLoadTexture(const char* filePath, TextureError* error);


#ifdef __cplusplus
}
#endif

#endif // TEXTURELOADER_H
