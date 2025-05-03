#define AppName "YAMDCC"
#define AppNameCE "Config Editor"
#define AppNameHH "Hotkey Handler"
#define AppVer "1.1.0"
#define AppPublisher "Sparronator9999"
#define AppURL "https://github.com/Sparronator9999/YAMDCC"
#define AppExeCE "ConfigEditor.exe"
#define AppExeHH "HotkeyHandler.exe"
#define AppExeSvc "yamdccsvc.exe"
#define BuildConfig "Debug"
; Used to determine which Win32 function to use (ANSI or Unicode version).
; Should resolve to "W" since Inno Setup 6 and later since the Unicode version is always used in that case.
#ifdef UNICODE
  #define AW "W"
#else
  #define AW "A"
#endif

[Setup]
AllowNetworkDrive=no
AllowUNCPath=no
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{AFE03526-3AAD-40FA-AF49-03A0150C4229}
AppName={#AppName}
AppVersion={#AppVer}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
ArchitecturesAllowed=x86compatible or x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
; we already handle stopping YAMDCC service ourselves
CloseApplicationsFilterExcludes={#AppExeSvc}
Compression=lzma/ultra64
DefaultDirName={autopf}\{#AppPublisher}\{#AppName}
DisableProgramGroupPage=yes
DisableWelcomePage=no
LicenseFile=Installer\LICENSE.rtf
LZMANumFastBytes=273
OutputBaseFilename=YAMDCC-v{#AppVer}-{#BuildConfig}-setup
SetupIconFile=YAMDCC.Updater\fan-update.ico
SetupMutex=YAMDCC-Setup-{{AFE03526-3AAD-40FA-AF49-03A0150C4229}
SolidCompression=yes
SourceDir=..
Uninstallable=Not WizardIsTaskSelected('portable')
UninstallDisplayIcon={app}\{#AppNameCE}
WizardStyle=classic
WizardImageFile=Installer\setup.bmp
WizardSmallImageFile=Installer\fan-update.bmp

[CustomMessages]
english.Portable=Portable mode (don't create uninstaller files or entries)
english.DeskIcons=Create desktop icons
english.DeskIconsCommon=For all users
english.DeskIconsUser=For the current user only
english.LaunchCE=Launch config editor

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "portable"; Description: "{cm:Portable}"; Flags: unchecked
Name: "deskicons"; Description: "{cm:DeskIcons}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "deskicons\common"; Description: "{cm:DeskIconsCommon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: exclusive unchecked
Name: "deskicons\user"; Description: "{cm:DeskIconsUser}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: exclusive unchecked

[Files]
Source: "YAMDCC.ConfigEditor\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}\{#AppNameCE}"; Filename: "{app}\{#AppExeCE}"
Name: "{autoprograms}\{#AppName}\{#AppNameHH}"; Filename: "{app}\{#AppExeHH}"
Name: "{commondesktop}\{#AppName} {#AppNameCE}"; Filename: "{app}\{#AppExeCE}"; Tasks: deskicons\common
Name: "{commondesktop}\{#AppName} {#AppNameHH}"; Filename: "{app}\{#AppExeHH}"; Tasks: deskicons\common
Name: "{userdesktop}\{#AppName} {#AppNameCE}"; Filename: "{app}\{#AppExeCE}"; Tasks: deskicons\user
Name: "{userdesktop}\{#AppName} {#AppNameHH}"; Filename: "{app}\{#AppExeHH}"; Tasks: deskicons\user

[Run]
Filename: "{dotnet40}\InstallUtil.exe"; Parameters: """{app}\yamdccsvc.exe"""; StatusMsg: "Installing YAMDCC service..."; Check: ShouldInstallService; Flags: logoutput runhidden
Filename: "{sys}\net.exe"; Parameters: "start yamdccsvc"; StatusMsg: "Starting YAMDCC service..."; Flags: logoutput runhidden
; Run YAMDCC updater to show "YAMDCC has been updated successfully" message
; if run silently, otherwise run Config Editor (if selected during setup)
Filename: "{app}\Updater.exe"; Parameters: "--updated"; Flags: postinstall runascurrentuser skipifnotsilent
Filename: "{app}\{#AppExeCE}"; Description: "{cm:LaunchCE}"; Flags: nowait postinstall runascurrentuser skipifsilent

; Stop and uninstall YAMDCC service before deleting program files
; TODO: better YAMDCC service stop/uninstall
[UninstallRun]
Filename: "{sys}\net.exe"; Parameters: "stop yamdccsvc"; RunOnceId: "StopSvc"; Flags: logoutput runhidden
Filename: "{dotnet40}\InstallUtil.exe"; Parameters: "/u ""{app}\yamdccsvc.exe"""; RunOnceId: "DelSvc"; Flags: logoutput runhidden

[Code]
const
  SC_MANAGER_CONNECT = $0001;

  SERVICE_CONTROL_STOP = $00000001;

  SERVICE_STOPPED = $00000001;
  SERVICE_START_PENDING = $00000002;
  SERVICE_STOP_PENDING = $00000003;
  SERVICE_RUNNING = $00000004;
  SERVICE_CONTINUE_PENDING = $00000005;
  SERVICE_PAUSE_PENDING = $00000006;
  SERVICE_PAUSED = $00000007;

  ERROR_SERVICE_DOES_NOT_EXIST = $0424;

type
  TSCHandle = THandle;

  TServiceStatus = record
    dwServiceType: DWORD;
    dwCurrentState: DWORD;
    dwControlsAccepted: DWORD;
    dwWin32ExitCode: DWORD;
    dwServiceSpecificExitCode: DWORD;
    dwCheckPoint: DWORD;
    dwWaitHint: DWORD;
  end;

var
  ServiceInstalled: Boolean;

function OpenService(hSCManager: TSCHandle; lpServiceName: string; dwDesiredAccess: DWORD): TSCHandle;
  external 'OpenService{#AW}@advapi32.dll stdcall';
function OpenSCManager(lpMachineName: string; lpDatabaseName: string; dwDesiredAccess: DWORD): TSCHandle;
  external 'OpenSCManager{#AW}@advapi32.dll stdcall';
function QueryServiceStatus(hService: TSCHandle; out lpServiceStatus: TServiceStatus): BOOL;
  external 'QueryServiceStatus@advapi32.dll stdcall';
function CloseServiceHandle(hSCObject: TSCHandle): BOOL;
  external 'CloseServiceHandle@advapi32.dll stdcall';
function ControlService(hService: TSCHandle; dwControl: DWORD; out lpServiceStatus: TServiceStatus): BOOL;
  external 'ControlService@advapi32.dll stdcall';

function GetWin32ErrorMsg: String;
begin
  Result := SysErrorMessage(DllGetLastError()) + ' (' + IntToStr(DllGetLastError()) + ')';
end;

function ShouldInstallService: Boolean;
begin
  Result := not ServiceInstalled
end;

function GetServiceImagePath(const ServiceName: String): String;
  var
    ImagePath: String;
begin
  if RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SYSTEM\CurrentControlSet\Services\' + ServiceName, 'ImagePath', ImagePath) then
    Result := ImagePath
  else
    Result := '';
end;

// Based on: https://stackoverflow.com/q/70208868
function NormaliseFilePath(Path: String): String;
begin
  // Normalise the provided file path. The below code:
  // - Removes surrounding quotes from a path,
  // - Normalises a sequence of any slashes to one backslash (e.g. '//' -> '\'), except for leading backslashes in UNC paths (e.g. '\\.\'),
  // - Resolves relative paths,
  // - Removes the trailing backslash (if any),
  // - Returns the normalised path.
  Result := RemoveBackslashUnlessRoot(ExpandFileName(RemoveQuotes(Path)));
end;

function InitializeSetup: Boolean;
begin
  // make sure .NET Framework 4.8 or later is installed
  // (it should be by default on Windows 10 and 11, but I KNOW someone will try to run this on Windows 7...)
  Result := IsDotNetInstalled(net48, 0)
  if not Result then
    SuppressibleMsgBox(FmtMessage(SetupMessage(msgWinVersionTooLowError), ['.NET Framework', '4.8']), mbCriticalError, MB_OK, IDOK);
end;

// Partially based on: https://stackoverflow.com/a/32476546
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  hSCM: TSCHandle;
  hService: TSCHandle;
  ServiceStatus: TServiceStatus;
  ExpectedPath: String;
  ActualPath: String;
begin
  ServiceInstalled := False;
  Log('-- YAMDCC service checks --');
  Log('Checking if YAMDCC service is already installed...');
  hSCM := OpenSCManager('', '', SC_MANAGER_CONNECT);
  if hSCM <> 0 then
  begin
    hService := OpenService(hSCM, 'yamdccsvc', $0024);  //SERVICE_QUERY_STATUS | SERVICE_STOP
    if hService <> 0 then
    begin
      ServiceInstalled := True;
      if QueryServiceStatus(hService, ServiceStatus) then
      begin
        Log('YAMDCC service is already installed. Making sure it''s in the right location...');
        ExpectedPath := NormaliseFilePath(ExpandConstant('{app}') + '\{#AppExeSvc}');
        ActualPath := NormaliseFilePath(GetServiceImagePath('yamdccsvc'));
        if SameText(ExpectedPath, ActualPath) then
          Log('YAMDCC service paths match!')
        else
        begin
          Log('YAMDCC service paths do NOT match!');
          Log('Expected YAMDCC service location: ' + ExpectedPath);
          Log('Actual YAMDCC service location: ' + ActualPath);
          Result := 'The YAMDCC service appears to be already be installed somewhere else. ' +
            'Make sure you uninstalled all previous versions of YAMDCC, then try installing again.' + #10#10 +
            'Expected YAMDCC service location: ' + ExpectedPath + #10
            'Actual YAMDCC service location: ' + ActualPath;
        end;
        Log('Querying service status...');
        if ServiceStatus.dwCurrentState = SERVICE_STOPPED then
          Log('YAMDCC service is already stopped.')
        else
        begin
          Log('YAMDCC service is running! Stopping it...');
          if ControlService(hService, SERVICE_CONTROL_STOP, ServiceStatus) then
            Log('YAMDCC service stop requested successfully.')
          else
            Result := 'Failed to stop YAMDCC service: ' + GetWin32ErrorMsg() + #10#10 +
             'Please try stopping it manually with `net stop yamdccsvc` before proceeding with the installation.';
        end;
      end
      else
        Result := 'Failed to query status of YAMDCC service: ' + GetWin32ErrorMsg();
      CloseServiceHandle(hService);
    end
    else
    begin
      if DllGetLastError() = ERROR_SERVICE_DOES_NOT_EXIST then
      begin
        Log('YAMDCC service is not installed.');
      end
      else
        Result := 'Failed to connect to YAMDCC service: ' + GetWin32ErrorMsg();
    CloseServiceHandle(hSCM);
    end;
  end
  else
    Result := 'Failed to connect to the Service Control Manager to check if the YAMDCC service is installed: ' + GetWin32ErrorMsg();
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  AppDataDir: String;
begin
  if CurUninstallStep = usPostUninstall then
  begin
    AppDataDir := ExpandConstant('{commonappdata}\Sparronator9999\YAMDCC')
    if DirExists(AppDataDir) and (SuppressibleMsgBox('Delete the YAMDCC data directory (located at `' + AppDataDir + '`)?'#10#10
      'Click "Yes" if you don''t intend to re-install YAMDCC.', mbConfirmation, MB_YESNO, IDNO) = IDYES) then
      DelTree(AppDataDir, True, True, True);
  end;
end;
