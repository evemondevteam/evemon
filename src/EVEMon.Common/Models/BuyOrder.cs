using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// This class represents a buy order.
    /// </summary>
    public sealed class BuyOrder : MarketOrder
    {
        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="issuedFor">Whether the order was issued for a corporation or a
        /// character.</param>
        internal BuyOrder(EsiOrderListItem src, IssuedFor issuedFor, CCPCharacter character)
            : base(src, issuedFor, character)
        {
            Escrow = src.Escrow;
            Range = src.Range;
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        internal BuyOrder(SerializableOrderBase src, CCPCharacter character)
            : base(src, character)
        {
        }

        /// <summary>
        /// Gets the amount currently invested in escrow.
        /// </summary>
        public decimal Escrow { get; internal set; }

        /// <summary>
        /// Gets the range of this order.
        /// </summary>
        public int Range { get; internal set; }

        /// <summary>
        /// Gets the description of the range.
        /// </summary>
        public string RangeDescription
        {
            get
            {
                switch (Range)
                {
                    case -1:
                        return "Station";
                    case 0:
                        return "Solar System";
                    case 1:
                        return $"{Range} jump";
                    case EveConstants.RegionRange:
                        return "Region";
                    default:
                        return $"{Range} jumps";
                }
            }
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        public override SerializableOrderBase Export() => Export(new SerializableBuyOrder());
    }
}
