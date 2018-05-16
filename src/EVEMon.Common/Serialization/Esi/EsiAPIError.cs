using EVEMon.Common.Serialization.Eve;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Matches the ESI server response when an error occurs.
    /// </summary>
    [DataContract]
    public class EsiAPIError
    {
        [DataMember(Name = "error", IsRequired = false)]
        public string Error { get; set; }
    }
}
