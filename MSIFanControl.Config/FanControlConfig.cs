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

using System;
using System.Xml;
using System.Xml.Serialization;

namespace MSIFanControl.Config
{
    /// <summary>
    /// Represents an MSI Fan Control configuration.
    /// </summary>
    public sealed class FanControlConfig
    {
        /// <summary>
        /// The config version expected when loading a config.
        /// </summary>
        [XmlIgnore]
        public const int ExpectedVer = 1;

        /// <summary>
        /// The config version. Should be the same as <see cref="ExpectedVer"/>
        /// unless the config is newer or invalid.
        /// </summary>
        [XmlAttribute]
        public int Version;

        /// <summary>
        /// The laptop model the config was made for.
        /// </summary>
        [XmlElement]
        public string Model;

        /// <summary>
        /// The author of the config file.
        /// </summary>
        [XmlElement]
        public string Author;

        /// <summary>
        /// The list of <see cref="FanConfig"/>s associated with the laptop.
        /// </summary>
        [XmlArray]
        public FanConfig[] FanConfigs;

        /// <summary>
        /// The laptop's Cooler Boost config. May be <c>null</c>.
        /// </summary>
        [XmlElement]
        public FullBlastConfig FullBlastConfig;

        /// <summary>
        /// The laptop's charge threshold config. May be <c>null</c>.
        /// </summary>
        [XmlElement]
        public ChargeLimitConfig ChargeLimitConfig;

        /// <summary>
        /// A list of registers to write when applying a fan config.
        /// May be <c>null</c>, but if not <c>null</c>, must have
        /// at least one <see cref="RegConfig"/>.
        /// </summary>
        [XmlArray]
        public RegConfig[] RegConfigs;

        /// <summary>
        /// Parses an MSI Fan Control config XML and returns an
        /// <see cref="FanControlConfig"/> object.
        /// </summary>
        /// <param name="xmlFile">The path to an XML config file.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when an error occurred while reading the XML config,
        /// most likely due to invalid XML syntax.
        /// </exception>
        /// <exception cref="InvalidConfigException">
        /// Thrown when an invalid config was loaded. This most likely means
        /// that certain required fields are missing from the loaded config.
        /// </exception>
        public static FanControlConfig Load(string xmlFile)
        {
            XmlSerializer serialiser = new XmlSerializer(typeof(FanControlConfig));
            using (XmlReader reader = XmlReader.Create(xmlFile))
            {
                FanControlConfig cfg = (FanControlConfig)serialiser.Deserialize(reader);
                return cfg.IsValid() ? cfg : throw new InvalidConfigException();
            }
        }

        /// <summary>
        /// Saves an MSI Fan Control config to the specified location.
        /// </summary>
        /// <param name="xmlFile">The XML file to write to.</param>
        /// <exception cref="InvalidOperationException"/>
        public void Save(string xmlFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FanControlConfig));
            using (XmlWriter writer = XmlWriter.Create(xmlFile))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Performs some validation on the loaded config to make
        /// sure it is in the expected format.
        /// </summary>
        /// <remarks>
        /// This does NOT guarantee the loaded config is valid!
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the config is valid, otherwise <c>false</c>.
        /// </returns>
        private bool IsValid()
        {
            // Check config version:
            // if the loaded config is older/newer than the version expected
            // by the config library, don't bother checking anything else
            if (Version != ExpectedVer) return false;

            if (string.IsNullOrEmpty(Model) ||
                string.IsNullOrEmpty(Author))
                return false;

            // 1. Check if FanConfigs is not null
            // 2. Check if there's at least 1 FanConfig
            if (FanConfigs?.Length >= 1)
            {
                for (int i = 0; i < FanConfigs.Length; i++)
                {
                    FanConfig cfg = FanConfigs[i];

                    if (string.IsNullOrEmpty(cfg.Name))
                        return false;

                    if (cfg.UpThresholdRegs?.Length >= 1 &&
                        cfg.DownThresholdRegs?.Length >= 1 &&
                        cfg.FanCurveRegs?.Length >= 2 &&
                        cfg.FanCurveConfigs?.Length >= 1)
                    {
                        for (int j = 0; j < cfg.FanCurveConfigs.Length; j++)
                        {
                            FanCurveConfig curveCfg = cfg.FanCurveConfigs[j];
                            if (string.IsNullOrEmpty(curveCfg.Name) ||
                                string.IsNullOrEmpty(curveCfg.Description) ||
                                // there should be exactly one temperature threshold
                                // per fan curve register; if there isn't, return false
                                curveCfg.TempThresholds?.Length != cfg.FanCurveRegs.Length)
                                return false;
                        }
                    }
                    else return false;
                }
            }
            else return false;

            // If the RegConfigs tag is defined in the XML,
            // but has no elements, return false
            if (RegConfigs?.Length == 0)
                return false;

            // All other values are considered to be valid; return true
            return true;
        }
    }
}
