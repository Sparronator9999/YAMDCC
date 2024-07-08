@echo off
setlocal enableextensions
set "vbspath=%temp%\elevate.vbs"

echo Checking for admin access...
net sess>nul 2>&1
if "%errorlevel%"=="0" (goto main) else goto runasadmin

:runasadmin
echo Elevating command prompt...

:: Create a VBS script to re-run this batch script as admin:
echo Set UAC = CreateObject^("Shell.Application"^) > %vbspath%
echo For Each strArg in WScript.Arguments >> %vbspath%
echo args = args ^& strArg ^& " "  >> %vbspath%
echo Next >> %vbspath%
echo UAC.ShellExecute "%~0", args, "", "runas", 1 >> %vbspath%
"%SystemRoot%\System32\WScript.exe" %vbspath% %*
exit /b

:main
:: Clean up after ourselves (if we elevated command prompt from this script):
if exist "%vbspath%" del "%vbspath%" >nul 2>&1
cd %~dp0

if not exist msifcsvc.exe (
	echo Please run this script with the MSI Fan Control Service
	echo executable ^(msifcsvc.exe^) next to this script.
	pause
	exit /b
)

set "InstallUtil=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe"
if not exist "%InstallUtil%" (
	echo Could not find InstallUtil.exe.
	echo Make sure the file exists at %InstallUtil%.
	echo You may also edit this script's InstallUtil variable if .NET Framework
	echo has been updated.
	pause
	exit /b
)

:installchoice
echo What would you like to do?
echo.
echo 1.  Install the MSI Fan Control Service.
echo 2.  Uninstall the MSI Fan Control Service.
echo 3.  Start the MSI Fan Control Service.
echo 4.  Stop the MSI Fan Control Service.
echo 5.  Exit.
echo.
choice /c 12345 /n /m "Your choice (1-5): "
if "%errorlevel%"=="1" goto install
if "%errorlevel%"=="2" goto uninstall
if "%errorlevel%"=="3" goto startsvc
if "%errorlevel%"=="4" goto stopsvc
if "%errorlevel%"=="5" exit /b
goto installchoice

:install
echo Installing the MSI Fan Control Service...
echo.
"%InstallUtil%" msifcsvc.exe
echo.
goto end

:uninstall
echo Uninstalling the MSI Fan Control Service...
echo.
"%InstallUtil%" /u msifcsvc.exe
echo.
goto end

:startsvc
net start msifcsvc
goto end

:stopsvc
net stop msifcsvc

:end
echo Done!
pause

if exist InstallUtil.InstallLog del /q InstallUtil.InstallLog
if exist msifcsvc.InstallLog del /q msifcsvc.InstallLog
