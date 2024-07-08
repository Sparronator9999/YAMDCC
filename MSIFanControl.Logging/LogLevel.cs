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

namespace MSIFanControl.Logs
{
	/// <summary>
	/// The verbosity of logs
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Do not log anything.
		/// </summary>
		None = 0,

		/// <summary>
		/// Only log Fatal events.
		/// </summary>
		Fatal = 1,

		/// <summary>
		/// Log Errors and Fatal events.
		/// </summary>
		Error = 2,

		/// <summary>
		/// Log Warnings, Errors, and Fatal events.
		/// </summary>
		Warn = 3,

		/// <summary>
		/// Log all events, except for Debug events.
		/// </summary>
		Info = 4,

		/// <summary>
		/// Log all events.
		/// </summary>
		Debug = 5,
	}
}
