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
using YAMDCC.Logs;

namespace YAMDCC.Common;

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
    public int Ver { get; set; }

    /// <summary>
    /// The product this <see cref="CommonConfig"/> was made for.
    /// </summary>
    [XmlAttribute]
    public string App { get; set; }

    /// <summary>
    /// How verbose logs should be.
    /// </summary>
    [XmlElement]
    public LogLevel LogLevel { get; set; } = LogLevel.Debug;

    /// <summary>
    /// Loads the global app config XML and returns a
    /// <see cref="CommonConfig"/> object.
    /// </summary>
    public static CommonConfig Load()
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
    public void Save()
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
