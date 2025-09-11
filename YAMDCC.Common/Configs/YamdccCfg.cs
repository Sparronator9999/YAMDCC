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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace YAMDCC.Common.Configs;

/// <summary>
/// Represents a YAMDCC configuration.
/// </summary>
public sealed class YamdccCfg
{
    /// <summary>
    /// The config version expected when loading a config.
    /// </summary>
    [XmlIgnore]
    public const int ExpectedVer = 2;

    /// <summary>
    /// The config version. Should be the same as <see cref="ExpectedVer"/>
    /// unless the config is newer or invalid.
    /// </summary>
    [XmlAttribute]
    public int Ver { get; set; }

    /// <summary>
    /// The manufacturer of the laptop the config was made for.
    /// </summary>
    [XmlElement]
    public string Manufacturer { get; set; }

    /// <summary>
    /// The laptop model the config was made for.
    /// </summary>
    [XmlElement]
    public string Model { get; set; }

    /// <summary>
    /// The author of the config file.
    /// </summary>
    [XmlElement]
    public string Author { get; set; }

    /// <summary>
    /// The EC firmware version of the laptop this config was made for.
    /// </summary>
    [XmlElement]
    public string FirmVer { get; set; }

    /// <summary>
    /// The EC firmware date of the laptop this config was made for.
    /// </summary>
    [XmlElement]
    public DateTime? FirmDate { get; set; }

    /// <summary>
    /// <see langword="true"/> if this laptop uses a new (WMI2) EC,
    /// otherwise <see cref="false"/>.
    /// </summary>
    [XmlElement]
    public bool IsNewEC { get; set; }

    /// <summary>
    /// Set to <see langword="true"/> if the temperature down thresholds are
    /// stored in the EC as an offset from the corresponding up threshold,
    /// otherwise <see langword="false"/>.
    /// </summary>
    /// <remarks>
    /// Some MSI laptops released in 2025 or later are known to need this disabled.
    /// </remarks>
    [XmlElement]
    public bool OffsetDT { get; set; } = true;

    /// <summary>
    /// The CPU fan configuration, as a <see cref="FanConf"/>.
    /// </summary>
    [XmlElement]
    public FanConf CpuFan { get; set; }

    /// <summary>
    /// The GPU fan configuration, as a <see cref="FanConf"/>.
    /// </summary>
    [XmlElement]
    public FanConf GpuFan { get; set; }

    /// <summary>
    /// The laptop's current charge limit.
    /// </summary>
    /// <remarks>
    /// Expects any value between 0-100, with 0 disabling charge limit.
    /// </remarks>
    [XmlElement]
    public byte ChargeLim { get; set; }

    /// <summary>
    /// The laptop's default performance mode setting.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="PerfMode.Balanced"/>.
    /// </remarks>
    [XmlElement]
    public PerfMode PerfMode { get; set; } = PerfMode.Balanced;

    /// <summary>
    /// The laptop's fan mode.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="FanMode.Advanced"/>.
    /// </remarks>
    [XmlElement]
    public FanMode FanMode { get; set; } = FanMode.Advanced;

    /// <summary>
    /// The laptop's Win/Fn keyboard swap config.
    /// </summary>
    [XmlElement]
    public bool KeySwapEnabled { get; set; }

    /// <summary>
    /// Parses a YAMDCC config XML and returns a
    /// <see cref="YamdccCfg"/> object.
    /// </summary>
    /// <param name="path">
    /// The path to an XML config file.
    /// </param>
    /// <exception cref="InvalidConfigException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="InvalidOperationException"/>
    public static YamdccCfg Load(string path)
    {
        XmlSerializer serialiser = new(typeof(YamdccCfg));
        using (XmlReader reader = XmlReader.Create(path))
        {
            YamdccCfg cfg = (YamdccCfg)serialiser.Deserialize(reader);
            return cfg.IsValid() ? cfg : throw new InvalidConfigException();
        }
    }

