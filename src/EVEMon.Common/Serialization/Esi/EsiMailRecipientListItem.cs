using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiMailRecipientListItem
    {
        [DataMember(Name = "recipient_id")]
        public long RecipientID { get; set; }

        // One of: alliance, character, corporation, mailing_list
        [DataMember(Name = "recipient_type")]
        public string RecipientType { get; set; }
    }
}
