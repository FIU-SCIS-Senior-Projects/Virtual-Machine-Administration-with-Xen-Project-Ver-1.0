Directory descriptions are as follows...


Golang UI
===================
console.go - contains the GO UI code for the server
introspector.conf -settings file that the console.go file edits, necessary to run the server


VMAX Web Client
===================
Contains all of the code and cs files necessary to open the Web client from Visual Studio

*XenMaster is a dependency see the XenMaster description below


VMAX Windows Client
===================
Contains all the code and C# files necessary to open the Windows client in Visual Studio

*XenMaster is a dependency see the XenMaster description below


XenMaster
===================
XenMaster is a dependency for both the VMAX Windows Client and the VMAX Web Client. Due to this, it is necessary
to either rebuild XenMaster from Visual Studio and import/reference the .../bin/Debug/XenMaster.dll library file from
either of the projects or include it as a project reference