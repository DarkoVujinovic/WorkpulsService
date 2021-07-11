@echo off
cd C:\Windows
WorkpulsServiceDemo.exe uninstall

if ERRORLEVEL 1 goto error
exit
:error
echo There was a problem
pause