using EVEMon.Common.Serialization.Eve;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    [DataContract]
    public sealed class EsiAPICalendarEventAttendees : List<EsiCalendarEventAttendeeListItem>
    {
    }
}
