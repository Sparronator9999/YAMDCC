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

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Windows.Forms;

namespace YAMDCC.GUI
{
    /// <summary>
    /// A collection of miscellaneous useful utilities
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Shows an error dialog.
        /// </summary>
        /// <param name="message">
        /// The message to show in the error dialog.
        /// </param>
        /// <returns>
        /// One of the <see cref="DialogResult"/> values.
        /// </returns>
        internal static DialogResult ShowError(string message)
        {
            return MessageBox.Show(message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Installs the specified .NET Framework
        /// service to the local computer.
        /// </summary>
        /// <remarks>
        /// The service is not started automatically. Use
        /// <see cref="StartService(string)"/> to start it if needed.
        /// </remarks>
        /// <param name="svcExe">
        /// The path to the service executable.
        /// </param>
        /// <returns>
        /// <c>true</c> if the service installation
        /// was successful, otherwise <c>false</c>.
        /// </returns>
        public static bool InstallService(string svcExe)
        {
            string runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
            int exitCode = RunCmd($"{runtimePath}\\installutil.exe", $"{svcExe}.exe");
            DeleteInstallUtilLogs();
            return exitCode == 0;
        }

        /// <summary>
        /// Uninstalls the specified .NET Framework
        /// service from the local computer.
        /// </summary>
        /// <param name="svcExe">
        /// The path to the service executable.
        /// </param>
        /// <returns>
        /// <c>true</c> if the service uninstallation
        /// was successful, otherwise <c>false</c>.
        /// </returns>
        public static bool UninstallService(string svcExe)
        {
            string runtimePath = RuntimeEnvironment.GetRuntimeDirectory();
            int exitCode = RunCmd($"{runtimePath}\\installutil.exe", $"/u {svcExe}.exe");
            DeleteInstallUtilLogs();
            return exitCode == 0;
        }

        /// <summary>
        /// Starts the specified service.
        /// </summary>
        /// <param name="svcName">
        /// The service name, as shown in <c>services.msc</c>
        /// (NOT to be confused with its display name).
        /// </param>
        /// <returns>
        /// <c>true</c> if the service started
        /// successfully, otherwise <c>false</c>.
        /// </returns>
        public static bool StartService(string svcName)
        {
            return RunCmd("net.exe", $"start {svcName}") == 0;
        }

        /// <summary>
        /// Stops the specified service.
        /// </summary>
        /// <param name="svcName">
        /// The service name, as shown in <c>services.msc</c>
        /// (NOT to be confused with its display name).
        /// </param>
        /// <returns>
        /// <c>true</c> if the service was stopped
        /// successfully, otherwise <c>false</c>.
        /// </returns>
        public static bool StopService(string svcName)
        {
            return RunCmd("net.exe", $"stop {svcName}") == 0;
        }

        /// <summary>
        /// Checks to see if the specified service
        /// is installed on the computer.
        /// </summary>
        /// <param name="svcName">
        /// The service name, as shown in <c>services.msc</c>
        /// (NOT to be confused with its display name).
        /// </param>
        /// <returns>
        /// <c>true</c> if the service was
        /// found, otherwise <c>false</c>.
        /// </returns>
        public static bool ServiceExists(string svcName)
        {
            return ServiceController.GetServices().Any(s => s.ServiceName == svcName);
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

        private static int RunCmd(string exe, string args)
        {
            Process p = new()
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
