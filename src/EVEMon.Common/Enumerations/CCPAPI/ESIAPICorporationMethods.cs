using System;
using System.ComponentModel;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumeration of the corporation related API methods. Each method has an access mask.
    /// Each method should have an entry in APIMethodsEnum and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    [Flags]
    public enum ESIAPICorporationMethods : ulong
    {
        None = 0,

        /// <summary>
        /// The corporation issued market orders.
        /// </summary>
        [Header("Market Orders")]
        [Description("The corporation market orders of a character.")]
        [Update(UpdatePeriod.Hours1, UpdatePeriod.Hours1, CacheStyle.Long)]
        CorporationMarketOrders = 1 << 1,

        /// <summary>
        /// The corporation issued contracts.
        /// </summary>
        [Header("Contracts")]
        [Description("The corporation contracts of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationContracts = 1 << 2,

        /// <summary>
        /// The corporation issued industry jobs.
        /// </summary>
        [Header("Industry Jobs")]
        [Description("The corporation industry jobs of a character.")]
        [Update(UpdatePeriod.Minutes15, UpdatePeriod.Minutes15, CacheStyle.Long)]
        CorporationIndustryJobs = 1 << 3,

        /// <summary>
        /// A corporation's wallet balances.
        /// </summary>
        CorporationAccountBalance = 1 << 4,

        /// <summary>
        /// Asset list of a corporation.
        /// </summary>
        CorporationAssetList = 1 << 5,

        /// <summary>
        /// A list of corporation contacts.
        /// </summary>
        CorporationContactList = 1 << 6,

        /// <summary>
        /// The log of the corporation's containers.
        /// </summary>
        CorporationContainerLog = 1 << 7,

        /// <summary>
        /// Public corporation info.
        /// </summary>
        CorporationSheet = 1 << 8,

        /// <summary>
        /// Factional warfare statistics for a corporation.
        /// </summary>
        CorporationFactionalWarfareStats = 1 << 9,

        /// <summary>
        /// The Kill log for a corporation (Kill mails).
        /// </summary>
        CorporationKillLog = 1 << 10,

        /// <summary>
        /// List of all medals created by the corporation.
        /// </summary>
        [Header("Medals")]
        [Description("The medals created by a corporation.")]
        [Update(UpdatePeriod.Hours6, UpdatePeriod.Hours6, CacheStyle.Short)]
        CorporationMedals = 1 << 11,

        /// <summary>
        /// List of medals awarded to corporation members.
        /// </summary>
        CorporationMemberMedals = 1 << 12,

        /// <summary>
        /// Member roles and titles.
        /// </summary>
        CorporationMemberSecurity = 1 << 13,

        /// <summary>
        /// Member role and title change log.
        /// </summary>
        CorporationMemberSecurityLog = 1 << 14,

        /// <summary>
        /// Corporation member information.
        /// </summary>
        CorporationMemberTracking = 1 << 15,
        
        /// <summary>
        /// List of all outposts controlled by the corporation.
        /// </summary>
        CorporationOutpostList = 1 << 16,

        /// <summary>
        /// List of all service settings of corporate outposts.
        /// </summary>
        CorporationOutpostServiceDetail = 1 << 17,

        /// <summary>
        /// Shareholders of the corporation.
        /// </summary>
        CorporationShareholders = 1 << 18,

        /// <summary>
        /// NPC Standings towards corporation.
        /// </summary>
        CorporationStandings = 1 << 19,

        /// <summary>
        /// List of all settings of corporate starbases.
        /// </summary>
        CorporationStarbaseDetails = 1 << 20,

        /// <summary>
        /// List of all corporate starbases.
        /// </summary>
        CorporationStarbaseList = 1 << 21,

        /// <summary>
        /// Titles of corporation and the roles they grant.
        /// </summary>
        CorporationTitles = 1 << 22,

        /// <summary>
        /// Wallet journal for all corporate accounts.
        /// </summary>
        CorporationWalletJournal = 1 << 23,

        /// <summary>
        /// Market transactions of all corporate accounts.
        /// </summary>
        CorporationWalletTransactions = 1 << 24,
        
        /// <summary>
        /// List of all corporate bookmarks.
        /// </summary>
        CorporationBookmarks = 1 << 25,

        /// <summary>
        /// List of corporate contract bids.
        /// </summary>
        CorporationContractBids = 1 << 26,

        /// <summary>
        /// List of corporate contract items.
        /// </summary>
        CorporationContractItems = 1 << 27
    }
}
