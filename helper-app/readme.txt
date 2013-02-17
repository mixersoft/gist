This is a solution devoted to the Snaphappi helper application. Here is a quick run-down of all the constituent projects:


api
Contains the Apache Thrift API definition and the generated code.

client
Is the main helper app C# project.

client-installer
Creates an MSI installer for the helper app itself.

client-installer-bootstrapper
Creates a combined installer for the helper app and the top level folder Adobe AIR app. The ".exe" it produces is the final redistributable file for the user.

client-installer-version
Keeps track of your local build count and saves it for use by the bootstrapper. An incrementing build count is necessary for proper re-install of the generated installer.

client-test
Contains the unit tests for the "client".

InstallDeviceID
Contains a small DLL used by "client-installer" to create a device ID during installation.

Launcher
Contains a small application used by the bootstrapper to run the Adobe AIR installer.
