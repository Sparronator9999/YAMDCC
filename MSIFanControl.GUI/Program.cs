// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;

namespace MSIFanControl.GUI
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

            if (ServiceExists("msifcsvc"))
            {
                ServiceController msifcSvc = new ServiceController("msifcsvc");

                // Check if the service is stopped:
                try
                {
                    if (msifcSvc.Status == ServiceControllerStatus.Stopped)
                    {
                        if (MessageBox.Show(
                            Strings.GetString("svcNotRunning"),
                            "Service not running", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information) == DialogResult.Yes)
                        {
                            if (RunCmd("net", "start msifcsvc") != 0)
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
                    msifcSvc.Close();
                }
            }
            else // Service doesn't exist
            {
                if (File.Exists("msifcsvc.exe"))
                {
                    if (MessageBox.Show(
                        Strings.GetString("svcNotInstalled"),
                        "Service not installed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        if (InstallService("msifcsvc"))
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

        private static bool ServiceExists(string svcName) =>
            ServiceController.GetServices().Any(s => s.ServiceName == svcName);

        private static bool InstallService(string exeName)
        {
            string runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
            int exitCode = RunCmd($"{runtimePath}\\installutil.exe", $"{exeName}.exe");
            DeleteInstallUtilLogs();
            return exitCode == 0;
        }

        private static void DeleteInstallUtilLogs()
        {
            foreach (string file in Directory.GetFiles(".", "*.InstallLog", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    File.Delete(file);
                }
                catch (DirectoryNotFoundException) { }
            }
        }

        /// <summary>
        /// Runs a command, waiting for it to exit.
        /// </summary>
        /// <param name="exe">The executable to run.</param>
        /// <param name="args">The arguments to pass to the executable.</param>
        /// <returns>The exit code generated by the called executable.</returns>
        private static int RunCmd(string exe, string args)
        {
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo(exe)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    Arguments = args,
                },
            };

            p.Start();
            p.WaitForExit();

            return p.ExitCode;
        }
    }
}
