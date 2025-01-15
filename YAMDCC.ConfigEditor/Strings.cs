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

using System.Globalization;
using System.Resources;

namespace YAMDCC.ConfigEditor;

/// <summary>
/// A resource class for retrieving strings.
/// </summary>
internal static class Strings
{
    private static ResourceManager resMan;

    /// <summary>
    /// Gets a string from the underlying resource file, and
    /// replaces format objects with their string representation.
    /// </summary>
    /// <param name="name">
    /// The name of the string to find.
    /// </param>
    /// <param name="args">
    /// The objects to format the string with.
    /// </param>
    /// <returns>
    /// <para>
    /// The formatted string corresponding to
    /// the specified string name, if found.
    /// </para>
    /// <para><see langword="null"/> if the string couldn't be found.</para>
    /// </returns>
    public static string GetString(string name, params object[] args)
    {
        resMan ??= new ResourceManager(typeof(Strings));
        string temp = resMan.GetString(name, CultureInfo.InvariantCulture);

        return temp is null
            ? null
            : string.Format(CultureInfo.InvariantCulture, temp, args);
    }
}
