!include LogicLib.nsh
!include "nsDialogs.nsh"

; !include "MUI2.nsh"

Icon icons\kkl.ico
UninstallIcon icons\kkl.ico
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
    ; SetOutPath "$INSTDIR\icons"
	
    ; Files to be installed: Icons
    ; File "icons\speaker.ico"
    ; File "icons\english.ico"
    ; File "icons\hanzi.ico"
    ; File "icons\pinyin.ico"
	
	
    SetOutPath "$INSTDIR\fonts"
	File "fonts\SourceHanSerif.otf"
	
    SetOutPath "$INSTDIR\runtimes"
	File /r "bin\Release\net5.0-windows\runtimes\*.*"
	
    SetOutPath $InstDir
    ; Files to be installed: Binaries
    File "bin\Release\net5.0-windows\KarteiKartenLernen.dll"
    File "bin\Release\net5.0-windows\KarteiKartenLernen.exe"
    File "bin\Release\net5.0-windows\KarteiKartenLernen.runtimeconfig.json"
    File "bin\Release\net5.0-windows\libmp3lame.32.dll"
	
    File "bin\Release\net5.0-windows\libmp3lame.64.dll"
    File "bin\Release\net5.0-windows\NAudio.Asio.dll"
    File "bin\Release\net5.0-windows\NAudio.Core.dll"
    File "bin\Release\net5.0-windows\NAudio.dll"
    File "bin\Release\net5.0-windows\NAudio.Lame.dll"
	
    File "bin\Release\net5.0-windows\NAudio.Midi.dll"
    File "bin\Release\net5.0-windows\NAudio.Wasapi.dll"
    File "bin\Release\net5.0-windows\NAudio.WinForms.dll"
    File "bin\Release\net5.0-windows\NAudio.WinMM.dll"
    File "bin\Release\net5.0-windows\Newtonsoft.Json.dll"
	
    File "bin\Release\net5.0-windows\KarteiKartenLernen.deps.json"
    File "bin\Release\net5.0-windows\Microsoft.CognitiveServices.Speech.csharp.dll"
	

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