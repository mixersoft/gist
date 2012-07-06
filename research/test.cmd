@echo off
set nunit="C:\Program Files (x86)\NUnit 2.6\bin\nunit-console.exe"
%nunit%  /nologo /noresult /framework=4.0 bin\ImageGroupingTest.dll
