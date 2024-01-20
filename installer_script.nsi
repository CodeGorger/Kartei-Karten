!include LogicLib.nsh
!include "nsDialogs.nsh"

; !include "MUI2.nsh"

Icon kkl_new.ico
UninstallIcon kkl_new.ico
; !define MUI_ICON ".\kkl_new.ico"
; !define MUI_UNICON ".\kkl_new.ico"
; !define MUI_HEADERIMAGE
; !define MUI_HEADERIMAGE_BITMAP "path\to\InstallerLogo.bmp"
; !define MUI_HEADERIMAGE_RIGHT

; Name of the installer
Outfile "FlashCardLearningInstaller.exe"

; Default installation directory
InstallDir $PROGRAMFILES\FlashCardLearning

; Request application privileges for Windows Vista and later
RequestExecutionLevel admin


Page Directory
Page InstFiles

; Directory page
; Page directory 
; Var ANOTHER_DIR
; PageEx directory
  ; DirVar $ANOTHER_DIR
; PageExEnd
; !define MUI_FINISHPAGE_RUN_FUNCTION "StartNotepad"


; Default section
Section SectInstall
    ; Output path for installation
    SetOutPath $InstDir
    
    ; Files to be installed
    File "bin\publish\clrcompression.dll"
    File "bin\publish\clrjit.dll"
    File "bin\publish\coreclr.dll"
    File "bin\publish\D3DCompiler_47_cor3.dll"
    File "bin\publish\KarteiKartenLernen.exe"
    File "bin\publish\mscordaccore.dll"
    File "bin\publish\PenImc_cor3.dll"
    File "bin\publish\PresentationNative_cor3.dll"
    File "bin\publish\vcruntime140_cor3.dll"
    File "bin\publish\wpfgfx_cor3.dll"

    ; Create a desktop shortcut
    CreateShortCut "$DESKTOP\FlashCardLearning.lnk" "$InstDir\YourExecutable.exe" ; Replace YourExecutable.exe with the actual executable name

SectionEnd

; Uninstaller section
Section "Uninstall"
    ; Remove installed files
    Delete $InstDir\*.*
    
    ; Remove desktop shortcut
    Delete "$DESKTOP\FlashCardLearning.lnk"
    
    ; Remove the installation directory
    RMDir $InstDir

SectionEnd