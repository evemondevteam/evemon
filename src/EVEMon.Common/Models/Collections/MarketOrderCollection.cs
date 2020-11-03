using System;
using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Serialization.Esi;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;

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

        private IssuedFor AdjustIssuer(IssuedFor issuedFor, EsiOrderListItem srcOrder)
        {
            var orderFor = issuedFor;
            const ulong MARKET_ORDER_MASK = (ulong)ESIAPICharacterMethods.MarketOrders;
            // Orders in corporation endpoint are unconditionally for corp, the character
            // endpoint has a special field since *some* are for corp, why...
            if (srcOrder.IsCorporation)
                orderFor = IssuedFor.Corporation;
            // Exclude corporation orders made by a monitored character with ESI market
            // order tracking turned on
            if (issuedFor == IssuedFor.Corporation)
            {
                // Find matching character identity, if any
                var issuer = EveMonClient.CharacterIdentities.FirstOrDefault(character =>
                    character.CharacterID == srcOrder.IssuedBy);
                // If the character is monitored and has access mask to market
                if (issuer != null && (issuer.CCPCharacter?.Monitored ?? false) && issuer.
                        ESIKeys.Any(key => (key.AccessMask & MARKET_ORDER_MASK) != 0UL))
                    orderFor = IssuedFor.None;
            }
            return orderFor;
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
                    Items.Add(new BuyOrder(srcOrder, m_character) { OwnerID = id });
                else
                    Items.Add(new SellOrder(srcOrder, m_character) { OwnerID = id });
            }
        }

        /// <summary>
        /// Imports an enumeration of API objects.
        /// </summary>
        /// <param name="src">The orders to import.</param>
        /// <param name="issuedFor">Whether the orders were issued for a character or
        /// corporation.</param>
        /// <param name="ended">The location to place ended orders.</param>
        /// <returns>The list of expired orders.</returns>
        internal void Import(IEnumerable<EsiOrderListItem> src, IssuedFor issuedFor,
            ICollection<MarketOrder> ended)
        {
            var now = DateTime.UtcNow;
            // Mark all orders for deletion
            // If they are found again on the API feed, they will not be deleted and those set
            // as ignored will be left as ignored
            foreach (var order in Items)
                order.MarkedForDeletion = true;
            var newOrders = new List<MarketOrder>(Items.Count);
            foreach (var srcOrder in src)
            {
                var limit = srcOrder.Issued.AddDays(srcOrder.Duration + MarketOrder.
                    MaxExpirationDays);
                var orderFor = AdjustIssuer(issuedFor, srcOrder);
                if (limit >= now && orderFor != IssuedFor.None && !Items.Any(x => x.TryImport(
                    srcOrder, orderFor, ended)))
                {
                    // New order
                    if (srcOrder.IsBuyOrder)
                    {
                        var order = new BuyOrder(srcOrder, orderFor, m_character);
                        if (order.Item != null)
                            newOrders.Add(order);
                    }
                    else
                    {
                        var order = new SellOrder(srcOrder, orderFor, m_character);
                        if (order.Item != null)
                            newOrders.Add(order);
                    }
                }
            }
            // Add the items that are no longer marked for deletion
            foreach (var order in Items)
                if (order.MarkedForDeletion)
                    ended.Add(order);
                else
                    newOrders.Add(order);
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
