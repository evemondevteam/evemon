using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Eve
{
    /// <summary>
    /// Represents a serializable version of a kill log. Used for querying CCP.
    /// </summary>
    public sealed class SerializableAPIKillLog
    {
        private readonly Collection<SerializableKillLogListItem> m_kills;

        public SerializableAPIKillLog()
        {
            m_kills = new Collection<SerializableKillLogListItem>();
        }

        [XmlArray("ArrayOfEsiKillLogListItem")]
        [XmlArrayItem("EsiKillLogListItem")]
        public Collection<SerializableKillLogListItem> Kills => m_kills;
    }
}
