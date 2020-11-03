using System.ComponentModel;

namespace EVEMon.Common.Enumerations
{
    /// <summary>
    /// Represents the activity of a blueprint.
    /// </summary>
    public enum BlueprintActivity
    {
        [Description("None")]
        None = 0,

        [Description("Manufacturing")]
        Manufacturing = 1,

        [Description("Researching Technology")]
        ResearchingTechnology = 2,

        [Description("Time Efficiency Research")]
        ResearchingTimeEfficiency = 3,

        [Description("Material Efficiency Research")]
        ResearchingMaterialEfficiency = 4,

        [Description("Copying")]
        Copying = 5,

        [Description("Duplicating")]
        Duplicating = 6,

        [Description("Reverse Engineering")]
        ReverseEngineering = 7,

        [Description("Invention")]
        Invention = 8,

        [Description("Simple Reactions")]
        SimpleReactions = 9,

        [Description("Reactions")]
        Reactions = 11
    }
}
