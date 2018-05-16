using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Represents a serializable version of a corporation's sheet. Used for querying CCP.
    /// </summary>
    [DataContract]
    public sealed class EsiAPICorporationSheet
    {
        private DateTime founded;

        public EsiAPICorporationSheet()
        {
            founded = DateTime.MinValue;
        }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "ticker")]
        public string Ticker { get; set; }

        [DataMember(Name = "ceo_id")]
        public long CeoID { get; set; }

        [DataMember(Name = "creator_id")]
        public long CreatorID { get; set; }

        [DataMember(Name = "home_station_id")]
        public int HQStationID { get; set; }

        [DataMember(Name = "date_founded", EmitDefaultValue = false, IsRequired = false)]
        private string DateFoundedJson
        {
            get
            {
                return founded.DateTimeToTimeString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    founded = value.TimeStringToDateTime();
            }
        }

        [IgnoreDataMember]
        public DateTime DateFounded
        {
            get
            {
                return founded;
            }
        }

        [DataMember(Name = "description", EmitDefaultValue = false, IsRequired = false)]
        public string Description { get; set; }

        [DataMember(Name = "url", EmitDefaultValue = false, IsRequired = false)]
        public string WebUrl { get; set; }

        [DataMember(Name = "alliance_id", EmitDefaultValue = false, IsRequired = false)]
        public int AllianceID { get; set; }
        
        [DataMember(Name = "faction_id", EmitDefaultValue = false, IsRequired = false)]
        public int FactionID { get; set; }
        
        [DataMember(Name = "tax_rate")]
        public float TaxRate { get; set; }

        [DataMember(Name = "member_count")]
        public int MemberCount { get; set; }

        [DataMember(Name = "shares", EmitDefaultValue = false, IsRequired = false)]
        public int Shares { get; set; }

        public SerializableAPICorporationSheet ToXMLItem(long id, EsiAPIDivisions divisions)
        {
            var ret = new SerializableAPICorporationSheet()
            {
                AllianceID = AllianceID,
                CeoID = CeoID,
                Description = Description,
                FactionID = FactionID,
                HQStationID = HQStationID,
                ID = id,
                MemberCount = MemberCount,
                // MemberLimit not supplied by ESI
                Name = Name,
                Shares = Shares,
                TaxRate = TaxRate,
                Ticker = Ticker,
                WebUrl = WebUrl
            };

            // Wallet divisions
            foreach (var division in divisions.Wallet)
                ret.WalletDivisions.Add(division.ToXMLWalletItem());

            // Hangar divisions
            foreach (var division in divisions.Hangar)
                ret.Divisions.Add(division.ToXMLHangarItem());

            return ret;
        }
    }
}
