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
    /// Represents a configuration for a fan in the target laptop.
    /// </summary>
    public sealed class FanConf
    {
        /// <summary>
        /// The display name of the fan in the curve editor.
        /// </summary>
        [XmlElement]
        public string Name;

        /// <summary>
        /// The minimum possible register value for the fan speed.
        /// </summary>
        [XmlElement]
        public byte MinSpeed;

        /// <summary>
        /// The maximum possible register value for the fan speed.
        /// </summary>
        [XmlElement]
        public byte MaxSpeed;

        /// <summary>
        /// The zero-based index of the <see cref="FanCurveConf"/> to apply for this fan.
        /// </summary>
        [XmlElement]
        public int CurveSel;

        /// <summary>
        /// The register to read to get the fan speed percentage.
        /// </summary>
        [XmlElement]
        public byte SpeedReadReg;

        /// <summary>
        /// The register to read to get the temperature
        /// of the component that controls this fan's speed.
        /// </summary>
        [XmlElement]
        public byte TempReadReg;

        /// <summary>
        /// Contains information on how to calculate the fan RPM.
        /// </summary>
        /// <remarks>
        /// May be <c>null</c>.
        /// </remarks>
        [XmlElement]
        public FanRPMConf RPMConf;

        /// <summary>
        /// The registers that the up thresholds are written to.
        /// </summary>
        [XmlArray]
        public byte[] UpThresholdRegs;

        /// <summary>
        /// The registers that the down thresholds are written to.
        /// </summary>
        [XmlArray]
        public byte[] DownThresholdRegs;

        /// <summary>
        /// The registers to write a fan speed profile to.
        /// </summary>
        [XmlArray]
        public byte[] FanCurveRegs;

        /// <summary>
        /// The list of <see cref="FanCurveConf"/>s associated with this fan.
        /// </summary>
        [XmlArray]
        public FanCurveConf[] FanCurveConfs;
    }
}
