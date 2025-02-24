using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using YAMDCC.Common.Configs;

namespace YAMDCC.HotkeyHandler.Config;

public class HotkeyConf
{
    [XmlAttribute]
    public int Ver { get; set; } = 1;

    [XmlIgnore]
    private static readonly int ExpectedVer = 1;

    [XmlArray]
    public List<Hotkey> Hotkeys { get; set; } = [];

    /// <summary>
    /// Parses a hotkey config XML and returns a
    /// <see cref="HotkeyConf"/> object.
    /// </summary>
    /// <param name="path">
    /// The path to an XML config file.
    /// </param>
    /// <exception cref="InvalidConfigException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="FileNotFoundException"/>
    /// <exception cref="InvalidOperationException"/>
    public static HotkeyConf Load(string path)
    {
        XmlSerializer serialiser = new(typeof(HotkeyConf));
        using (XmlReader reader = XmlReader.Create(path))
        {
            HotkeyConf cfg = (HotkeyConf)serialiser.Deserialize(reader);
            return cfg.IsValid() ? cfg : throw new InvalidConfigException();
        }
    }

    /// <summary>
    /// Saves a hotkey config to the specified location.
    /// </summary>
    /// <param name="path">
    /// The XML file to write to.
    /// </param>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    public void Save(string path)
    {
        XmlSerializer serialiser = new(typeof(HotkeyConf));
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

        // All other values are considered to be valid; return true.
        return true;
    }
}
