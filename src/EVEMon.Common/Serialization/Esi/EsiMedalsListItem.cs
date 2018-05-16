using System;
using EVEMon.Common.Extensions;
using System.Runtime.Serialization;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiMedalsListItem
    {
        private DateTime issued;

        public EsiMedalsListItem()
        {
            issued = DateTime.MinValue;
        }

        [DataMember(Name = "medal_id")]
        public int MedalID { get; set; }

        [DataMember(Name = "reason")]
        public string Reason { get; set; }

        // One of: public, private
        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "issuer_id")]
        public int IssuerID { get; set; }

        [DataMember(Name = "date")]
        public string IssuedJson
        {
            get { return issued.DateTimeToTimeString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    issued = value.TimeStringToDateTime();
            }
        }
        
        [DataMember(Name = "corporation_id")]
        public int CorporationID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        // Medal graphics are not used in EVEMon

        [IgnoreDataMember]
        public DateTime Issued
        {
            get
            {
                return issued;
            }
        }

        public SerializableMedalsListItem ToXMLItem()
        {
            return new SerializableMedalsListItem()
            {
                CorporationID = CorporationID,
                Description = Description,
                IssuerID = IssuerID,
                Issued = Issued,
                MedalID = MedalID,
                Reason = Reason,
                Status = Status,
                Title = Title,
            };
        }
    }
}
