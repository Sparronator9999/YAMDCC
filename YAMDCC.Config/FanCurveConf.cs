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
/// Represents a fan profile (a.k.a. fan curve) config.
/// </summary>
public sealed class FanCurveConf
{
    /// <summary>
    /// The name of the fan profile.
    /// </summary>
    [XmlElement]
    public string Name { get; set; }

    /// <summary>
    /// The description of the fan profile.
    /// </summary>
    [XmlElement]
    public string Desc { get; set; }

    /// <summary>
    /// The <see cref="PerfMode"/> to use with this fan profile,
    /// as an index of the available performance modes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This setting is ignored if this <see cref="FanCurveConf"/>
    /// is not for the first fan in the computer.
    /// </para>
    /// <para>
    /// Set to -1 to use the default performance mode
    /// (as set by <see cref="PerfModeConf.ModeSel"/>).
    /// </para>
    /// </remarks>
    [XmlElement]
    public int PerfModeSel { get; set; } = -1;

    /// <summary>
    /// The fan speeds and associated up and down thresholds.
    /// </summary>
    [XmlArray]
    public List<TempThreshold> TempThresholds { get; set; }

    /// <summary>
    /// Creates a deep copy of this <see cref="FanCurveConf"/>.
    /// </summary>
    /// <returns>
    /// A copy of this <see cref="FanCurveConf"/>.
    /// </returns>
    public FanCurveConf Copy()
    {
        // create a shallow copy of this FanCurveConfig
        FanCurveConf newCfg = (FanCurveConf)MemberwiseClone();

        // create a copy of everything that didn't get copied by the above
        newCfg.TempThresholds = new List<TempThreshold>(TempThresholds.Count);
        for (int i = 0; i < TempThresholds.Count; i++)
        {
            newCfg.TempThresholds.Add(TempThresholds[i].Copy());
        }
        return newCfg;
    }
}
