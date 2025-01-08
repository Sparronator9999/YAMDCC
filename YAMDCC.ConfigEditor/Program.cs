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
using System.Globalization;
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
    private const int SW_RESTORE = 9;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        #region Global exception handlers
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += static (sender, e) =>
            new CrashDialog(e.Exception).ShowDialog();

        AppDomain.CurrentDomain.UnhandledException += static (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                new CrashDialog(ex).ShowDialog();
            }
        };
        #endregion

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
                            ProgressDialog dlg = new(Strings.GetString("dlgSvcInstalling"), (e) =>
                            {
                                e.Result = false;
                                if (Utils.InstallService("yamdccsvc"))
                                {
                                    if (Utils.StartService("yamdccsvc"))
                                    {
                                        e.Result = true;
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
                            });
                            dlg.ShowDialog();

                            if (dlg.Result is bool b && b)
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
                ServiceController yamdccSvc = new("yamdccsvc");
                try
                {
                    ServiceControllerStatus status = yamdccSvc.Status;
                    if (status == ServiceControllerStatus.Stopped)
                    {
                        if (Utils.ShowInfo(
                            Strings.GetString("dlgSvcStopped"), "Service not running",
                            MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            ProgressDialog dlg = new(Strings.GetString("dlgSvcStarting"), (e) =>
                            {
                                if (Utils.StartService("yamdccsvc"))
                                {
                                    e.Result = false;
                                }
                                else
                                {
                                    Utils.ShowError(Strings.GetString("dlgSvcStartCrash"));
                                    e.Result = true;
                                }
                            });
                            dlg.ShowDialog();

                            if (dlg.Result is bool b && b)
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
                finally
                {
                    yamdccSvc?.Close();
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
                            ShowWindow(process.MainWindowHandle, SW_RESTORE);
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
        int rebootFlag = -1;
        try
        {
            StreamReader sr = new(Paths.ECToConfPending);
            if (int.TryParse(sr.ReadToEnd(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                rebootFlag = value;
            }
            sr.Close();
        }
        catch (FileNotFoundException) { }
        catch (DirectoryNotFoundException) { }

        if (rebootFlag == 1)
        {
            if (Utils.ShowWarning(Strings.GetString("dlgECtoConfReboot"),
                "Reboot pending", MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                try
                {
                    File.Delete(Paths.ECToConfPending);
                }
                catch (DirectoryNotFoundException) { }
            }
            else
            {
                return;
            }
        }

        Application.Run(new MainWindow());
    }

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}
