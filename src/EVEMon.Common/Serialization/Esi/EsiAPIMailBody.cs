using EVEMon.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Would inherit from SerializableMailMessagesListItem, but 'is_read' is changed to
    /// 'read'. CCPls, had to make an intermediate base instead
    /// </summary>
    [DataContract]
    public sealed class EsiAPIMailBody : EsiMailBase
    {
        [DataMember(Name = "body", EmitDefaultValue = false, IsRequired = false)]
        public string Body { get; set; }

        [DataMember(Name = "read", IsRequired = false)]
        public bool Read { get; set; }
    }
}
