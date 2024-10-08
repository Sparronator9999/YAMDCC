using System.Xml.Serialization;

namespace YAMDCC.Config
{
    public sealed class KeySwapConf
    {
        /// <summary>
        /// The register that controls the Win/Fn key swap state.
        /// </summary>
        [XmlElement]
        public byte Reg { get; set; }

        /// <summary>
        /// Is the Win/Fn key swap feature enabled?
        /// </summary>
        [XmlElement]
        public bool Enabled { get; set; }

        /// <summary>
        /// The value to turn on Win/Fn key swapping.
        /// </summary>
        [XmlElement]
        public byte OnVal { get; set; }

        /// <summary>
        /// The value to turn off Win/Fn key swapping.
        /// </summary>
        [XmlElement]
        public byte OffVal { get; set; }
    }
}
