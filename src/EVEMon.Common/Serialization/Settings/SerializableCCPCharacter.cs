using System.Collections.ObjectModel;
using System.Xml.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Settings
{
    /// <summary>
    /// Represents a serializable character defined from the API
    /// </summary>
    public sealed class SerializableCCPCharacter : SerializableSettingsCharacter
    {
        public SerializableCCPCharacter()
        {
            SkillQueue = new Collection<SerializableQueuedSkill>();
            LastUpdates = new Collection<SerializableAPIUpdate>();
            MarketOrders = new Collection<SerializableOrderBase>();
            Contracts = new Collection<SerializableContract>();
            IndustryJobs = new Collection<SerializableJob>();
        }

		[XmlArray("queue")]
		[XmlArrayItem("skill")]
		public Collection<SerializableQueuedSkill> SkillQueue { get; }

		[XmlArray("marketOrders")]
		[XmlArrayItem("buy", typeof(SerializableBuyOrder))]
		[XmlArrayItem("sell", typeof(SerializableSellOrder))]
		public Collection<SerializableOrderBase> MarketOrders { get; }

		[XmlArray("contracts")]
		[XmlArrayItem("contract")]
		public Collection<SerializableContract> Contracts { get; }

		[XmlArray("industryJobs")]
		[XmlArrayItem("job")]
		public Collection<SerializableJob> IndustryJobs { get; }

		[XmlElement("eveMailMessages")]
        public string EveMailMessagesIDs { get; set; }

        [XmlElement("eveNotifications")]
        public string EveNotificationsIDs { get; set; }

		[XmlArray("lastUpdates")]
		[XmlArrayItem("apiUpdate")]
		public Collection<SerializableAPIUpdate> LastUpdates { get; }
	}
}
