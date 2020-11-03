using System;
using EVEMon.Common.Data;

namespace EVEMon.Common.Interfaces
{
    /// <summary>
    /// Represents a static skill and level tuple
    /// </summary>
    public interface ISkillLevel
    {
        long Level { get; }
        StaticSkill Skill { get; }
    }
}