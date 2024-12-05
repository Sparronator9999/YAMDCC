// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
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

namespace YAMDCC.Config
{
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
        /// The fan speeds and associated up and down thresholds.
        /// </summary>
        [XmlArray]
        public TempThreshold[] TempThresholds { get; set; }

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
            newCfg.Name = string.Copy(Name);
            newCfg.Desc = string.Copy(Desc);
            newCfg.TempThresholds = new TempThreshold[TempThresholds.Length];
            for (int i = 0; i < newCfg.TempThresholds.Length; i++)
            {
                newCfg.TempThresholds[i] = TempThresholds[i].Copy();
            }
            return newCfg;
        }
    }
}
