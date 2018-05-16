using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi {
    [DataContract]
    public sealed class EsiContactListItem
    {
        [DataMember(Name = "contact_id")]
        public long ContactID { get; set; }
        
        [DataMember(Name = "is_watched")]
        public bool InWatchlist { get; set; }

        [DataMember(Name = "is_blocked")]
        public bool IsBlocked { get; set; }

        [DataMember(Name = "standing")]
        public float Standing { get; set; }

        // One of: character, corporation, alliance, faction
        [DataMember(Name = "contact_type")]
        private string ContactTypeJson
        {
            get
            {
                switch (Group)
                {
                case ContactGroup.Corporate:
                    return "corporation";
                case ContactGroup.Agent:
                    return "faction";
                case ContactGroup.Alliance:
                    return "alliance";
                default:
                    return "character";
                }
            }
            set
            {
                switch (value)
                {
                case "corporation":
                    Group = ContactGroup.Corporate;
                    break;
                case "faction":
                    Group = ContactGroup.Agent;
                    break;
                case "alliance":
                    Group = ContactGroup.Alliance;
                    break;
                case "character":
                    Group = ContactGroup.Personal;
                    break;
                default:
                    break;
                }
            }
        }
        
        // Custom label of the contact
        [DataMember(Name = "label_id")]
        public long LabelID { get; set; }

        [IgnoreDataMember]
        public ContactGroup Group { get; set; }

        public SerializableContactListItem ToXMLItem()
        {
            return new SerializableContactListItem()
            {
                ContactID = ContactID,
                Group = Group,
                InWatchlist = InWatchlist,
                Standing = Standing
            };
        }
    }
}
