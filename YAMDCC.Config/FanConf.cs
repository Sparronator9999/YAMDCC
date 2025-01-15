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

using System.Collections.Generic;
using System.Xml.Serialization;

namespace YAMDCC.Config;

/// <summary>
/// Represents a configuration for a fan in the target laptop.
/// </summary>
public sealed class FanConf
{
    /// <summary>
    /// The display name of the fan in the config editor.
    /// </summary>
    [XmlElement]
    public string Name { get; set; }

    /// <summary>
    /// The minimum possible register value for the fan speed.
    /// </summary>
    [XmlElement]
    public byte MinSpeed { get; set; }

    /// <summary>
    /// The maximum possible register value for the fan speed.
    /// </summary>
    [XmlElement]
    public byte MaxSpeed { get; set; }

    /// <summary>
    /// The zero-based index of the <see cref="FanCurveConf"/> to apply for this fan.
    /// </summary>
    [XmlElement]
    public int CurveSel { get; set; }

    /// <summary>
    /// The register to read to get the fan speed percentage.
    /// </summary>
    [XmlElement]
    public byte SpeedReadReg { get; set; }

    /// <summary>
    /// The register to read to get the temperature
    /// of the component that controls this fan's speed.
    /// </summary>
    [XmlElement]
    public byte TempReadReg { get; set; }

    /// <summary>
    /// Contains information on how to calculate the fan RPM.
    /// </summary>
    /// <remarks>
    /// May be <c>null</c>.
    /// </remarks>
    [XmlElement]
    public FanRPMConf RPMConf { get; set; }

    /// <summary>
    /// The registers that the up thresholds are written to.
    /// </summary>
    [XmlArray]
    public byte[] UpThresholdRegs { get; set; }

    /// <summary>
    /// The registers that the down thresholds are written to.
    /// </summary>
    [XmlArray]
    public byte[] DownThresholdRegs { get; set; }

    /// <summary>
    /// The registers to write a fan speed profile to.
    /// </summary>
    [XmlArray]
    public byte[] FanCurveRegs { get; set; }

    /// <summary>
    /// The list of <see cref="FanCurveConf"/>s associated with this fan.
    /// </summary>
    /// <remarks>
    /// If the base config is a template, this may be <c>null</c>,
    /// otherwise at least one fan curve (the "default" curve) must exist.
    /// </remarks>
    [XmlArray]
    public List<FanCurveConf> FanCurveConfs { get; set; }
}
