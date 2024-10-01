// resource.h
#define IDM_OPEN      101
#define IDM_SAVE      102
#define IDM_SAVEAS    103
#define IDM_EXIT      104
#define IDM_ABOUT     105
#define IDI_XMLTEXTEDITOR  106
#define IDC_XMLTEXTEDITOR  107
#define IDS_APP_TITLE  108

// Next default values for new objects
//

#ifdef APSTUDIO_INVOKED
#ifndef APSTUDIO_READONLY_SYMBOLS

#define _APS_NO_MFC					130
#define _APS_NEXT_RESOURCE_VALUE	129
#define _APS_NEXT_COMMAND_VALUE		32771
#define _APS_NEXT_CONTROL_VALUE		1000
#define _APS_NEXT_SYMED_VALUE		110
#endif
#endif

IDR_MAINFRAME MENU
BEGIN
POPUP "&File"
BEGIN
MENUITEM "&Open", IDM_OPEN
MENUITEM "&Save", IDM_SAVE
MENUITEM "Save &As...", IDM_SAVEAS
MENUITEM SEPARATOR
MENUITEM "E&xit", IDM_EXIT
END
POPUP "&Help"
BEGIN
MENUITEM "&About", IDM_ABOUT
END
END

IDI_XMLTEXTEDITOR ICON "path_to_icon_file.ico"
IDI_SMALL ICON "path_to_small_icon_file.ico"