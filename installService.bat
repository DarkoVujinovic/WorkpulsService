@echo off
cd C:\Windows
WorkpulsServiceDemo.exe install

if ERRORLEVEL 1 goto error
exit
:error
echo There was a problem
pause