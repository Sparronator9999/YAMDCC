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

namespace YAMDCC.Common.Configs;

/// <summary>
/// Contains an MSI laptop's various fan modes
/// (i.e. Default, Silent, Basic, and Advanced).
/// </summary>
public sealed class FanModeConf
{
    /// <summary>
    /// The register that controls the fan mode.
    /// </summary>
    [XmlElement]
    public byte Reg { get; set; }

    /// <summary>
    /// The currently selected fan mode, as
    /// an index of the available fan modes.
    /// </summary>
    [XmlElement]
    public int ModeSel { get; set; }

    /// <summary>
    /// An array of possible fan modes for the laptop.
    /// </summary>
    [XmlArray]
    public List<FanMode> FanModes { get; set; }
}
