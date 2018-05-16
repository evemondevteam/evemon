using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiCalendarEventAttendeeListItem
    {
        [DataMember(Name = "character_id", EmitDefaultValue = false, IsRequired = false)]
        public long CharacterID { get; set; }
        
        // One of: declined, not_responded, accepted, tentative
        [DataMember(Name = "event_response", EmitDefaultValue = false, IsRequired = false)]
        public string Response { get; set; }

        public SerializableCalendarEventAttendeeListItem ToXMLItem()
        {
            return new SerializableCalendarEventAttendeeListItem()
            {
                CharacterID = CharacterID,
                Response = Response
            };
        }
    }
}
