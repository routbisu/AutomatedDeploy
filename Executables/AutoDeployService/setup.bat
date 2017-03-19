net stop "Auto Deploy Service"

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" /u "C:\Projects\AD_Sandbox\binary\AD.AutoDeployService.exe"

"C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe" "C:\Projects\AD_Sandbox\binary\AD.AutoDeployService.exe"

net start "Auto Deploy Service"