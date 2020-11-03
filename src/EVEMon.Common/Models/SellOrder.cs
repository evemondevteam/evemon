using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models
{
    /// <summary>
    /// This class represents a sell order.
    /// </summary>
    public sealed class SellOrder : MarketOrder
    {
        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        /// <param name="issuedFor">Whether the order was issued for a corporation or a
        /// character.</param>
        internal SellOrder(EsiOrderListItem src, IssuedFor issuedFor, CCPCharacter character)
            : base(src, issuedFor, character)
        {
        }

        /// <summary>
        /// Constructor from an object deserialized from the settings file.
        /// </summary>
        /// <param name="src"></param>
        internal SellOrder(SerializableOrderBase src, CCPCharacter character)
            : base(src, character)
        {
        }

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        /// <returns></returns>
        public override SerializableOrderBase Export() => Export(new SerializableSellOrder());
    }
}
