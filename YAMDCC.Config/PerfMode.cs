using System.Xml.Serialization;

namespace YAMDCC.Config
{
    public class PerfMode
    {
        /// <summary>
        /// The name of the performance mode.
        /// </summary>
        [XmlElement]
        public string Name;

        /// <summary>
        /// The description of the performance mode.
        /// </summary>
        [XmlElement]
        public string Desc;

        /// <summary>
        /// The value to write to the EC register
        /// when this performance mode is selected.
        /// </summary>
        [XmlElement]
        public byte Value;
    }
}
