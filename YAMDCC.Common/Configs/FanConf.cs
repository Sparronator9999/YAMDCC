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
/// Represents a configuration for a fan in the target laptop.
/// </summary>
public sealed class FanConf
{
    /// <summary>
    /// The zero-based index of the <see cref="FanProf"/> to apply for this fan.
    /// </summary>
    [XmlElement]
    public int ProfSel { get; set; }

    /// <summary>
    /// The list of <see cref="FanProf"/>s associated with this fan.
    /// </summary>
    /// <remarks>
    /// If the base config is a template, this may be <see langword="null"/>,
    /// otherwise at least one fan profile (the "default" profile) must exist.
    /// </remarks>
    [XmlArray]
    public List<FanProf> FanProfs { get; set; }
}
