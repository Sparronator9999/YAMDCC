using System.Xml.Serialization;

namespace YAMDCC.Config
{
    public sealed class PerfModeConf
    {
        /// <summary>
        /// The register that controls the performance mode.
        /// </summary>
        [XmlElement]
        public byte Reg { get; set; }

        /// <summary>
        /// The currently selected performance mode, as
        /// an index of the available performance modes.
        /// </summary>
        [XmlElement]
        public int ModeSel { get; set; }

        /// <summary>
        /// An array of possible performance modes for the laptop.
        /// </summary>
        [XmlArray]
        public PerfMode[] PerfModes { get; set; }
    }
}
