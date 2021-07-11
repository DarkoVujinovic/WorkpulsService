*This is step to step guide for the installation of WorkpulsServiceDemo windows service application*

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