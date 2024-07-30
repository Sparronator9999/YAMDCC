using System.Xml.Serialization;

namespace YAMDCC.Config
{
    public class KeySwapConf
    {
        /// <summary>
        /// The register that controls the Win/Fn key swap state.
        /// </summary>
        [XmlElement]
        public byte Reg;

        /// <summary>
        /// Is the Win/Fn key swap feature enabled?
        /// </summary>
        [XmlElement]
        public bool Enabled;

        /// <summary>
        /// The value to turn on Win/Fn key swapping.
        /// </summary>
        [XmlElement]
        public byte OnVal;

        /// <summary>
        /// The value to turn off Win/Fn key swapping.
        /// </summary>
        [XmlElement]
        public byte OffVal;
    }
}
