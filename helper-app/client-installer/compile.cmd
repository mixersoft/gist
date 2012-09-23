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

mkdir %ObjDir% 2> nul
mkdir %BinDir% 2> nul

set Target=..\client\bin\%Config%\SnaphappiHelper.exe

set ExtDir=C:\Program Files (x86)\WiX Toolset v3.6\bin

(
	:: Compile the source files into object files
	candle -dclient.TargetPath="%Target%" -nologo -o "%Obj%" "%Src%.wxs"

	:: Link the object files into the installer
	light -ext "%ExtDir%\WixNetFxExtension.dll" -ext "%ExtDir%\WixUIExtension.dll" -nologo -o "%Bin%" "%Obj%"
)
