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

using YAMDCC.Logs;
using System;
using System.Resources;
using System.ServiceProcess;

namespace YAMDCC.Service
{
    internal static class Program
    {
        private static readonly Logger Log = new Logger
        {
            ConsoleLogLevel = LogLevel.None,
            FileLogLevel = LogLevel.Debug,
        };

        private static readonly ResourceManager Res = new ResourceManager(typeof(Program));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;

            ServiceBase.Run(new ServiceBase[] { new svcFanControl(Log, Res) });
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(Res.GetString("svcException"), e.ExceptionObject);
        }
    }
}
