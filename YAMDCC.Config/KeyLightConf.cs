using System.Xml.Serialization;

namespace YAMDCC.Config
{
    public sealed class KeyLightConf
    {
        /// <summary>
        /// The register that controls the keyboard backlight.
        /// </summary>
        [XmlElement]
        public byte Reg { get; set; }

        /// <summary>
        /// The value that turns off the backlight
        /// (or reduces it to its minimum brightness).
        /// </summary>
        [XmlElement]
        public byte MinVal { get; set; }

        /// <summary>
        /// The value that sets the keyboard
        /// backlight to the maximum brightness.
        /// </summary>
        [XmlElement]
        public byte MaxVal { get; set; }
    }
}
