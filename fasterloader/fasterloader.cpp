#define _CRT_SECURE_NO_WARNINGS

#include "fasterload.h"
#include <stdio.h>    // For file handling
#include <string.h>   // For string operations

// Helper function to check for OpenGL errors
TextureError CheckGLError() {
    GLenum error = glGetError();
    if (error == GL_NO_ERROR) return TEXTURE_NO_ERROR;
    // You can add more specific checks here if needed
    return TEXTURE_ERROR_LOADING_FAILED;
}

GLuint FasterLoadTexture(const char* filePath, TextureError* error) {
    if (!filePath || strlen(filePath) == 0) {
        *error = TEXTURE_ERROR_FILE_NOT_FOUND;
        return 0;
    }

    // Open the DDS file
    FILE* file = fopen(filePath, "rb");
    if (!file) {
        *error = TEXTURE_ERROR_FILE_NOT_FOUND;
        return 0;
    }

    // Load DDS file header and data here (simplified example)
    // You will need to parse the DDS header and load the data properly
    // Assume you have a valid DDS file with BC3_UNORM format

    // Create and bind the OpenGL texture
    GLuint textureID;
    glGenTextures(1, &textureID);
    glBindTexture(GL_TEXTURE_2D, textureID);

    // Load texture data to OpenGL (This is a simplified example)
    // Replace this with actual DDS data loading and handling
    // For BC3_UNORM, you might use glCompressedTexImage2D

    // Generate mipmaps for the texture
    glGenerateMipmap(GL_TEXTURE_2D);

    // Set texture parameters (if needed)
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);

    // Check for OpenGL errors
    *error = CheckGLError();
    if (*error != TEXTURE_NO_ERROR) {
        // Cleanup in case of error
        if (textureID > 0) {
        glDeleteTextures(1, &textureID);
        fclose(file);
        return 0;
        }

    }

    // Close the file and return the texture ID
    fclose(file);
    *error = TEXTURE_NO_ERROR;
    return textureID;
}


