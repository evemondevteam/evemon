using System;
using System.Runtime.Serialization;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Serialization.FittingClf
{
    [DataContract]
    public sealed class SerializableClfFittingMetaData
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "creationdate")]
        public string CreationDateJson
        {
            get { return CreationDate.DateTimeToTimeString("ddd, dd MMM yyyy HH:mm:ss +0000"); }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    CreationDate = value.TimeStringToDateTime();
            }
        }

        public DateTime CreationDate { get; set; }
    }
}