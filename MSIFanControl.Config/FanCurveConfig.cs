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

using System.Xml.Serialization;

namespace MSIFanControl.Config
{
	/// <summary>
	/// Represents a fan profile (a.k.a. fan curve) config.
	/// </summary>
	public sealed class FanCurveConfig
	{
		/// <summary>
		/// The name of the fan profile.
		/// </summary>
		[XmlElement]
		public string Name;

		/// <summary>
		/// The description of the fan profile.
		/// </summary>
		[XmlElement]
		public string Description;

		/// <summary>
		/// The fan speeds and associated up and down thresholds.
		/// </summary>
		[XmlArray]
		public TempThreshold[] TempThresholds;
	}
}
