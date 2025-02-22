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

namespace YAMDCC.Common.Configs;

/// <summary>
/// Represents a charge limit config for a laptop.
/// </summary>
public sealed class ChargeLimitConf
{
    /// <summary>
    /// The register that controls the charge limit.
    /// </summary>
    [XmlElement]
    public byte Reg { get; set; }

    /// <summary>
    /// The value that corresponds to 0% charge limit (i.e. disabled).
    /// </summary>
    [XmlElement]
    public byte MinVal { get; set; }

    /// <summary>
    /// The value that corresponds to 100% charge limit.
    /// </summary>
    [XmlElement]
    public byte MaxVal { get; set; }

    /// <summary>
    /// The currently set charge limit value.
    /// </summary>
    [XmlElement]
    public byte CurVal { get; set; }
}
