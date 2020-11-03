using EVEMon.Common.Data;
using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    public class ContractItem
    {
        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src">The source.</param>
        internal ContractItem(EsiContractItemsListItem src)
        {
            RecordID = src.RecordID;
            Item = StaticItems.GetItemByID(src.TypeID);
            Quantity = src.Quantity;
            RawQuantity = src.RawQuantity;
            Singleton = src.Singleton;
            Included = src.Included;
        }


        #region Properties

        /// <summary>
        /// Gets or sets the record ID.
        /// </summary>
        /// <value>The record ID.</value>
        public long RecordID { get; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>The item.</value>
        public Item Item { get; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public long Quantity { get; }

        /// <summary>
        /// Gets or sets the raw quantity.
        /// </summary>
        /// <value>The raw quantity.</value>
        public int RawQuantity { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContractItem"/> is a singleton.
        /// </summary>
        /// <value><c>true</c> if singleton; otherwise, <c>false</c>.</value>
        public bool Singleton { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ContractItem"/> is included.
        /// </summary>
        /// <value><c>true</c> if included; otherwise, <c>false</c>.</value>
        public bool Included { get; }


        #endregion
    }
}
