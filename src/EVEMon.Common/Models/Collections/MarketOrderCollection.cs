using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Settings;

namespace EVEMon.Common.Models.Collections
{
    /// <summary>
    /// A collection of market orders.
    /// </summary>
    public sealed class MarketOrderCollection : ReadonlyCollection<MarketOrder>
    {
        private readonly CCPCharacter m_character;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="character">The character.</param>
        internal MarketOrderCollection(CCPCharacter character)
        {
            m_character = character;
        }

        /// <summary>
        /// Imports an enumeration of serialization objects.
        /// </summary>
        /// <param name="src"></param>
        internal void Import(IEnumerable<SerializableOrderBase> src)
        {
            long id = m_character.CharacterID;
            Items.Clear();
            foreach (SerializableOrderBase srcOrder in src)
            {
                if (srcOrder is SerializableBuyOrder)
                    Items.Add(new BuyOrder(srcOrder) { OwnerID = id });
                else
                    Items.Add(new SellOrder(srcOrder) { OwnerID = id });
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="endedOrders"></param>
        /// <returns>The list of expired orders.</returns>
        internal void Import(IEnumerable<SerializableOrderListItem> src,
            ICollection<MarketOrder> endedOrders)
        {
            var now = DateTime.UtcNow;
            // Mark all orders for deletion 
            // If they are found again on the API feed, they will not be deleted and those set
            // as ignored will be left as ignored
            foreach (MarketOrder order in Items)
                order.MarkedForDeletion = true;
            var newOrders = new LinkedList<MarketOrder>();
            foreach (SerializableOrderListItem srcOrder in src)
            {
                var limit = srcOrder.Issued.AddDays(srcOrder.Duration + MarketOrder.
                    MaxExpirationDays);
                if (limit >= now && !Items.Any(x => x.TryImport(srcOrder, endedOrders)))
                {
                    // New order
                    if (srcOrder.IsBuyOrder != 0)
                    {
                        BuyOrder order = new BuyOrder(srcOrder);
                        if (order.Item != null)
                            newOrders.AddLast(order);
                    }
                    else
                    {
                        SellOrder order = new SellOrder(srcOrder);
                        if (order.Item != null)
                            newOrders.AddLast(order);
                    }
                }
            }
            // Add the items that are no longer marked for deletion
            newOrders.AddRange(Items.Where(x => !x.MarkedForDeletion));
            Items.Clear();
            Items.AddRange(newOrders);
        }

        /// <summary>
        /// Exports only the character issued orders to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used to export only the corporation orders issued by a character.</remarks>
        internal IEnumerable<SerializableOrderBase> ExportOnlyIssuedByCharacter() => Items.
            Where(order => order.OwnerID == m_character.CharacterID).Select(order => order.Export());

        /// <summary>
        /// Exports the orders to a serialization object for the settings file.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Used to export all orders of the collection.</remarks>
        internal IEnumerable<SerializableOrderBase> Export() => Items.Select(order => order.Export());
    }
}
