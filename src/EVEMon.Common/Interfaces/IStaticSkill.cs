using System;
using System.Collections.ObjectModel;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Models;

namespace EVEMon.Common.Interfaces
{
    public interface IStaticSkill
    {
        int ID { get; }
        int ArrayIndex { get; }
        string Name { get; }

        long Rank { get; }
        long Cost { get; }
        StaticSkillGroup Group { get; }

        Collection<StaticSkillLevel> Prerequisites { get; }

        EveAttribute PrimaryAttribute { get; }
        EveAttribute SecondaryAttribute { get; }

        Skill ToCharacter(Character character);
    }
}