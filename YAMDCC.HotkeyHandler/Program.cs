// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2025.
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
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Dialogs;
using YAMDCC.HotkeyHandler.Win32;

namespace YAMDCC.HotkeyHandler;

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
            Utils.ShowError(
                "If you see this message, Hotkey Handler is not running as an Administrator.\n\n" +
                "Please re-run this program as Administrator\n" +
                "(by right-clicking on this program and clicking \"Run as administrator\").");
            return;
        }

        // multi-instance detection
        // NOTE: GUID is used to prevent conflicts with potential
        // identically named but different program
        // based on: https://stackoverflow.com/a/184143
        using (Mutex mutex = new(true, "{ff629e0f-d8d7-4e86-aeff-fb4192622440}", out bool createdNew))
        {
            // this instance is the first to open; proceed as normal:
            if (createdNew)
            {
                // check that the YAMDCC service is installed and running
                using (ServiceController yamdccSvc = new("yamdccsvc"))
                {
                    try
                    {
                        if (yamdccSvc.Status == ServiceControllerStatus.Stopped)
                        {
                            Utils.ShowError(
                                "The YAMDCC service is currently not running.\n" +
                                "Please run the YAMDCC Config Editor to start it.");
                            return;
                        }
                    }
                    catch
                    {
                        Utils.ShowError(
                            "The YAMDCC service is not installed.\n" +
                            "Please run the YAMDCC Config Editor to install it.");
                        return;
                    }
                }
                Application.Run(new MainForm());
            }
            else
            {
                // Hotkey Handler is already running, focus
                // (and restore, if minimised) its window:
                Process current = Process.GetCurrentProcess();
                foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                {
                    // TODO:
                    // fix already existing Hotkey Handler
                    // instance not being restored
                    if (process.Id != current.Id)
                    {
                        if (process.MainWindowHandle != IntPtr.Zero)
                        {
                            User32.ShowWindow(process.MainWindowHandle, 9);    // SW_RESTORE
                            User32.SetForegroundWindow(process.MainWindowHandle);
                        }
                        break;
                    }
                }
            }
        }
    }

    private static void ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        new CrashDialog(e.Exception).ShowDialog();
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        new CrashDialog((Exception)e.ExceptionObject).ShowDialog();
    }
}
