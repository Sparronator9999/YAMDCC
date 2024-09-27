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

using System.Xml.Serialization;

namespace YAMDCC.Config
{
    /// <summary>
    /// Represents a Cooler Boost configuration.
    /// </summary>
    public sealed class FullBlastConf
    {
        /// <summary>
        /// The register that controls the Cooler Boost function.
        /// </summary>
        [XmlElement]
        public byte Reg { get; set; }

        /// <summary>
        /// A bitmask that controls which EC register
        /// bits to toggle when toggling Full Blast.
        /// </summary>
        /// <remarks>
        /// For example, 128 (0x80, or 10000000b) would
        /// toggle the MSB of the Full Blast register.
        /// </remarks>
        [XmlElement]
        public byte Mask { get; set; }
    }
}
