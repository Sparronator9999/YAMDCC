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
/// Deprecated as of v1.1.0, and will be removed in a future release. Do not use.
/// </summary>
public sealed class RegConf
{
    /// <summary>
    /// Should this <see cref="RegConf"/> be applied?
    /// </summary>
    [XmlElement]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// A short name for this EC register config.
    /// </summary>
    [XmlElement]
    public string Name { get; set; }

    /// <summary>
    /// A longer description of what this config does.
    /// </summary>
    [XmlElement]
    public string Desc { get; set; }

    /// <summary>
    /// The register to write to.
    /// </summary>
    [XmlElement]
    public byte Reg { get; set; }

    /// <summary>
    /// The value to write to the register when
    /// this <see cref="RegConf"/> is enabled.
    /// </summary>
    [XmlElement]
    public byte OnVal { get; set; }

    /// <summary>
    /// The value to write to the register when
    /// this <see cref="RegConf"/> is disabled.
    /// </summary>
    [XmlElement]
    public byte OffVal { get; set; }
}
