using EVEMon.Common.Enumerations.CCPAPI;
using System;
using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Base for classes which have a location and location type. Can be instantiated on its
    /// own or inherited.
    /// </summary>
    [DataContract]
    public class EsiLocationBase
    {
        private CCPAPILocationType locationType;

        public EsiLocationBase()
        {
            locationType = CCPAPILocationType.Other;
        }

        [DataMember(Name = "location_id")]
        public long LocationID { get; set; }

        // One of "station", "solar_system", "other"
        [DataMember(Name = "location_type")]
        private string LocationTypeJson
        {
            get
            {
                return locationType.ToString().ToLower();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Enum.TryParse(value, true, out locationType);
            }
        }

        [IgnoreDataMember]
        public CCPAPILocationType LocationType
        {
            get
            {
                return locationType;
            }
        }
    }
}
