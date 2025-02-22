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
using YAMDCC.Common.Logs;

namespace YAMDCC.Common.Configs;

public class CommonConfig
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
    public int Ver { get; set; } = 1;

    /// <summary>
    /// The path to the last YAMDCC config loaded by the config editor.
    /// </summary>
    [XmlElement]
    public string LastConf { get; set; }

    /// <summary>
    /// The current progress of the EC-to-config feature.
    /// </summary>
    /// <remarks>
    /// 0 = EC-to-config not in progress<br/>
    /// 1 = EC-to-config pending, reboot required<br/>
    /// 2 = EC-to-config pending, post-reboot<br/>
    /// 3 = EC-to-config successful<br/>
    /// 4 = EC-to-config failed
    /// </remarks>
    [XmlElement]
    public ECtoConfState ECtoConfState { get; set; }

    /// <summary>
    /// How verbose logs should be.
    /// </summary>
    [XmlElement]
    public LogLevel LogLevel { get; set; } = LogLevel.Debug;

    /// <summary>
    /// <see langword="true"/> if we've already asked to enable auto-updating,
    /// otherwise <see langword="false"/>.
    /// </summary>
    [XmlElement]
    public bool AutoUpdateAsked { get; set; }

    /// <summary>
    /// <see langword="true"/> if the YAMDCC updater should update
    /// to pre-releases, otherwise <see langword="false"/>.
    /// </summary>
    [XmlElement]
    public bool PreRelease { get; set; }

    public static string GetLastConf()
    {
        return Load().LastConf;
    }

    public static ECtoConfState GetECtoConfState()
    {
        return Load().ECtoConfState;
    }

    public static LogLevel GetLogLevel()
    {
        return Load().LogLevel;
    }

    public static bool GetAutoUpdateAsked()
    {
        return Load().AutoUpdateAsked;
    }

    public static bool GetPreRelease()
    {
        return Load().PreRelease;
    }

    public static void SetLastConf(string path)
    {
        CommonConfig cfg = Load();
        cfg.LastConf = path;
        cfg.Save();
    }

    public static void SetECtoConfState(ECtoConfState state)
    {
        CommonConfig cfg = Load();
        cfg.ECtoConfState = state;
        cfg.Save();
    }

    public static void SetLogLevel(LogLevel level)
    {
        CommonConfig cfg = Load();
        cfg.LogLevel = level;
        cfg.Save();
    }

    public static void SetAutoUpdateAsked(bool value)
    {
        CommonConfig cfg = Load();
        cfg.AutoUpdateAsked = value;
        cfg.Save();
    }

    public static void SetPreRelease(bool value)
    {
        CommonConfig cfg = Load();
        cfg.PreRelease = value;
        cfg.Save();
    }

    /// <summary>
    /// Loads the global app config XML and returns a
    /// <see cref="CommonConfig"/> object.
    /// </summary>
    private static CommonConfig Load()
    {
        XmlSerializer serialiser = new(typeof(CommonConfig));
        try
        {
            using (XmlReader reader = XmlReader.Create(Paths.GlobalConf))
            {
                CommonConfig cfg = (CommonConfig)serialiser.Deserialize(reader);
                return cfg.Ver == ExpectedVer
                    ? cfg
                    : throw new InvalidConfigException();
            }
        }
        catch (Exception ex)
        {
            if (ex is FileNotFoundException
                or InvalidOperationException
                or InvalidConfigException)
            {
                return new CommonConfig();
            }
            else
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Saves the global app config XML.
    /// </summary>
    /// <exception cref="InvalidOperationException"/>
    private void Save()
    {
        XmlSerializer serializer = new(typeof(CommonConfig));
        XmlWriterSettings settings = new()
        {
            Indent = true,
            IndentChars = "\t",
        };

        using (XmlWriter writer = XmlWriter.Create(Paths.GlobalConf, settings))
        {
            serializer.Serialize(writer, this);
        }
    }
}

public enum ECtoConfState
{
    None,
    PendingReboot,
    PostReboot,
    Success,
    Fail,
}
