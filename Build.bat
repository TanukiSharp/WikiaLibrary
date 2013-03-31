set NETFX_VERSION=4.0.30319
set MSBUILD=%WINDIR%\Microsoft.NET\Framework\v%NETFX_VERSION%\MSBuild.exe
set SOLUTION=WikiaLibrary

%MSBUILD% %SOLUTION%.sln /t:Rebuild /p:Configuration=Release

del %SOLUTION%.sln.cache

pause
