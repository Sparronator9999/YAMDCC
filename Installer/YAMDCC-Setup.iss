; Defined when compiled via GitHub Actions. Uncomment one of the below when compiling locally.
;#define BuildConfig "Debug"
;#define BuildConfig "Release"

; Define constants used in other sections of the installer.
#define AppName "YAMDCC"
#define AppNameCE "Config Editor"
#define AppNameHH "Hotkey Handler"
#define AppVer "1.2.0"
#define AppVerFriendly "1.2"
#define AppPublisher "Sparronator9999"
#define AppURL "https://github.com/Sparronator9999/YAMDCC"
#define AppExeCE "ConfigEditor.exe"
#define AppExeHH "HotkeyHandler.exe"
#define AppExeSvc "yamdccsvc.exe"
#define AppExeUpdater "Updater.exe"

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
AppVersion={#AppVerFriendly} ({#BuildConfig})
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}/issues
AppUpdatesURL={#AppURL}/releases
ArchitecturesAllowed=x86compatible or x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
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
Uninstallable=not IsPortableMode
UninstallDisplayIcon={app}\{#AppExeCE}
WizardStyle=modern
WizardImageFile=Installer\setup.bmp
WizardSmallImageFile=Installer\setup-small.bmp

[CustomMessages]
english.StartMenu=Start menu:
english.StartIcons=Create Start menu shortcuts
english.DeskIcons=Create desktop icons
english.DeskIconsCommon=For all users
english.DeskIconsUser=For the current user only
english.LaunchCE=Launch config editor

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Components]
Name: "main"; Description: "YAMDCC Service and common libraries (required)"; Types: full compact custom; Flags: fixed
Name: "cli"; Description: "CLI"; types: full
Name: "confeditor"; Description: "Config Editor"; Types: full compact
Name: "ecinspect"; Description: "EC Inspector"; Types: full
Name: "hkhandler"; Description: "Hotkey Handler"; Types: full
Name: "updater"; Description: "Updater"; Types: full compact

[Tasks]
Name: "starticons"; Description: "{cm:StartIcons}"; GroupDescription: "{cm:StartMenu}"
Name: "deskicons"; Description: "{cm:DeskIcons}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "deskicons\common"; Description: "{cm:DeskIconsCommon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: exclusive unchecked
Name: "deskicons\user"; Description: "{cm:DeskIconsUser}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: exclusive unchecked

[Files]
Source: "YAMDCC.Service\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: main
Source: "YAMDCC.CLI\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: cli
Source: "YAMDCC.ConfigEditor\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: confeditor
Source: "YAMDCC.ECInspector\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: ecinspect
Source: "YAMDCC.HotkeyHandler\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: hkhandler
Source: "YAMDCC.Updater\bin\{#BuildConfig}\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Components: updater; Check: Not IsPortableMode

[Icons]
Name: "{autoprograms}\{#AppName}\{#AppNameCE}"; Filename: "{app}\{#AppExeCE}"; Tasks: starticons; Check: not IsPortableMode
Name: "{autoprograms}\{#AppName}\{#AppNameHH}"; Filename: "{app}\{#AppExeHH}"; Tasks: starticons; Check: not IsPortableMode
Name: "{commondesktop}\{#AppName} {#AppNameCE}"; Filename: "{app}\{#AppExeCE}"; Tasks: deskicons\common
Name: "{commondesktop}\{#AppName} {#AppNameHH}"; Filename: "{app}\{#AppExeHH}"; Tasks: deskicons\common
Name: "{userdesktop}\{#AppName} {#AppNameCE}"; Filename: "{app}\{#AppExeCE}"; Tasks: deskicons\user
Name: "{userdesktop}\{#AppName} {#AppNameHH}"; Filename: "{app}\{#AppExeHH}"; Tasks: deskicons\user

[Run]
Filename: "{dotnet40}\InstallUtil.exe"; Parameters: """{app}\yamdccsvc.exe"""; StatusMsg: "Installing YAMDCC service..."; Check: ShouldInstallService; Flags: logoutput runhidden
Filename: "{sys}\net.exe"; Parameters: "start yamdccsvc"; StatusMsg: "Starting YAMDCC service..."; Check: not IsPortableMode; Flags: logoutput runhidden
; Run YAMDCC updater to show "YAMDCC has been updated successfully" message
; if run silently, otherwise run Config Editor (if selected during setup)
Filename: "{app}\{#AppExeUpdater}"; Parameters: "--updated"; Flags: postinstall skipifnotsilent; Components: updater
Filename: "{app}\{#AppExeCE}"; Description: "{cm:LaunchCE}"; Flags: nowait postinstall runascurrentuser skipifsilent; Components: confeditor

; Stop and uninstall YAMDCC service before deleting program files
; TODO: better YAMDCC service stop/uninstall
[UninstallRun]
Filename: "{app}\{#AppExeUpdater}"; Parameters: "--setautoupdate false"; RunOnceId: "NoAutoUpdate"; Components: updater
Filename: "{sys}\net.exe"; Parameters: "stop yamdccsvc"; RunOnceId: "StopSvc"; Flags: logoutput runhidden
Filename: "{dotnet40}\InstallUtil.exe"; Parameters: "/u ""{app}\yamdccsvc.exe"""; RunOnceId: "DelSvc"; Flags: logoutput runhidden

; Remove logs left behind by running InstallUtil while uninstalling the YAMDCC service.
[UninstallDelete]
Type: files; Name: "{app}\InstallUtil.InstallLog"
Type: files; Name: "{app}\yamdccsvc.InstallLog"

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
  SetupTypePage: TInputOptionWizardPage;
  PrepareToInstallProgressPage: TOutputProgressWizardPage;
  PathWarningPage: TOutputMsgWizardPage;

function OpenService(hSCManager: THandle; lpServiceName: string; dwDesiredAccess: DWORD): THandle;
  external 'OpenService{#AW}@advapi32.dll stdcall';
function OpenSCManager(lpMachineName: string; lpDatabaseName: string; dwDesiredAccess: DWORD): THandle;
  external 'OpenSCManager{#AW}@advapi32.dll stdcall';
function QueryServiceStatus(hService: THandle; out lpServiceStatus: TServiceStatus): BOOL;
  external 'QueryServiceStatus@advapi32.dll stdcall';
function CloseServiceHandle(hSCObject: THandle): BOOL;
  external 'CloseServiceHandle@advapi32.dll stdcall';
function ControlService(hService: THandle; dwControl: DWORD; out lpServiceStatus: TServiceStatus): BOOL;
  external 'ControlService@advapi32.dll stdcall';

function GetWin32ErrorMsg: String;
begin
  Result := SysErrorMessage(DllGetLastError()) + ' (' + IntToStr(DllGetLastError()) + ')';
end;

function IsPortableMode: Boolean;
begin
  Result := SetupTypePage.SelectedValueIndex = 1;
end;

function ShouldInstallService: Boolean;
begin
  if IsPortableMode then
    Result := False
  else
    Result := not ServiceInstalled;
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

procedure InitializeWizard;
var
  S: String;
begin
  // https://stackoverflow.com/a/25811746
  SetupTypePage := CreateInputOptionPage(wpLicense, 'Setup type', 'How to install YAMDCC', 'Should YAMDCC be installed like any other app, or just extracted to a selected directory?', True, False);
  SetupTypePage.Add('Standard install (select this option if unsure)');
  SetupTypePage.Add('Portable mode (extract files only)');
  SetupTypePage.Values[0] := True;
  SetupTypePage.Values[1] := False;

  S := SetupMessage(msgPreparingDesc);
  StringChange(S, '[name]', '{#AppName}')
  PrepareToInstallProgressPage := CreateOutputProgressPage(SetupMessage(msgWizardPreparing), S);

  // Create a custom wizard page to be shown if the CLI is going to be installed
  PathWarningPage := CreateOutputMsgPage(wpSelectComponents, 'Important', 'CLI is not added to PATH (yet)',
    'The YAMDCC CLI is not currently added to PATH on install (meaning you can''t just run it from the Command Prompt without first cd-ing into the program directory).' + #10#10 +
    'You will have to add it manually using the Advanced System Settings dialog.');
end;

function ShouldSkipPage(PageID: Integer): Boolean;
begin
  if PageID = PathWarningPage.ID then
  begin
    if not WizardIsComponentSelected('cli') then
      Result := True;
  end
  else
  begin
    if IsPortableMode then
    begin
      case PageID of
        wpSelectComponents, wpSelectProgramGroup, wpSelectTasks, wpReady:
          Result := True;
      else
        Result := False;
      end;
    end
    else
      Result := False;
  end;
end;

// Partially based on: https://stackoverflow.com/a/32476546
function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  hSCM: THandle;
  hService: THandle;
  ServiceStatus: TServiceStatus;
  ExpectedPath: String;
  ActualPath: String;
  i: Integer;
begin
  ServiceInstalled := False;

  // Don't do any YAMDCC service checks if running in portable mode
  if IsPortableMode() then
    Exit;

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
        begin
          Log('YAMDCC service paths match! Querying service status...');
          if ServiceStatus.dwCurrentState = SERVICE_STOPPED then
            Log('YAMDCC service is already stopped.')
          else
          begin
            Log('YAMDCC service is running! Stopping it...');
            if ControlService(hService, SERVICE_CONTROL_STOP, ServiceStatus) then
            begin
              Log('YAMDCC service stop requested successfully. Waiting for it to stop...');
              PrepareToInstallProgressPage.SetProgress(0, 10);
              PrepareToInstallProgressPage.SetText('Waiting for the YAMDCC service to stop.', 'This shouldn''t take long, but may take up to 10 seconds if the service is unresponsive.');
              PrepareToInstallProgressPage.Show();
              try
                // wait up to 10 seconds (20 half-seconds) for the YAMDCC service to stop
                for i := 1 to 20 do
                begin
                  PrepareToInstallProgressPage.SetProgress(i - 1, 10);
                  Sleep(500);
                  if QueryServiceStatus(hService, ServiceStatus) then
                  begin
                    if ServiceStatus.dwCurrentState = SERVICE_STOPPED then
                    begin
                      Log('Service stopped successfully!');
                      break;
                    end
                    else if ServiceStatus.dwCurrentState <> SERVICE_STOP_PENDING then
                    begin
                      Log('Unexpected service state! (' + IntToStr(ServiceStatus.dwCurrentState) + ')');
                      i := 21;
                      break;
                    end;
                  end
                  else
                  begin
                    Log('Service status query failed!');
                    Result := 'Failed to query status of YAMDCC service: ' + GetWin32ErrorMsg() + #10#10 +
                      'Please try installing again. If you get this error again, this is probably a bug, and should be reported to: ' + #10#10 +
                      'https://github.com/Sparronator9999/YAMDCC/issues';
                    break;
                  end;
                end;
                if i = 211 then
                begin
                  Log('Failed to stop YAMDCC service (timed out/unexpected state)!');
                  Result := 'Failed to stop the YAMDCC service, either due to a timeout or unexpected service state.' + #10#10 +
                    'This is probably a bug. Please report it, along with service logs (if any) at:' + #10#10 +
                    'https://github.com/Sparronator9999/YAMDCC/issues' + #10#10 +
                    'Service status code: ' + IntToStr(ServiceStatus.dwCurrentState);
                end;
              finally
                PrepareToInstallProgressPage.Hide();
              end;
            end
            else
            begin
              Log('Failed to send service stop request!');
              Result := 'Failed to stop YAMDCC service: ' + GetWin32ErrorMsg() + #10#10 +
                'Please try stopping it manually with `net stop yamdccsvc` before proceeding with the installation.';
            end;
          end;
        end
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
      end
      else
      begin
        Log('Service status query failed!');
        Result := 'Failed to query status of YAMDCC service: ' + #10#10 +
          'Please try installing again. If you get this error again, this is probably a bug, and should be reported to: ' + #10#10 +
          'https://github.com/Sparronator9999/YAMDCC/issues';
      end;
      CloseServiceHandle(hService);
    end
    else
    begin
      if DllGetLastError() = ERROR_SERVICE_DOES_NOT_EXIST then
      begin
        Log('YAMDCC service is not installed.');
      end
      else
        Result := 'Failed to connect to YAMDCC service: ' + GetWin32ErrorMsg() + #10#10 +
          'Please try installing again. If you get this error again, this is probably a bug, and should be reported to: ' + #10#10 +
          'https://github.com/Sparronator9999/YAMDCC/issues';
    CloseServiceHandle(hSCM);
    end;
  end
  else
    Result := 'Failed to connect to the Service Control Manager to check if the YAMDCC service is installed: ' + GetWin32ErrorMsg() + #10#10 +
      'Please try installing again. If you get this error again, this is probably a bug, and should be reported to: ' + #10#10 +
      'https://github.com/Sparronator9999/YAMDCC/issues';
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  AppDataDir: String;
begin
  // Ask to delete the YAMDCC data directory on successful uninstall.
  if CurUninstallStep = usPostUninstall then
  begin
    AppDataDir := ExpandConstant('{commonappdata}\Sparronator9999\YAMDCC')
    if DirExists(AppDataDir) and (SuppressibleMsgBox('Keep the YAMDCC data directory (located at `' + AppDataDir + '`)?' + #10#10
      'Click "No" if you don''t intend to re-install YAMDCC.', mbConfirmation, MB_YESNO, IDYES) = IDNO) then
    begin
      Log('Deleting YAMDCC data directory...');
      DelTree(AppDataDir, True, True, True);
    end
    else
      Log('Leaving YAMDCC data directory untouched.');
  end;
end;
