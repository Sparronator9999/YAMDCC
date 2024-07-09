// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using System.Xml.Serialization;

namespace MSIFanControl.Config
{
    /// <summary>
    /// Represents a fan speed/temperature threshold setting for a fan curve.
    /// </summary>
    public sealed class TempThreshold
    {
        /// <summary>
        /// The temperature threshold before the fan speeds up to this fan speed.
        /// </summary>
        /// <remarks>
        /// Ignored if this is the last temperature threshold in the list
        /// (i.e. this is the highest fan speed that can be set).
        /// </remarks>
        [XmlElement]
        public byte UpThreshold;

        /// <summary>
        /// The temperature threshold before the fan slows down to the previous fan speed.
        /// </summary>
        /// <remarks>
        /// Ignored if this is the first temperature threshold in the list
        /// (i.e. this is the default fan speed).
        /// </remarks>
        [XmlElement]
        public byte DownThreshold;

        /// <summary>
        /// The target fan speed to set when reaching the up threshold.
        /// </summary>
        [XmlElement]
        public byte FanSpeed;

        /// <summary>
        /// Creates a copy of this <seealso cref="TempThreshold"/>.
        /// </summary>
        /// <returns>
        /// The copy of this <seealso cref="TempThreshold"/>
        /// </returns>
        public TempThreshold Copy()
        {
            return (TempThreshold)MemberwiseClone();
        }
    }
}
