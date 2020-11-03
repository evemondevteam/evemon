using EVEMon.Common.Serialization.Eve;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace EVEMon.Common.Serialization.Datafiles
{
    /// <summary>
    /// Represents a serializable version of the stations list. Used for data files only.
    /// </summary>
    public sealed class SerializableStationList
    {
        private readonly Collection<SerializableOutpost> m_stations;

        public SerializableStationList()
        {
            m_stations = new Collection<SerializableOutpost>();
        }

        [XmlArray("stations")]
        [XmlArrayItem("station")]
        public Collection<SerializableOutpost> Stations => m_stations;
    }
}
