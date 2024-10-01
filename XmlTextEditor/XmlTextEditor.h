#pragma once

#include "resource.h"
#pragma once

#include "resource.h"  // Make sure to include the resource header for menu definitions
#include <windows.h>
#include <richedit.h>
#include <commdlg.h>   // For file dialogs

// Global Variables:
extern HINSTANCE hInst;                          // current instance
extern WCHAR szTitle[];                          // The title bar text
extern WCHAR szWindowClass[];                    // the main window class name

// Function Declarations
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

// File Handling Functions
void OpenFile(HWND hWnd);                        // Function to handle the "Open" functionality
void SaveFile(HWND hWnd, BOOL bSaveAs);          // Function to handle "Save" and "Save As" functionality

// Syntax Highlighting (Optional, for future usage)
void ApplySyntaxHighlighting(HWND hEdit);        // Function to apply basic XML syntax highlighting
