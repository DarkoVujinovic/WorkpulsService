*Quick description of WorkpulsServiceDemo Windows Service*
Built in .Net Framework 4.0

Service has been intended to monitor what application is currently in focus for all logged-in users on the PC and log this info in the C:\log-app.txt file. 
It also monitor camera and microphone usage and log all these informations in C:\log-camera.txt file.
Every time C:\log-app.txt is open, service log this information in C:\log-open-log-file.txt file with the user who opened the file and the time when it was open.

*This is step to step guide for the installation of WorkpulsServiceDemo Windows Service application*

Prerequisites: Microsoft VisualStudio 2019

1. Please open WorkpulsServiceDemo folder, find and run WorkpulsServiceDemo.sln file 
2. That should open all projects in Microsoft VisualStudio 2019
3. All executables must be built in the Microsoft VisualStudio 2019 [please set: Debug, Any CPU]
4  It should be 5 .exe files after successful build -> WorkpulsServiceDemo.exe, WorkpulsActiveWindow.exe, WorkpulsCamera.exe, WorkpulsMicrophone.exe, WorkpulsLogCheck.exe
5. Find all newly built executables and paste it to the 'C:\Windows' directory
6. Also in the WorkpulsServiceDemo\bin\Debug\ build directory find 'Topshelf.dll' (version 3.3.154) and also paste to the 'C:\Windows' directory
7. Run installService.bat file as Administrator
8. Go to the Services, find WorkpulsServiceDemo and click Start
8. Enjoy :)