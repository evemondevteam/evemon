using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Settings
{
    public sealed class SerializableESIKey
    {
        private readonly Collection<SerializableCharacterIdentity> m_ignoreList;

        public SerializableESIKey()
        {
            m_ignoreList = new Collection<SerializableCharacterIdentity>();
        }

        [XmlAttribute("id")]
        public long ID { get; set; }

        [XmlAttribute("refreshToken")]
        public string RefreshToken { get; set; }

        [XmlAttribute("accessMask")]
        public ulong AccessMask { get; set; }

        [XmlAttribute("monitored")]
        public bool Monitored { get; set; }
    }
}
