using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPICalendarEventAttendees : List<EsiCalendarEventAttendeeListItem>
    {
        public SerializableAPICalendarEventAttendees ToXMLItem()
        {
            var ret = new SerializableAPICalendarEventAttendees();
            foreach (var attendee in this)
                ret.EventAttendees.Add(attendee.ToXMLItem());
            return ret;
        }
    }
}
