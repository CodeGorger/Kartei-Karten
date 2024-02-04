!include LogicLib.nsh
!include "nsDialogs.nsh"

; !include "MUI2.nsh"

Icon kkl.ico
UninstallIcon kkl.ico
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
; RequestExecutionLevel admin


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
    File "bin\publish\speaker.png"
    File "bin\publish\KarteiKartenLernen.dll"
    File "bin\publish\KarteiKartenLernen.exe"
    File "bin\publish\KarteiKartenLernen.runtimeconfig.json"
    File "bin\publish\libmp3lame.32.dll"
	
    File "bin\publish\libmp3lame.64.dll"
    File "bin\publish\NAudio.Asio.dll"
    File "bin\publish\NAudio.Core.dll"
    File "bin\publish\NAudio.dll"
    File "bin\publish\NAudio.Lame.dll"
	
    File "bin\publish\NAudio.Midi.dll"
    File "bin\publish\NAudio.Wasapi.dll"
    File "bin\publish\NAudio.WinForms.dll"
    File "bin\publish\NAudio.WinMM.dll"

    ; Create a desktop shortcut
    CreateShortCut "$DESKTOP\FlashCardLearning.lnk" "$InstDir\KarteiKartenLernen.exe" ; Replace YourExecutable.exe with the actual executable name

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