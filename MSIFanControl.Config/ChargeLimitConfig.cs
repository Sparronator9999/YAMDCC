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
    /// Represents a charge threshold (a.k.a charge limit) config for a laptop.
    /// </summary>
    public sealed class ChargeLimitConfig
    {
        /// <summary>
        /// The register that controls the charge threshold.
        /// </summary>
        [XmlElement]
        public byte Register;

        /// <summary>
        /// The value that corresponds to 0% battery threshold.
        /// </summary>
        [XmlElement]
        public byte MinValue;

        /// <summary>
        /// The value that corresponds to 100% battery threshold.
        /// </summary>
        [XmlElement]
        public byte MaxValue;

        /// <summary>
        /// The currently set Charge Threshold value.
        /// </summary>
        [XmlElement]
        public byte Value;
    }
}
