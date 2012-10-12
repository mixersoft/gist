:: Installer compilation script for those who do not have Visual Studio Wix integration

@echo off
setlocal

set PATH=%PATH%;c:\Program Files (x86)\WiX Toolset v3.6\bin

set Project=SnaphappiHelperSetup
set Config=Debug

set Src=Product

set ObjDir=obj\Debug
set Obj=%ObjDir%\%Src%.wixobj

set BinDir=bin\Debug
set Bin=%BinDir%\%Project%.msi

set Target=..\client\bin\%Config%\SnaphappiHelper.exe

set ProjectDir=..\client

set ExtDir=C:\Program Files (x86)\WiX Toolset v3.6\bin

pushd client-installer

mkdir %ObjDir% 2> nul
mkdir %BinDir% 2> nul

:: Compile the source files into object files
candle -dclient.TargetPath="%Target%" -dclient.ProjectDir="%ProjectDir%" -nologo -o "%Obj%" "%Src%.wxs"

:: Link the object files into the installer
light -ext "%ExtDir%\WixNetFxExtension.dll" -ext "%ExtDir%\WixUIExtension.dll" -nologo -o "%Bin%" "%Obj%"

popd

pushd client-installer-bootstrapper\

:: Create a bootstrapper

popd
