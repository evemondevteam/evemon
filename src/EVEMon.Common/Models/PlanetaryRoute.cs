using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    public sealed class PlanetaryRoute
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PlanetaryRoute"/> class.
        /// </summary>
        /// <param name="colony">The colony.</param>
        /// <param name="src">The source.</param>
        internal PlanetaryRoute(PlanetaryColony colony, EsiPlanetaryRoute src)
        {
            Colony = colony;
            ID = src.RouteID;
            SourcePinID = src.SourcePinID;
            DestinationPinID = src.DestinationPinID;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the colony.
        /// </summary>
        /// <value>
        /// The colony.
        /// </value>
        public PlanetaryColony Colony { get; private set; }

        /// <summary>
        /// Gets or sets the route identifier.
        /// </summary>
        /// <value>
        /// The route identifier.
        /// </value>
        public long ID { get; private set; }

        /// <summary>
        /// Gets or sets the source pin identifier.
        /// </summary>
        /// <value>
        /// The source pin identifier.
        /// </value>
        public long SourcePinID { get; private set; }

        /// <summary>
        /// Gets or sets the destination pin identifier.
        /// </summary>
        /// <value>
        /// The destination pin identifier.
        /// </value>
        public long DestinationPinID { get; private set; }

        #endregion

    }
}
