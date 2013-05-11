@echo off

:: create Nemerle folder and files

set nccdir=%ProgramFiles%\Nemerle
set dlls=Nemerle.dll Nemerle.Compiler.dll Nemerle.Macros.dll Nemerle.MSBuild.Tasks.dll
if not exist "%nccdir%" mkdir "%nccdir%"
for %%f in (ncc.exe %dlls%) do xcopy/i/y "%%f" "%nccdir%"

:: add Nemerle to GAC

pushd %nccdir%

for %%f in (%dlls%) do (
	ngen uninstall /nologo "%%f"
	gacutil /nologo /u "%%f"
	ngen install /nologo "%%f"
	gacutil /nologo /i "%%f"
)

:: add Nemerle to the system PATH

set path=%path%;%nccdir%
setx/m path "%PATH%;%nccdir%"

popd
