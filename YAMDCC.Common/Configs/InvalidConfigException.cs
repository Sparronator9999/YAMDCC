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

using System;

namespace YAMDCC.Common.Configs;

/// <summary>
/// The exception thrown when an invalid <see cref="YAMDCC_Config"/> is loaded.
/// </summary>
public sealed class InvalidConfigException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidConfigException"/> class.
    /// </summary>
    public InvalidConfigException()
        : base("The config was not in the expected format.") { }
}
