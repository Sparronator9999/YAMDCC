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

using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace YAMDCC.Service
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            ServiceInstaller svcInstaller = new()
            {
                Description = Strings.GetString("svcDesc"),
                DisplayName = "YAMDCC Service",
                ServiceName = "yamdccsvc",
                StartType = ServiceStartMode.Automatic
            };

            ServiceProcessInstaller svcPInstaller = new()
            {
                Account = ServiceAccount.LocalSystem
            };

            Installers.AddRange(
            [
                svcPInstaller,
                svcInstaller
            ]);
        }
    }
}
