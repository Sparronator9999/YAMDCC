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

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows.Forms;

namespace YAMDCC.Common
{
    /// <summary>
    /// A collection of miscellaneous useful utilities
    /// </summary>
    public static class Utils
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
        public static DialogResult ShowError(string message)
        {
            return MessageBox.Show(message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string GetVerString()
        {
            // format: X.Y.Z-SUFFIX[.W]+REVISION,
            // where W is a beta/release candidate version if applicable
            string prodVer = Application.ProductVersion;

            string suffix;
            if (prodVer.Contains("-"))
            {
                // remove the version number (SUFFIX[.W]+REVISION at this point):
                suffix = prodVer.Remove(0, prodVer.IndexOf('-') + 1);

                // remove Git hash, if it exists (for "dev" detection)
                if (suffix.Contains("+"))
                {
                    suffix = suffix.Remove(suffix.IndexOf('+'));
                }
            }
            else
            {
                // suffix probably doesn't exist...
                suffix = string.Empty;
            }

            switch (suffix.ToLowerInvariant())
            {
                case "release":
                    // only show the version number (e.g. X.Y.Z):
                    return prodVer.Remove(prodVer.IndexOf('-'));
                case "dev":
                    return prodVer.Contains("+")
                        // probably a snapshot release (e.g. X.Y.Z-SNAPSHOT+REVISION);
                        // show shortened Git commit hash if it exists:
                        ? prodVer.Remove(prodVer.IndexOf('+') + 8)
                        // Return the product version if not in expected format
                        : prodVer;
                default:    // beta, RC, etc.
                    return prodVer.Contains(".") && prodVer.Contains("+")
                        // Beta releases should be in format X.Y.Z-beta.W+REVISION.
                        // Remove the revision (i.e. only show X.Y.Z-beta.W):
                        ? prodVer.Remove(prodVer.IndexOf('+'))
                        // Return the product version if not in expected format
                        : prodVer;
            }
        }

        /// <summary>
        /// Gets the Git revision of this program, if available.
        /// </summary>
        /// <returns>
        /// The Git hash of the program version if available,
        /// otherwise <see cref="string.Empty"/>.
        /// </returns>
        public static string GetRevision()
        {
            string prodVer = Application.ProductVersion;

            return prodVer.Contains("+")
                ? prodVer.Remove(0, prodVer.IndexOf('+') + 1)
                : string.Empty;
        }

        public static bool IsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            try
            {
                WindowsPrincipal principal = new(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch
            {
                return false;
            }
            finally
            {
                identity.Dispose();
            }
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
                    UseShellExecute = true,
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
