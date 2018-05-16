using System.Runtime.Serialization;

namespace EVEMon.Common.Serialization.Esi
{
    /// <summary>
    /// Refers to an x/y/z coordinate within a system.
    /// </summary>
    [DataContract]
    public sealed class EsiPosition
    {
        [IgnoreDataMember]
        public static readonly EsiPosition ORIGIN = new EsiPosition();

        public EsiPosition() : this(0.0, 0.0, 0.0) { }
        public EsiPosition(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [DataMember(Name = "x")]
        public double X { get; set; }

        [DataMember(Name = "y")]
        public double Y { get; set; }

        [DataMember(Name = "z")]
        public double Z { get; set; }
    }
}
