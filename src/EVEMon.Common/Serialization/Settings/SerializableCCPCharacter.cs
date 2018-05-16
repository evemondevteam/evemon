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
        private readonly Collection<SerializableQueuedSkill> m_skillQueue;
        private readonly Collection<SerializableAPIUpdate> m_lastUpdates;
        private readonly Collection<SerializableOrderBase> m_marketOrders;
        private readonly Collection<SerializableContract> m_contracts;
        private readonly Collection<SerializableContractBid> m_contractBids;
        private readonly Collection<SerializableJob> m_industryJobs;

        public SerializableCCPCharacter()
        {
            m_skillQueue = new Collection<SerializableQueuedSkill>();
            m_lastUpdates = new Collection<SerializableAPIUpdate>();
            m_marketOrders = new Collection<SerializableOrderBase>();
            m_contracts = new Collection<SerializableContract>();
            m_contractBids = new Collection<SerializableContractBid>();
            m_industryJobs = new Collection<SerializableJob>();
        }

        [XmlArray("queue")]
        [XmlArrayItem("skill")]
        public Collection<SerializableQueuedSkill> SkillQueue => m_skillQueue;

        [XmlArray("marketOrders")]
        [XmlArrayItem("buy", typeof(SerializableBuyOrder))]
        [XmlArrayItem("sell", typeof(SerializableSellOrder))]
        public Collection<SerializableOrderBase> MarketOrders => m_marketOrders;

        [XmlArray("contracts")]
        [XmlArrayItem("contract")]
        public Collection<SerializableContract> Contracts => m_contracts;

        [XmlArray("industryJobs")]
        [XmlArrayItem("job")]
        public Collection<SerializableJob> IndustryJobs => m_industryJobs;

        [XmlElement("eveMailMessages")]
        public string EveMailMessagesIDs { get; set; }

        [XmlElement("eveNotifications")]
        public string EveNotificationsIDs { get; set; }

        [XmlArray("lastUpdates")]
        [XmlArrayItem("apiUpdate")]
        public Collection<SerializableAPIUpdate> LastUpdates => m_lastUpdates;
    }
}
