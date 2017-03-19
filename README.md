About AutomatedDeploy
----------------------
This service "monitors" a configurable file location in the system and watched for change in "File Size" or "Last Modified Date". Whenever a change is seen in the above parameters, a batch file is executed.
All these activies are logged in a log file.
Most parameters are configurable. The service can monitor any number of file locations.

Uses
-----
1. Can be used for automated deployment of a web application to IIS.
2. Can be used for automated deployment of a windows service.

Steps to Install / Uninstall
-----------------------------
Use the setup.bat file to install the service.
[OR]
Use the InstalLUtil.exe utility provided by .Net Framework
> InstallUtil.exe AutoDeployService.exe

Configuration File
------------------
The settings.config file contains all configurable paramters for the service:
1. Polling Location (milliseconds)
2. Log File Location
3. Monitor File Locations & Batch File Locations

How to setup automated deployment of a ASP.Net Web Application
--------------------------------------------------------------
1. Package the application into a single .zip file using Visual Studio along with IIS website path

[You can also use this command to create it] 
Example:
--------
"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" "src\TestApi\TestApi.csproj" /T:Build;Package
/p:Configuration=DEBUG /p:OutputPath="obj\DEBUG" /p:DeployIisAppPath="/TurboQA/TestAPI"
/p:VisualStudioVersion=14.0

2. Put it in a drop location
3. Write a batch file that use msdeploy.exe to deploy the application to IIS.

[Example]
"C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe" -verb:sync -
source:package="E:\JenkinsWorkspace\Turbo_QA\src\TestApi\obj\DEBUG\_PublishedWebsites\TestApi_Package\TestApi.zip" -
dest:auto,computerName=user-pc,Username=Turbo,password=<password> -allowUntrusted=true

4. Restart the Auto Deploy Service from Services Control Panel (services.msc)