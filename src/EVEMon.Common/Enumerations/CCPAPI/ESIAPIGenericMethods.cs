using System.ComponentModel;
using EVEMon.Common.Attributes;
using EVEMon.Common.Enumerations.UISettings;

namespace EVEMon.Common.Enumerations.CCPAPI
{
    /// <summary>
    /// Enumeration of the ESI API methods. Those methods do not require a token.
    /// Each method should have an entry in APIMethodsEnum and
    /// an equivalent string entry in NetworkConstants indicating the default path of the method.
    /// </summary>
    public enum ESIAPIGenericMethods
    {
        /// <summary>
        /// The EVE server status.
        /// </summary>
        [Header("EVE Server Status")]
        [Description("The status of the EVE server.")]
        [Update(UpdatePeriod.Minutes5, UpdatePeriod.Never, UpdatePeriod.Hours1)]
        ServerStatus,
        
        /// <summary>
        /// Used to convert IDs to Names.
        /// </summary>
        CharacterName,

        /// <summary>
        /// Used to convert Names to IDs.
        /// </summary>
        CharacterID,
        
        /// <summary>
        /// Retrieves information about a conquerable station.
        /// </summary>
        StationInfo,

        /// <summary>
        /// Retrieves information about a citadel.
        /// </summary>
        CitadelInfo,

        /// <summary>
        /// Retrieves information about a planet.
        /// </summary>
        PlanetInfo,

        /// <summary>
        /// List of alliances in EVE.
        /// </summary>
        AllianceList,
        
        /// <summary>
        /// Factional warfare statistics for all EVE.
        /// </summary>
        EVEFactionalWarfareStats,

        /// <summary>
        /// Factional warfare top 100 statistics for all EVE.
        /// </summary>
        EVEFactionalWarfareTopStats,

        /// <summary>
        /// List of factions at war.
        /// </summary>
        FactionWars,

        /// <summary>
        /// List of references of typeIDs.
        /// </summary>
        RefTypes,
        
        /// <summary>
        /// Used to convert typeIDs to typeNames.
        /// </summary>
        TypeName,

        /// <summary>
        /// List of solar systems taking part in Factional Warfare and their occupancy.
        /// </summary>
        FactionalWarfareSystems,

        /// <summary>
        /// List of solar sytems with jump gates.
        /// </summary>
        Jumps,

        /// <summary>
        /// List of solar systems with kills within the last hour.
        /// </summary>
        Kills,

        /// <summary>
        /// List of solar systems that are controlled by a faction or alliance.
        /// </summary>
        Sovereignty,

        /// <summary>
        /// An individual killmail by hash.
        /// </summary>
        KillMail
    }
}
