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
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace YAMDCC.Common.Configs;

// IDE0079: Remove unnecessary suppression (even though it *is* necessary)
#pragma warning disable IDE0079
// CA1707: Identifiers should not contain underscores
#pragma warning disable CA1707
/// <summary>
/// Represents a YAMDCC configuration.
/// </summary>

public sealed class YAMDCC_Config
#pragma warning restore CA1707
#pragma warning restore IDE0079
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
    /// Set to <see langword="true"/> if the EC supports reading
    /// firmware version from registers <c>0xA0</c>-<c>0xBB</c>.
    /// </summary>
    /// <remarks>
    /// If <see langword="false"/>, the <see cref="FirmVer"/>
    /// and <see cref="FirmDate"/> properties are ignored.
    /// </remarks>
    [XmlElement]
    public bool FirmVerSupported { get; set; }

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
    /// The list of <see cref="FanConf"/>s associated with the laptop.
    /// </summary>
    [XmlArray]
    public List<FanConf> FanConfs { get; set; }

    /// <summary>
    /// The laptop's Full Blast config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not supported on the laptop.
    /// </remarks>
    [XmlElement]
    public FullBlastConf FullBlastConf { get; set; }

    /// <summary>
    /// The laptop's charge threshold config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not supported on the laptop.
    /// </remarks>
    [XmlElement]
    public ChargeLimitConf ChargeLimitConf { get; set; }

    /// <summary>
    /// The laptop's performance mode config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not supported on the laptop.
    /// </remarks>
    [XmlElement]
    public PerfModeConf PerfModeConf { get; set; }

    /// <summary>
    /// The laptop's fan mode config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not supported on the laptop.
    /// </remarks>
    [XmlElement]
    public FanModeConf FanModeConf { get; set; }

    /// <summary>
    /// The laptop's Win/Fn keyboard swap config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not supported on the laptop.
    /// </remarks>
    [XmlElement]
    public KeySwapConf KeySwapConf { get; set; }

    /// <summary>
    /// The laptop's keyboard backlight config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> if not supported on the laptop.
    /// </remarks>
    [XmlElement]
    public KeyLightConf KeyLightConf { get; set; }

    /// <summary>
    /// A list of registers to write when applying a fan config.
    /// </summary>
    /// <remarks>
    /// May be <see langword="null"/> or empty if not needed.
    /// </remarks>
    [XmlArray]
    public List<RegConf> RegConfs { get; set; }

    /// <summary>
    /// Parses a YAMDCC config XML and returns a
    /// <see cref="YAMDCC_Config"/> object.
    /// </summary>
    /// <param name="path">
    /// The path to an XML config file.
    /// </param>
    /// <exception cref="InvalidConfigException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="InvalidOperationException"/>
    public static YAMDCC_Config Load(string path)
    {
        XmlSerializer serialiser = new(typeof(YAMDCC_Config));
        using (XmlReader reader = XmlReader.Create(path))
        {
            YAMDCC_Config cfg = (YAMDCC_Config)serialiser.Deserialize(reader);
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
        XmlSerializer serialiser = new(typeof(YAMDCC_Config));
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

        // 1. Check if FanConfigs is not null
        // 2. Check if there's at least 1 FanConfig
        if (FanConfs?.Count < 1)
        {
            return false;
        }

        for (int i = 0; i < FanConfs.Count; i++)
        {
            FanConf cfg = FanConfs[i];

            if (string.IsNullOrEmpty(cfg.Name))
            {
                return false;
            }

            // all fans must have same number of fan profiles now
            if (!FansHaveSameProfCount())
            {
                return false;
            }

            // YAMDCC doesn't handle MinSpeed lower than MaxSpeed,
            // so return false if MinSpeed is lower or equal to MaxSpeed:
            if (cfg.MinSpeed >= cfg.MaxSpeed)
            {
                return false;
            }

            // the selected fan curve shouldn't be higher than
            // the number of fan curves in the config.
            if (cfg.CurveSel >= FanConfs[i].FanCurveConfs.Count ||
                cfg.CurveSel < 0)
            {
                // if the fan profile selection is out of range,
                // silently set it to 0 (the first fan curve)
                // which should always exist:
                cfg.CurveSel = 0;
            }

            // make sure that:
            // - there is at least one each of up threshold, down threshold,
            //   and fan curve registers
            // - there are the same amount of up threshold registers
            //   as down threshold registers
            // - there is one more fan profile register than up/down threshold registers
            // - there is at least one fan profile to apply (first should be Default)
            if (cfg.UpThresholdRegs?.Length < 1 ||
                cfg.UpThresholdRegs?.Length != cfg.DownThresholdRegs?.Length ||
                cfg.FanCurveRegs?.Length != cfg.UpThresholdRegs?.Length + 1 ||
                cfg.FanCurveConfs?.Count < 1)
            {
                return false;
            }

            for (int j = 0; j < cfg.FanCurveConfs.Count; j++)
            {
                FanCurveConf curveCfg = cfg.FanCurveConfs[j];
                if (string.IsNullOrEmpty(curveCfg.Name) ||
                    string.IsNullOrEmpty(curveCfg.Desc) ||
                    // there should be exactly one temperature threshold
                    // per fan proffile register; if there isn't, return false
                    curveCfg.TempThresholds?.Count != cfg.FanCurveRegs.Length)
                {
                    return false;
                }

                for (int k = 0; k < curveCfg.TempThresholds.Count; k++)
                {
                    if (curveCfg.TempThresholds[k] is null)
                    {
                        return false;
                    }
                }
            }
        }

        if (FullBlastConf is not null)
        {
            // full blast mask shouldn't be 0, as that would make it impossible
            // to change the full blast register's value when full blast toggled on/off
            if (FullBlastConf.Mask == 0)
            {
                return false;
            }
        }

        if (ChargeLimitConf is not null)
        {
            // YAMDCC cannot handle a lower min value than max value,
            // so return false if that's the case for this config
            if (ChargeLimitConf.MinVal >= ChargeLimitConf.MaxVal)
            {
                return false;
            }

            // make sure charge limit to apply is within the config's
            // defined bounds, but don't fail validation if it's not:
            if (ChargeLimitConf.CurVal > ChargeLimitConf.MaxVal - ChargeLimitConf.MinVal)
            {
                ChargeLimitConf.CurVal = ChargeLimitConf.MaxVal;
            }
            else if (ChargeLimitConf.CurVal < 0)
            {
                ChargeLimitConf.CurVal = ChargeLimitConf.MinVal;
            }
        }

        if (PerfModeConf is not null)
        {
            if (PerfModeConf.PerfModes?.Count < 1)
            {
                return false;
            }

            // the selected performance mode shouldn't be higher than
            // the number of performance modes in the config
            if (PerfModeConf.ModeSel >= PerfModeConf.PerfModes.Count ||
                PerfModeConf.ModeSel < 0)
            {
                // same as fan profile selection, set the performance
                // mode to the first available performance mode:
                PerfModeConf.ModeSel = 0;
            }

            for (int i = 0; i < PerfModeConf.PerfModes.Count; i++)
            {
                PerfMode perfMode = PerfModeConf.PerfModes[i];

                if (string.IsNullOrEmpty(perfMode.Name) ||
                    string.IsNullOrEmpty(perfMode.Desc))
                {
                    return false;
                }
            }
        }

        if (FanModeConf is not null)
        {
            if (FanModeConf.FanModes?.Count < 1)
            {
                return false;
            }

            // you know the drill by now
            if (FanModeConf.ModeSel >= FanModeConf.FanModes.Count ||
                FanModeConf.ModeSel < 0)
            {
                FanModeConf.ModeSel = 0;
            }

            for (int i = 0; i < FanModeConf.FanModes.Count; i++)
            {
                FanMode fanMode = FanModeConf.FanModes[i];

                if (string.IsNullOrEmpty(fanMode.Name) ||
                    string.IsNullOrEmpty(fanMode.Desc))
                {
                    return false;
                }
            }
        }

        if (KeySwapConf?.OnVal == KeySwapConf?.OffVal)
        {
            return false;
        }

        if (KeyLightConf?.MinVal >= KeyLightConf?.MaxVal)
        {
            return false;
        }

        if (RegConfs?.Count > 0)
        {
            for (int i = 0; i < RegConfs.Count; i++)
            {
                if (string.IsNullOrEmpty(RegConfs[i].Name) ||
                    string.IsNullOrEmpty(RegConfs[i].Desc))
                {
                    return false;
                }
            }
        }

        // All other values are considered to be valid; return true.
        // Note that registers aren't checked and are (almost) always
        // expected to be nonzero.
        return true;
    }

    private bool FansHaveSameProfCount()
    {
        for (int i = 0; i < FanConfs.Count - 1; i++)
        {
            if (FanConfs[i].FanCurveConfs.Count != FanConfs[i + 1].FanCurveConfs.Count)
            {
                return false;
            }
        }
        return true;
    }
}
