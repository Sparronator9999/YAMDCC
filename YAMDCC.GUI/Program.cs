// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
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

using YAMDCC.GUI.Dialogs;
using System;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace YAMDCC.GUI
{
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

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Make sure the application data directory structure is set up
            // because apparently windows services don't know how to create
            // directories:
            Directory.CreateDirectory(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Sparronator9999", "YAMDCC", "Logs"));

            if (Utils.ServiceExists("yamdccsvc"))
            {
                ServiceController yamdccSvc = new ServiceController("yamdccsvc");

                // Check if the service is stopped:
                try
                {
                    if (yamdccSvc.Status == ServiceControllerStatus.Stopped)
                    {
                        if (MessageBox.Show(
                            Strings.GetString("svcNotRunning"),
                            "Service not running", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            if (!Utils.StartService("yamdccsvc"))
                            {
                                MessageBox.Show(Strings.GetString("svcErrorCrash"),
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                        else return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        string.Format(Strings.GetString("svcErrorStart"), ex),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    yamdccSvc.Close();
                }
            }
            else // Service doesn't exist
            {
                if (File.Exists("yamdccsvc.exe"))
                {
                    if (MessageBox.Show(
                        Strings.GetString("svcNotInstalled"),
                        "Service not installed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        if (Utils.InstallService("yamdccsvc"))
                        {
                            MessageBox.Show(Strings.GetString("svcInstallSuccess"),
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(Strings.GetString("svcInstallFail"),
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(Strings.GetString("svcNotFound"),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // Start the program when the service finishes starting:
            Application.Run(new MainWindow());
        }

        #region Global exception handlers
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                CrashDialog dlg = new CrashDialog(ex, false);
                dlg.ShowDialog();
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            CrashDialog dlg = new CrashDialog(e.Exception, true);
            dlg.ShowDialog();
        }
        #endregion
    }
}
