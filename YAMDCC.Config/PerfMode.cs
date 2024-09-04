using System.Xml.Serialization;

namespace YAMDCC.Config
{
    public sealed class PerfMode
    {
        /// <summary>
        /// The name of the performance mode.
        /// </summary>
        [XmlElement]
        public string Name { get; set; }

        /// <summary>
        /// The description of the performance mode.
        /// </summary>
        [XmlElement]
        public string Desc { get; set; }

        /// <summary>
        /// The value to write to the EC register
        /// when this performance mode is selected.
        /// </summary>
        [XmlElement]
        public byte Value { get; set; }
    }
}
