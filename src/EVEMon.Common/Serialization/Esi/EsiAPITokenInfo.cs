using EVEMon.Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Template class for an ESI character response.
    /// </summary>
    [DataContract]
    public sealed class EsiAPITokenInfo : IComparable<EsiAPITokenInfo>
    {
        // Character ID #
        [DataMember(Name = "CharacterID")]
        public long CharacterID { get; set; }
        // Character name
        [DataMember(Name = "CharacterName")]
        public string CharacterName { get; set; }
        // Date the token expires (JSON format)
        [DataMember(Name = "ExpiresOn")]
        public string ExpiresOnString
        {
            get
            {
                // Report as "2016-05-31T04:02:16Z"
                return expires.DateTimeToTimeString();
            }
            set
            {
                // Parse from "2016-05-31T04:02:16Z"
                if (!string.IsNullOrEmpty(value))
                    expires = value.TimeStringToDateTime();
            }
        }
        // Scopes authorized
        [DataMember(Name = "Scopes")]
        private string Scopes { get; set; }

        // When this token expires
        private DateTime expires;

        public EsiAPITokenInfo()
        {
            expires = DateTime.UtcNow;
        }
        public int CompareTo(EsiAPITokenInfo other)
        {
            if (other == null)
                throw new ArgumentNullException("other");
            return CharacterID.CompareTo(other.CharacterID);
        }
        public override bool Equals(object obj)
        {
            var other = obj as EsiAPITokenInfo;
            return other != null && CompareTo(other) == 0;
        }
        public override int GetHashCode()
        {
            return CharacterID.GetHashCode();
        }
        public override string ToString()
        {
            return CharacterName;
        }
    }
}
