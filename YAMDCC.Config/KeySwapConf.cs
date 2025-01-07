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

using System.Xml.Serialization;

namespace YAMDCC.Config;

/// <summary>
/// Represents a configuration for the Win/Fn key swap feature of a laptop.
/// </summary>
public sealed class KeySwapConf
{
    /// <summary>
    /// The register that controls the Win/Fn key swap state.
    /// </summary>
    [XmlElement]
    public byte Reg { get; set; }

    /// <summary>
    /// Is the Win/Fn key swap feature enabled?
    /// </summary>
    [XmlElement]
    public bool Enabled { get; set; }

    /// <summary>
    /// The value to turn on Win/Fn key swapping.
    /// </summary>
    [XmlElement]
    public byte OnVal { get; set; }

    /// <summary>
    /// The value to turn off Win/Fn key swapping.
    /// </summary>
    [XmlElement]
    public byte OffVal { get; set; }
}