    /// <summary>
    /// Saves a YAMDCC config to the specified location.
    /// </summary>
    /// <param name="path">
    /// The XML file to write to.
    /// </param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public void Save(string path)
    {
        XmlSerializer serialiser = new(typeof(YamdccCfg));
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
        };

        using (XmlWriter writer = XmlWriter.Create(path, settings))
        {
            serialiser.Serialize(writer, this);
        }
    }

    /// <summary>
    /// Performs some validation on the loaded config to make
    /// sure it is in the expected format.
    /// </summary>
    /// <remarks>
    /// This does NOT guarantee the loaded config is valid!
    /// (e.g. register values are not checked)
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if the config is valid, otherwise <see langword="false"/>.
    /// </returns>
    private bool IsValid()
    {
        // Check the config version.
        // Pretty self-explanatory, if the loaded config is older/newer
        // than the version expected by the config library, don't bother
        // checking anything else as some/all of it is probably invalid.
        if (Ver != ExpectedVer)
        {
            return false;
        }

        if (string.IsNullOrEmpty(Manufacturer) ||
            string.IsNullOrEmpty(Model) ||
            string.IsNullOrEmpty(Author))
        {
            return false;
        }

        // Make sure both CPU and GPU fan configs are not null
        if (CpuFan is null || GpuFan is null)
        {
            return false;
        }

        // all fans must have same number of fan profiles
        if (CpuFan.FanProfs.Count != GpuFan.FanProfs.Count)
        {
            return false;
        }

        // the selected fan profile shouldn't be higher than
        // the number of fan profiles in the config.
        if (CpuFan.ProfSel >= CpuFan.FanProfs.Count ||
            CpuFan.ProfSel < 0)
        {
            // if the fan profile selection is out of range,
            // silently set it to 0 (the first fan profile)
            // which should always exist:
            CpuFan.ProfSel = 0;
        }

        if (GpuFan.ProfSel >= GpuFan.FanProfs.Count ||
            GpuFan.ProfSel < 0)
        {
            GpuFan.ProfSel = 0;
        }

        for (int j = 0; j < CpuFan.FanProfs.Count; j++)
        {
            FanProf curveCfg = CpuFan.FanProfs[j];
            if (string.IsNullOrEmpty(curveCfg.Name) ||
                string.IsNullOrEmpty(curveCfg.Desc))
            {
                return false;
            }

            for (int k = 0; k < curveCfg.Thresholds.Count; k++)
            {
                if (curveCfg.Thresholds[k] is null ||
                    curveCfg.Thresholds[k].Speed < 0 ||
                    curveCfg.Thresholds[k].Speed > 150)
                {
                    return false;
                }
            }
        }

        for (int j = 0; j < GpuFan.FanProfs.Count; j++)
        {
            FanProf curveCfg = GpuFan.FanProfs[j];
            if (string.IsNullOrEmpty(curveCfg.Name) ||
                string.IsNullOrEmpty(curveCfg.Desc))
            {
                return false;
            }

            for (int k = 0; k < curveCfg.Thresholds.Count; k++)
            {
                if (curveCfg.Thresholds[k] is null ||
                    curveCfg.Thresholds[k].Speed < 0 ||
                    curveCfg.Thresholds[k].Speed > 150)
                {
                    return false;
                }
            }
        }

        // make sure charge limit to apply is between 0 and 100%.
        // Restrict to that range if not:
        if (ChargeLim < 0)
        {
            ChargeLim = 0;
        }
        else if (ChargeLim > 100)
        {
            ChargeLim = 100;
        }

        // All other values are considered to be valid; return true.
        // Note that registers aren't checked and are (almost) always
        // expected to be nonzero.
        return true;
    }
}

public enum PerfMode
{
    Default = -1,
    MaxBattery = 0,
    Silent = 1,
    Balanced = 2,
    Performance = 3,
}

public enum FanMode
{
    Auto = 0,
    Silent = 1,
    Basic = 2,
    Advanced = 3,
}
