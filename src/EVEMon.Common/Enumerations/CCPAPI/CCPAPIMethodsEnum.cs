using System;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumerations to support APIMethodsEnum.
    /// </summary>
    [Flags]
    public enum CCPAPIMethodsEnum : ulong
    {
        None = 0,

        /// <summary>
        /// The basic character features of APIMethodsEnum.
        /// </summary>
        BasicCharacterFeatures = ESIAPICharacterMethods.AccountBalance |
            ESIAPICharacterMethods.Attributes | ESIAPICharacterMethods.CharacterSheet |
            ESIAPICharacterMethods.Clones | ESIAPICharacterMethods.EmploymentHistory |
            ESIAPICharacterMethods.Location | ESIAPICharacterMethods.Implants |
            ESIAPICharacterMethods.Ship | ESIAPICharacterMethods.SkillQueue |
            ESIAPICharacterMethods.Skills,

        /// <summary>
        /// The advanced character features of APIMethodsEnum.
        /// </summary>
        AdvancedCharacterFeatures = ESIAPICharacterMethods.AssetList |
            ESIAPICharacterMethods.CalendarEventAttendees | ESIAPICharacterMethods.ContactList |
            ESIAPICharacterMethods.Contracts | ESIAPICharacterMethods.ContractBids |
            ESIAPICharacterMethods.ContractItems | ESIAPICharacterMethods.FactionalWarfareStats |
            ESIAPICharacterMethods.IndustryJobs | ESIAPICharacterMethods.JumpFatigue |
            ESIAPICharacterMethods.KillLog | ESIAPICharacterMethods.MailMessages |
            ESIAPICharacterMethods.MailBodies | ESIAPICharacterMethods.MailingLists |
            ESIAPICharacterMethods.MarketOrders | ESIAPICharacterMethods.Medals |
            ESIAPICharacterMethods.Notifications | ESIAPICharacterMethods.PlanetaryColonies |
            ESIAPICharacterMethods.PlanetaryLayout | ESIAPICharacterMethods.ResearchPoints |
            ESIAPICharacterMethods.Standings | ESIAPICharacterMethods.UpcomingCalendarEvents |
            ESIAPICharacterMethods.UpcomingCalendarEventDetails |
            ESIAPICharacterMethods.WalletJournal | ESIAPICharacterMethods.WalletTransactions |
            ESIAPICharacterMethods.CitadelInfo | ESIAPICharacterMethods.LoyaltyPoints |
            ESIAPICharacterMethods.MarketOrdersHistory,

        /// <summary>
        /// The advanced corporation features of APIMethodsEnum.
        /// </summary>
        AdvancedCorporationFeatures = ESIAPICorporationMethods.CorporationContracts |
            ESIAPICorporationMethods.CorporationMedals |
            ESIAPICorporationMethods.CorporationMarketOrders |
            ESIAPICorporationMethods.CorporationIndustryJobs |
            ESIAPICorporationMethods.CorporationMarketOrdersHistory,

        /// <summary>
        /// All character features of APIMethodsEnum
        /// </summary>
        AllCharacterFeatures = BasicCharacterFeatures | AdvancedCharacterFeatures,
    }
}
