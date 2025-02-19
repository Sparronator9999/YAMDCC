// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2023-2025.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Dialogs;

namespace YAMDCC.ConfigEditor;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

        if (!Utils.IsAdmin())
        {
            Utils.ShowError(Strings.GetString("dlgNoAdmin"));
            return;
        }

        // multi-instance detection
        // NOTE: GUID is used to prevent conflicts with potential
        // identically named but different program
        // based on: https://stackoverflow.com/a/184143
        using (Mutex mutex = new(true, "{10572c4f-894e-4837-b31c-356d70c44e19}", out bool createdNew))
        {
            // this instance is the first to open; proceed as normal:
            if (createdNew)
            {
                // Make sure the application data directory structure is set up
                // because apparently windows services don't know how to create
                // directories:
                Directory.CreateDirectory(Paths.Logs);

                if (!Utils.ServiceExists("yamdccsvc"))
                {
                    if (File.Exists("yamdccsvc.exe"))
                    {
                        if (Utils.ShowInfo(
                            Strings.GetString("dlgSvcNotInstalled"), "Service not installed",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ProgressDialog<bool> dlg = new()
                            {
                                Caption = Strings.GetString("dlgSvcInstalling"),
                                DoWork = () =>
                                {
                                    if (Utils.InstallService("yamdccsvc"))
                                    {
                                        if (Utils.StartService("yamdccsvc"))
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            Utils.ShowError(Strings.GetString("dlgSvcStartCrash"));
                                        }
                                    }
                                    else
                                    {
                                        Utils.ShowError(Strings.GetString("dlgSvcInstallFail"));
                                    }
                                    return false;
                                }
                            };
                            dlg.ShowDialog();

                            if (dlg.Result)
                            {
                                // Start the program when the service finishes starting:
                                Start();
                            }
                        }
                        return;
                    }
                    else
                    {
                        Utils.ShowError(Strings.GetString("dlgSvcNotFound"));
                        return;
                    }
                }

                // Check if the service is stopped:
                using (ServiceController yamdccSvc = new("yamdccsvc"))
                {
                    ServiceControllerStatus status = yamdccSvc.Status;
                    if (status == ServiceControllerStatus.Stopped)
                    {
                        if (Utils.ShowInfo(
                            Strings.GetString("dlgSvcNotRunning"), "Service not running",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ProgressDialog<bool> dlg = new()
                            {
                                Caption = Strings.GetString("dlgSvcStarting"),
                                DoWork = () =>
                                {
                                    if (Utils.StartService("yamdccsvc"))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        Utils.ShowError(Strings.GetString("dlgSvcStartCrash"));
                                        return true;
                                    }
                                }
                            };
                            dlg.ShowDialog();

                            if (dlg.Result)
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                // Start the program when the service finishes starting:
                Start();
            }
            else
            {
                // YAMDCC is already running, focus
                // (and restore, if minimised) its window:
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    if (process.Id != current.Id)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            ShowWindow(process.MainWindowHandle, 9);    // SW_RESTORE
                            SetForegroundWindow(process.MainWindowHandle);
                        }
                        break;
                    }
                }
            }
        }
    }

    private static void Start()
    {
        if (CommonConfig.GetECtoConfState() == ECtoConfState.PendingReboot)
        {
            if (Utils.ShowWarning(Strings.GetString("dlgECtoConfReboot"),
                "Reboot pending", MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                CommonConfig.SetECtoConfState(ECtoConfState.None);
            }
            else
            {
                return;
            }
        }

        Application.Run(new MainForm());
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private static void ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        new CrashDialog(e.Exception).ShowDialog();
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        new CrashDialog((Exception)e.ExceptionObject).ShowDialog();
    }
}
