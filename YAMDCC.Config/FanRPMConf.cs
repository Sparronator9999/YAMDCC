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

using System;
using System.Xml.Serialization;

namespace YAMDCC.Config
{
    [Serializable]
    public sealed class FanRPMConf
    {
        /// <summary>
        /// The register to read to get the fan RPM.
        /// </summary>
        [XmlElement]
        public byte ReadReg { get; set; }

        /// <summary>
        /// Is the RPM value stored as a word (16-bit) or byte (8-bit)?
        /// </summary>
        [XmlElement]
        public bool Is16Bit { get; set; }

        /// <summary>
        /// Is the RPM value big-endian? This will only have an
        /// effect if <see cref="Is16Bit"/> is set to <c>true</c>.
        /// </summary>
        [XmlElement]
        public bool IsBigEndian { get; set; }

        /// <summary>
        /// The value to multiply (or divide, if <see cref="DivideByMult"/>
        /// is <c>true</c>) the read RPM value by.
        /// </summary>
        [XmlElement]
        public int RPMMult { get; set; } = 1;

        /// <summary>
        /// If <c>true</c>, divides the read RPM value by
        /// <see cref="RPMMult"/> instead of multiplying.
        /// </summary>
        [XmlElement]
        public bool DivideByMult { get; set; }

        /// <summary>
        /// Set to true if the read RPM value starts high
        /// and decreases as the fan speed increases.
        /// </summary>
        [XmlElement]
        public bool Invert { get; set; }
    }
}
