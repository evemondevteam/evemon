using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Datafiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EVEMon.Common.Data
{
    /// <summary>
    /// Base class for ships, items, implants.
    /// </summary>
    public class Item
    {
        private readonly FastList<StaticSkillLevel> m_prerequisites;
        private static Item s_unknownItem;


        #region Constructors

        /// <summary>
        /// Constructor for an unknown item.
        /// </summary>
        private Item()
            : this(int.MaxValue, EveMonConstants.UnknownText)
        {
        }

        /// <summary>
        /// Base constructor for default items.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        internal Item(int id, string name)
        {
            ID = id;
            Name = name;
            Description = "No description.";

            m_prerequisites = new FastList<StaticSkillLevel>(0);
        }

        /// <summary>
        /// Base constructor for blueprints.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="src">The source.</param>
        internal Item(MarketGroup group, SerializableBlueprint src)
            : this(src.ID, src.Name)
        {
            Icon = src.Icon;
            MetaGroup = src.MetaGroup;
            MarketGroup = group;
            Family = ItemFamily.Blueprint;

            // Skills prerequisites
            m_prerequisites = new FastList<StaticSkillLevel>(src.PrereqSkill?.Count ?? 0);
            if (src.PrereqSkill == null)
                return;

            foreach (SerializablePrereqSkill prereq in src.PrereqSkill)
            {
                m_prerequisites.Add(new StaticSkillLevel(prereq.ID, prereq.Level, prereq.Activity));
            }
        }

        /// <summary>
        /// Base deserialization constructor.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="src">The source.</param>
        internal Item(MarketGroup group, SerializableItem src)
            : this(src.ID, src.Name)
        {
            MarketGroup = group;
            Icon = src.Icon;
            Race = src.Race;
            FittingSlot = src.Slot == ItemSlot.None ? ItemSlot.NoSlot : src.Slot;
            Family = src.Family;
            Description = src.Description;
            CategoryName = src.Category;
            GroupName = src.Group;

            PortionSize = src.PortionSize;
            MetaLevel = src.MetaLevel;
            MetaGroup = src.MetaGroup;

            Properties = new EvePropertyCollection(src.Properties);
            ReactionMaterial = new ReactionMaterialCollection(src.ReactionInfo);
            ControlTowerFuel = new ControlTowerFuelCollection(src.ControlTowerFuelInfo);

            // Skills prerequisites
            m_prerequisites = new FastList<StaticSkillLevel>(src.PrerequisiteSkills?.Count ?? 0);
            if (src.PrerequisiteSkills == null)
                return;

            foreach (SerializablePrerequisiteSkill prereq in src.PrerequisiteSkills)
            {
                m_prerequisites.Add(new StaticSkillLevel(prereq.ID, prereq.Level));
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets this object's ID.
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Gets this object's icon.
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// Gets this object's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the item's family.
        /// </summary>
        public ItemFamily Family { get; }

        /// <summary>
        /// Gets the race this object is bound to.
        /// </summary>
        public Race Race { get; }

        /// <summary>
        /// Gets this object's portion size.
        /// </summary>
        public int PortionSize { get; }

        /// <summary>
        /// Gets this object's description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the metalevel this item belong to.
        /// </summary>
        public long MetaLevel { get; }

        /// <summary>
        /// Gets the category this item belong to.
        /// </summary>
        public string CategoryName { get; }

        /// <summary>
        /// Gets the group this item belong to.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Gets the metagroup this item belong to.
        /// </summary>
        public ItemMetaGroup MetaGroup { get; }

        /// <summary>
        /// Gets the market group this item belong to.
        /// </summary>
        public MarketGroup MarketGroup { get; }

        /// <summary>
        /// Gets the slot this items fit to.
        /// </summary>
        public ItemSlot FittingSlot { get; }

        /// <summary>
        /// Gets the collection of properties of this object.
        /// </summary>
        public EvePropertyCollection Properties { get; }

        /// <summary>
        /// Gets the collection of reaction info of this object.
        /// </summary>
        public ReactionMaterialCollection ReactionMaterial { get; }

        /// <summary>
        /// Gets the collection of control tower fuel info of this object.
        /// </summary>
        public ControlTowerFuelCollection ControlTowerFuel { get; }

        /// <summary>
        /// Gets the collection of skills this object must satisfy to be used.
        /// </summary>
        public IEnumerable<StaticSkillLevel> Prerequisites => m_prerequisites;

        /// <summary>
        /// Gets the reprocessing materials and their quantities.
        /// </summary>
        public IEnumerable<Material> ReprocessingMaterials => StaticReprocessing.GetItemMaterialsByID(ID);

        /// <summary>
        /// Gets the skill used to reprocess those items.
        /// </summary>
        public StaticSkill ReprocessingSkill
        {
            get
            {
                EvePropertyValue? property = Properties[DBConstants.ReprocessingSkillPropertyID];

                // Returns scrap metal processing by default
                if (property == null)
                    return StaticSkills.GetSkillByID(DBConstants.ScrapMetalProcessingSkillID);

                // Returns the reprocessing skill specified by the property
                long id = property.Value.Int64Value;
                return StaticSkills.GetSkillByID(id);
            }
        }

        /// <summary>
        /// Gets the category path of this item.
        /// </summary>
        /// <returns></returns>
        public string CategoryPath
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                MarketGroup cat = MarketGroup;

                while (cat != null)
                {
                    sb.Insert(0, cat.Name);
                    cat = cat.ParentGroup;
                    if (cat != null)
                        sb.Insert(0, " > ");
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the unknown item.
        /// </summary>
        /// <value>
        /// The unknown item.
        /// </value>
        public static Item UnknownItem => s_unknownItem ?? (s_unknownItem = new Item());

        #endregion


        #region Public Methods

        /// <summary>
        ///   Evaluates whether this item can be activated if the given CPU
        ///   and/or PowerGrid resources are available. Either argument for this
        ///   method can be null, which is interpreted as "no upper limit".
        ///   If the CPU and/or Grid requirements for this item are unknown,
        ///   the method returns false.
        /// </summary>
        /// <param name="cpuAvailable">The upper limit for CPU usage, or null for unlimited.</param>
        /// <param name="gridAvailable">The upper limit for Grid usage or null for unlimited.</param>
        /// <returns>true if both the CPU and powergrid requirements of this
        /// item lie between 0.0d and the given bounds.</returns>
        public bool CanActivate(double? cpuAvailable, double? gridAvailable)
        {
            // There are no limits, so anything fits
            if (cpuAvailable == null && gridAvailable == null)
                return true;

            if (FittingSlot == ItemSlot.None)
                return false;

            // If we have a slot index, we're a fittable item
            // Now see if we can find our usage numbers
            string cpuUsage = FindProperty(EveProperty.CPU, null);
            string gridUsage = FindProperty(EveProperty.Powergrid, null);

            double? cpuRequired = TryParseNullable(TryStripTail(cpuUsage, " tf"));
            double? gridRequired = TryParseNullable(TryStripTail(gridUsage, " MW"));

            if (cpuRequired == null && gridRequired == null)
                return false;

            //We have information about this item, see if it fits
            bool fits = true;
            if (cpuAvailable != null)
                fits &= cpuRequired <= cpuAvailable;

            if (gridAvailable != null)
                fits &= gridRequired <= gridAvailable;

            return fits;

            // We lack information about this item, or this item isn't fittable
            // Return false as specced in the method docs
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Tries to strip the given tail from the end of some string.
        /// </summary>
        /// <param name="stripMe">The string to evaluate</param>
        /// <param name="tail">The &quot;tail&quot; to try and remove</param>
        /// <returns>null if stripMe is null, stripMe if tail is null or stripMe doesn't
        /// end in tail, stripMe-with-tail-removed otherwise.</returns>
        private static string TryStripTail(string stripMe, string tail)
        {
            if (stripMe == null)
                return null;

            if (tail == null)
                return stripMe;

            return stripMe.EndsWith(tail, StringComparison.CurrentCulture) ? stripMe.Remove(
                stripMe.Length - tail.Length) : stripMe;
        }

        /// <summary>
        /// Try to parse a String as a double. Returns null for any kind of
        /// invalid input.
        /// </summary>
        /// <param name="parseMe">The string to try and parse.</param>
        /// <returns>The string as double, or null if failed to parse.</returns>
        private static double? TryParseNullable(string parseMe)
        {
            double? result = null;
            double tempValue;
            if (parseMe.TryParseInv(out tempValue))
                result = tempValue;
            return result;
        }

        /// <summary>
        /// Searches _properties for a property with the given property name and
        /// returns its value. If the property isn't found, it returns the given
        /// default value. If the property occurs more than once, only the first
        /// occurance is considered.
        /// </summary>
        /// <param name="property">The property name to look for.</param>
        /// <param name="defaultValue">The value to return if the property isn't found.</param>
        /// <returns>Either the value of the named property, or the given default value.</returns>
        private string FindProperty(EveProperty property, string defaultValue)
        {
            string result = defaultValue;
            foreach (EvePropertyValue prop in Properties)
            {
                if (prop.Property != property)
                    continue;

                result = prop.Value;
                break;
            }
            return result;
        }

        #endregion


        #region Overridden Methods

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>Name of the Item</returns>
        public override string ToString() => Name;

        #endregion
    }
}
