@echo off

:: Load and increment build count
set/p count=<%1
set/a count=count+1
echo %count% > %1

(
	echo ^<?xml version="1.0" encoding="utf-8"?^>
	echo ^<Include^>
	echo   ^<?define BuildCount="%count%" ?^>
	echo ^</Include^>
) > %2
