using System.Collections.Generic;
using System.Linq;
using EVEMon.Common.Collections;
using EVEMon.Common.Serialization.Datafiles;

namespace EVEMon.Common.Data
{
    public class Blueprint : Item
    {
        private readonly Dictionary<int, decimal> m_inventBlueprints;
        private readonly FastList<StaticRequiredMaterial> m_materialRequirements;


        #region Constructors

        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="group">Market Group the Blueprint will be a member of.</param>
        /// <param name="src">Source serializable blueprint.</param>
        internal Blueprint(MarketGroup group, SerializableBlueprint src)
            : base(group, src)
        {
            RunsPerCopy = src.MaxProductionLimit;
            ProducesItem = StaticItems.GetItemByID(src.ProduceItemID);
            ProductionTime = src.ProductionTime;
            ResearchCopyTime = src.ResearchCopyTime;
            ResearchMaterialTime = src.ResearchMaterialTime;
            ResearchProductivityTime = src.ResearchProductivityTime;
            ResearchInventionTime = src.InventionTime;
            ReverseEngineeringTime = src.ReverseEngineeringTime;
            ReactionTime = src.ReactionTime;
            if (src.ReactionOutcome != null)
                ReactionOutcome = new Material(src.ReactionOutcome);

            // Invented blueprints
            m_inventBlueprints = new Dictionary<int, decimal>(src.InventionTypeIDs?.Count ?? 0);
            if (src.InventionTypeIDs != null && src.InventionTypeIDs.Any())
            {
                m_inventBlueprints.AddRange(src.InventionTypeIDs);
            }

            // Materials prerequisites
            m_materialRequirements =
                new FastList<StaticRequiredMaterial>(src.ReqMaterial?.Count ?? 0);
            if (src.ReqMaterial == null)
                return;

            foreach (SerializableRequiredMaterial prereq in src.ReqMaterial)
            {
                m_materialRequirements.Add(new StaticRequiredMaterial(prereq));
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the item this blueprint produces.
        /// </summary>
        public Item ProducesItem { get; private set; }

        /// <summary>
        /// Gets the maximum items this blueprint can produce.
        /// </summary>
        public int RunsPerCopy { get; private set; }

        /// <summary>
        /// Gets the production time of the blueprint.
        /// </summary>
        public double ProductionTime { get; private set; }

        /// <summary>
        /// Gets the copying time of the blueprint.
        /// </summary>
        public double ResearchCopyTime { get; private set; }

        /// <summary>
        /// Gets the material efficiency research time of the blueprint.
        /// </summary>
        public double ResearchMaterialTime { get; private set; }

        /// <summary>
        /// Gets the productivity efficiency research time of the blueprint.
        /// </summary>
        public double ResearchProductivityTime { get; private set; }

        /// <summary>
        /// Gets the invention time of the blueprint.
        /// </summary>
        public double ResearchInventionTime { get; private set; }

        /// <summary>
        /// Gets the reverse engineering time.
        /// </summary>
        public double ReverseEngineeringTime { get; private set; }

        /// <summary>
        /// Gets the reaction time.
        /// </summary>
        public double ReactionTime { get; private set; }

        /// <summary>
        /// Gets the reaction outcome material.
        /// </summary>
        public Material ReactionOutcome { get; private set; }

        /// <summary>
        /// Gets the collection of materials this blueprint must satisfy to be build.
        /// </summary>
        public IEnumerable<StaticRequiredMaterial> MaterialRequirements => m_materialRequirements;

        /// <summary>
        /// Gets the collection of blueprints this object can invent.
        /// </summary>
        public IEnumerable<KeyValuePair<Blueprint, decimal>> InventBlueprints
            => m_inventBlueprints
                .Select(inventBlueprint => new KeyValuePair<Blueprint, decimal>(
                    StaticBlueprints.GetBlueprintByID(inventBlueprint.Key), inventBlueprint.Value));

        #endregion
    }
}
