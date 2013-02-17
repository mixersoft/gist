:: Installer compilation script for those who do not have Visual Studio Wix integration

@echo off
setlocal

:: Set up configuration

set PATH=%PATH%;c:\Program Files (x86)\WiX Toolset v3.6\bin

set Config=Debug

set ClientInstallerTargetPath=..\client-installer\bin\%Config%\SnaphappiHelperSetup.msi
set ClientProjectDir=..\client
set ClientTargetPath=..\client\bin\%Config%\SnaphappiHelper.exe
set InstallDeviceIdDir=..\InstallDeviceID\bin\%Config%
set LauncherTargetPath=..\Launcher\bin\%Config%\Launcher.exe

set ExtDir=C:\Program Files (x86)\WiX Toolset v3.6\bin

:: Create the helper app installer
pushd client-installer

set Src=Product

set ObjDir=obj\%Config%
set Obj=%ObjDir%\%Src%.wixobj

set BinDir=bin\%Config%
set Bin=%BinDir%\SnaphappiHelperSetup.msi

mkdir %ObjDir% 2> nul
mkdir %BinDir% 2> nul

:: Compile the source files into object files
echo Compiling '%Obj%'...
candle -dclient.TargetPath="%ClientTargetPath%" -dclient.ProjectDir="%ClientProjectDir%" -dInstallDeviceID.TargetDir="%InstallDeviceIdDir%" -nologo -o "%Obj%" "%Src%.wxs"
if ERRORLEVEL 1 goto ErrorExit

:: Link the object files into the installer
echo Linking '%Bin%'...
light -ext "%ExtDir%\WixNetFxExtension.dll" -ext "%ExtDir%\WixUIExtension.dll" -nologo -o "%Bin%" "%Obj%"
if ERRORLEVEL 1 goto ErrorExit

popd

:: Load and increment build count
set BuildCountPath=client-installer-version\build-count.txt
set/p BuildCount=<%BuildCountPath%
set/a BuildCount=BuildCount+1
echo %BuildCount% > %BuildCountPath%

:: Create the bootstrapper
pushd client-installer-bootstrapper

set Src=Bundle

set ObjDir=obj\%Config%
set Obj=%ObjDir%\%Src%.wixobj

set BinDir=bin\%Config%
set Bin=%BinDir%\SnaphappiSetup.exe

mkdir %ObjDir% 2> nul
mkdir %BinDir% 2> nul

:: Compile the source files into object files
echo Compiling '%Obj%'...
candle -ext "%ExtDir%\WixBalExtension.dll" -ext "%ExtDir%\WixUtilExtension.dll" -dclient-installer.TargetPath="%ClientInstallerTargetPath%" -dLauncher.TargetPath="%LauncherTargetPath%" -dBuildCount=%BuildCount% -nologo -o "%Obj%" "%Src%.wxs"
if ERRORLEVEL 1 goto ErrorExit

:: Link the object files into the installer
echo Linking '%Bin%'...
light -ext "%ExtDir%\WixBalExtension.dll" -ext "%ExtDir%\WixUtilExtension.dll" -nologo -o "%Bin%" "%Obj%"
if ERRORLEVEL 1 goto ErrorExit

popd

echo Done

exit

: ErrorExit
echo An error has occurred.
pause
