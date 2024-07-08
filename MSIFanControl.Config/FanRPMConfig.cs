using System;
using System.Xml.Serialization;

namespace MSIFanControl.Config
{
    [Serializable]
    public class FanRPMConfig
    {
        /// <summary>
        /// The register to read to get the fan RPM.
        /// </summary>
        [XmlElement]
        public byte ReadReg;

        /// <summary>
        /// Is the RPM value stored as a word (16-bit) or byte (8-bit)?
        /// </summary>
        [XmlElement]
        public bool Is16Bit;

        /// <summary>
        /// Is the RPM value big-endian? This will only have an
        /// effect if <see cref="Is16Bit"/> is set to <c>true</c>.
        /// </summary>
        [XmlElement]
        public bool IsBigEndian;

        /// <summary>
        /// The value to multiply (or divide, if <see cref="DivideByMult"/>
        /// is <c>true</c>) the read RPM value by.
        /// </summary>
        [XmlElement]
        public int Multiplier = 1;

        /// <summary>
        /// If <c>true</c>, divides the read RPM value by
        /// <see cref="Multiplier"/> instead of multiplying.
        /// </summary>
        [XmlElement]
        public bool DivideByMult;

        /// <summary>
        /// Set to true if the read RPM value starts high
        /// and decreases as the fan speed increases.
        /// </summary>
        [XmlElement]
        public bool Invert;
    }
}
