using System;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public sealed class Agent
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="station">The station.</param>
        /// <param name="src">The source.</param>
        internal Agent(Station station, SerializableAgent src)
        {
            Station = station;
            ID = src.ID;
            Name = src.Name;
            Level = src.Level;
            Quality = src.Quality;
            Division = src.DivisionName;
            AgentType type;
            if (!Enum.TryParse(src.AgentType, out type))
                type = AgentType.NonAgent;
            AgentType = type;
            ResearchSkill = StaticSkills.GetSkillName(src.ResearchSkillID);
            LocatorService = src.LocatorService;
        }

        #endregion


        # region Public Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the agent has locator service.
        /// </summary>
        /// <value><c>true</c> if the agent has locator service; otherwise, <c>false</c>.</value>
        public bool LocatorService { get; private set; }

        /// <summary>
        /// Gets or sets the quality.
        /// </summary>
        /// <value>The quality.</value>
        public int Quality { get; private set; }

        /// <summary>
        /// Gets or sets the research skill.
        /// </summary>
        /// <value>The research skill.</value>
        public string ResearchSkill { get; private set; }

        /// <summary>
        /// Gets or sets the type of the agent.
        /// </summary>
        /// <value>The type of the agent.</value>
        public AgentType AgentType { get; private set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>The division.</value>
        public string Division { get; private set; }

        /// <summary>
        /// Gets or sets the station.
        /// </summary>
        /// <value>The station.</value>
        public Station Station { get; private set; }

        #endregion
    }
}
