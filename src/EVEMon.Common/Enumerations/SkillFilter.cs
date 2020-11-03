using System.ComponentModel;

namespace EVEMon.Common.Enumerations
{
    /// <summary>
    /// Enumeration of skill browser filter.
    /// </summary>
    public enum SkillFilter
    {
        [Description("All")]
        All = 0,

        [Description("By Attributes")]
        ByAttributes = 1,

        [Description("Hide Completed")]
        NoLv5 = 2,

        [Description("Known")]
        Known = 3,

        [Description("Level I Ready")]
        Lv1Ready = 4,

        [Description("Not Known")]
        Unknown = 5,

        [Description("Not Known - Owned")]
        UnknownButOwned = 6,

        [Description("Not Known - Trainable")]
        UnknownButTrainable = 7,

        [Description("Not Known - Unowned")]
        UnknownAndNotOwned = 8,

        [Description("Not Known - Untrainable")]
        UnknownAndNotTrainable = 9,

        [Description("Not Planned")]
        NotPlanned = 10,

        [Description("Not Planned - Trainable")]
        NotPlannedButTrainable = 11,

        [Description("Partially Trained")]
        PartiallyTrained = 12,

        [Description("Planned")]
        Planned = 13,

        [Description("Trainable (All)")]
        Trainable = 14,

        [Description("Trainable on alpha account")]
        AlphaFriendly = 15
    }
}
