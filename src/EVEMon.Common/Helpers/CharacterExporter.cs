using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using EVEMon.Common.Collections;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Enumerations.CCPAPI;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Serialization.Exportation;
using EVEMon.Common.Serialization.Settings;
using EVEMon.Common.Service;

namespace EVEMon.Common.Helpers
{
    /// <summary>
    /// Provides exportation under different formats for characters.
    /// </summary>
    public static class CharacterExporter
    {
        /// <summary>
        /// Creates a TXT format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsText(Character character, Plan plan)
        {
            const string Separator = "=======================================================================";
            const string SubSeparator = "-----------------------------------------------------------------------";

            StringBuilder builder = new StringBuilder();
            builder
                .AppendLine("BASIC INFO")
                .AppendLine(Separator)
                .AppendLine($"     Name: {character.Name}")
                .AppendLine(FormattableString.Invariant($"  Balance: {character.Balance:N2} ISK"))
                .AppendLine($" Birthday: {character.Birthday.DateTimeToTimeString()} UTC")
                .AppendLine($"   Gender: {character.Gender}")
                .AppendLine($"     Race: {character.Race}")
                .AppendLine($"Bloodline: {character.Bloodline}")
                .AppendLine($" Ancestry: {character.Ancestry}")
                .AppendLine()
                .AppendLine("ATTRIBUTES")
                .AppendLine(Separator)
                .AppendLine($"Intelligence: {character.Intelligence.EffectiveValue}")
                .AppendLine($"    Charisma: {character.Charisma.EffectiveValue}")
                .AppendLine($"  Perception: {character.Perception.EffectiveValue}")
                .AppendLine($"      Memory: {character.Memory.EffectiveValue}")
                .AppendLine($"   Willpower: {character.Willpower.EffectiveValue}")
                .AppendLine();

            // Implants
            IList<Implant> implants = character.CurrentImplants.ToList();
            if (implants.Any())
            {
                builder.AppendLine("AUGMENTATIONS");
                builder.AppendLine(Separator);
                foreach (Implant implant in implants)
                {
                    builder.AppendLine($"+{implant.Bonus} {implant.Slot.GetDescription().PadRight(22)} : {implant.Name}");
                }
                builder.AppendLine();
            }

            // Skill groups
            builder.AppendLine("SKILLS");
            builder.AppendLine(Separator);
            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                AddSkillGroups(character, plan, builder, skillGroup);

                builder.AppendLine(SubSeparator);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Adds the skill groups.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="skillGroup">The skill group.</param>
        private static void AddSkillGroups(Character character, Plan plan, StringBuilder builder, SkillGroup skillGroup)
        {
            int count = skillGroup
                .Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x)))
                .Select(x => GetMergedSkill(plan, x))
                .Count();

            long skillGroupTotalSP = skillGroup
                .Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x)))
                .Select(x => GetMergedSkill(plan, x))
                .Sum(x => x.Skillpoints);

            // Skill Group
            builder.AppendLine($"{skillGroup.Name}, " +
                               $"{count} Skill{(count > 1 ? "s" : string.Empty)}, " +
                               FormattableString.Invariant($"{skillGroupTotalSP:N0} Points"));

            // Skills
            foreach (Skill skill in skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))))
            {
                AddSkills(character, plan, builder, skill);
            }
        }

        /// <summary>
        /// Adds the skills.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="skill">The skill.</param>
        private static void AddSkills(Character character, Plan plan, StringBuilder builder, Skill skill)
        {
            SerializableCharacterSkill mergedSkill = GetMergedSkill(plan, skill);

            string skillPointsText = FormattableString.Invariant($"{mergedSkill.Skillpoints:N0}");
            string pointToLevelFiveText = FormattableString.Invariant($"{skill.StaticData.GetPointsRequiredForLevel(5):N0}");
            string skillDesc = $"{skill} ({skill.Rank})";
            builder.AppendLine($"  {skillDesc.PadRight(45)} " +
                               $"L{mergedSkill.Level} ".PadLeft(5) +
                               $"{skillPointsText}/{pointToLevelFiveText} Points");

            // If the skill is in training...
            if (!skill.IsTraining)
                return;

            string levelText = Skill.GetRomanFromInt(character.CurrentlyTrainingSkill.Level);
            string adjustedEndTimeText = character.CurrentlyTrainingSkill.EndTime.DateTimeToTimeString();
            builder.AppendLine($":  (Currently training to level {levelText}, completes {adjustedEndTimeText} UTC)");
        }

        /// <summary>
        /// Creates a CHR format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsEFTCHR(Character character, Plan plan)
        {
            StringBuilder builder = new StringBuilder();

            foreach (SerializableCharacterSkill skill in character.Skills
                .Where(x => x.IsPublic &&
                            x.Group.ID != DBConstants.CorporationManagementSkillsGroupID &&
                            x.Group.ID != DBConstants.SocialSkillsGroupID &&
                            x.Group.ID != DBConstants.TradeSkillsGroupID)
                .Select(x => GetMergedSkill(plan, x)))
            {
                builder.AppendLine($"{skill.Name}={skill.Level}");
            }

            ESIKey apiKey = character.Identity.FindAPIKeyWithAccess(ESIAPICharacterMethods.CharacterSheet);

            if (apiKey == null)
                return builder.ToString();

            builder
                .AppendLine($"KeyID={apiKey.ID}")
                .AppendLine($"VCode={apiKey.AccessToken}")
                .Append($"CharID={character.CharacterID}");

            return builder.ToString();
        }

        /// <summary>
        /// Creates a HTML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsHTML(Character character, Plan plan)
        {
            // Retrieves a XML representation of this character
            OutputCharacter serial = new OutputCharacter
            {
                Name = character.Name,
                Balance = character.Balance.ToNumericString(2, CultureConstants.InvariantCulture),
                Birthday = character.Birthday.DateTimeToTimeString(),
                CorporationName = character.CorporationName,
                CharacterID = character.CharacterID,
                BloodLine = character.Bloodline,
                Ancestry = character.Ancestry,
                Gender = character.Gender,
                Race = character.Race,
                Intelligence = character.Intelligence.EffectiveValue,
                Perception = character.Perception.EffectiveValue,
                Willpower = character.Willpower.EffectiveValue,
                Charisma = character.Charisma.EffectiveValue,
                Memory = character.Memory.EffectiveValue
            };

            CompleteSerialization(character, plan, serial);

            // Serializes to XML and apply a XSLT to generate the HTML doc
            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(serial);

            XslCompiledTransform xslt = Util.LoadXslt(Properties.Resources.XmlToHtmlXslt);
            XmlDocument htmlDoc = (XmlDocument)Util.Transform(doc, xslt);

            // Returns the string representation of the generated doc
            return Util.GetXmlStringRepresentation(htmlDoc);
        }

        /// <summary>
        /// Completes the serialization.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="plan">The plan.</param>
        /// <param name="serial">The serial.</param>
        private static void CompleteSerialization(Character character, Plan plan, OutputCharacter serial)
        {
            // Attributes enhancers
            foreach (Implant implant in character.CurrentImplants)
            {
                serial.AttributeEnhancers.Add(new OutputAttributeEnhancer
                {
                    Attribute = implant.Slot,
                    Description = implant.Slot.GetDescription(),
                    Bonus = implant.Bonus,
                    Name = implant.Name
                });
            }

            // Skills (grouped by skill groups)
            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                OutputSkillGroup outGroup = AddSkillGroup(plan, skillGroup);

                foreach (Skill skill in skillGroup.Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x))))
                {
                    AddSkill(plan, outGroup, skill);
                }

                if (outGroup.Skills.Count != 0)
                    serial.SkillGroups.Add(outGroup);
            }
        }

        /// <summary>
        /// Adds the skill group.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="skillGroup">The skill group.</param>
        /// <returns></returns>
        private static OutputSkillGroup AddSkillGroup(Plan plan, SkillGroup skillGroup)
        {
            int count = skillGroup
                .Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x)))
                .Select(x => GetMergedSkill(plan, x))
                .Count();

            long skillGroupTotalSP = skillGroup
                .Where(x => x.IsKnown || (plan != null && plan.IsPlanned(x)))
                .Select(x => GetMergedSkill(plan, x))
                .Sum(x => x.Skillpoints);

            OutputSkillGroup outGroup = new OutputSkillGroup
            {
                Name = skillGroup.Name,
                SkillsCount = count,
                TotalSP = FormattableString.Invariant($"{skillGroupTotalSP:N0}")
            };
            return outGroup;
        }

        /// <summary>
        /// Adds the skill.
        /// </summary>
        /// <param name="plan">The plan.</param>
        /// <param name="outGroup">The out group.</param>
        /// <param name="skill">The skill.</param>
        private static void AddSkill(Plan plan, OutputSkillGroup outGroup, Skill skill)
        {
            SerializableCharacterSkill mergedSkill = GetMergedSkill(plan, skill);

            outGroup.Skills.Add(new OutputSkill
            {
                Name = mergedSkill.Name,
                Rank = skill.Rank,
                Level = mergedSkill.Level,
                SkillPoints = FormattableString.Invariant($"{mergedSkill.Skillpoints:N0}"),
                RomanLevel = Skill.GetRomanFromInt(mergedSkill.Level),
                MaxSkillPoints = FormattableString.Invariant($"{skill.StaticData.GetPointsRequiredForLevel(5):N0}")
            });
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        private static string ExportAsEVEMonXML(Character character, Plan plan)
        {
            SerializableSettingsCharacter serial = character.Export();

            if (plan != null)
            {
                serial.Skills.Clear();
                serial.Skills.AddRange(character.Skills.Where(skill => skill.IsKnown || plan.IsPlanned(skill))
                    .Select(skill => GetMergedSkill(plan, skill)));
            }

            XmlDocument doc = (XmlDocument)Util.SerializeToXmlDocument(serial);
            return doc != null ? Util.GetXmlStringRepresentation(doc) : null;
        }

        /// <summary>
        /// Creates a XML format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        private static string ExportAsCCPXML(Character character)
        {
            // Try to use the last XML character sheet downloaded from CCP
            XmlDocument doc = (XmlDocument)LocalXmlCache.GetCharacterXml(character);
            return doc != null ? Util.GetXmlStringRepresentation(doc) : null;
        }

        /// <summary>
        /// Creates a BBCode format file for character exportation.
        /// </summary>
        /// <param name="character"></param>
        /// <exception cref="System.ArgumentNullException">character</exception>
        public static string ExportAsBBCode(Character character)
        {
            character.ThrowIfNull(nameof(character));

            StringBuilder result = new StringBuilder();

            result
                .AppendLine($"[b]{character.Name}[/b]")
                .AppendLine()
                .AppendLine("[b]Attributes[/b]")
                .AppendLine("[table]")
                .AppendLine("[tr][td]Intelligence:[/td][td]" +
                            $"{character.Intelligence.EffectiveValue}".PadLeft(5) +
                            "[/td][/tr]")
                .AppendLine("[tr][td]Perception:[/td][td]" +
                            $"{character.Perception.EffectiveValue}".PadLeft(5) +
                            "[/td][/tr]")
                .AppendLine("[tr][td]Charisma:[/td][td]" +
                            $"{character.Charisma.EffectiveValue}".PadLeft(5) +
                            "[/td][/tr]")
                .AppendLine("[tr][td]Willpower:[/td][td]" +
                            $"{character.Willpower.EffectiveValue}".PadLeft(5) +
                            "[/td][/tr]")
                .AppendLine("[tr][td]Memory:[/td][td]" +
                            $"{character.Memory.EffectiveValue}".PadLeft(5) +
                            "[/td][/tr]")
                .AppendLine("[/table]");

            foreach (SkillGroup skillGroup in character.SkillGroups)
            {
                bool skillGroupAppended = false;
                foreach (Skill skill in skillGroup.Where(skill => skill.Level > 0))
                {
                    if (!skillGroupAppended)
                    {
                        result.AppendLine();
                        result.AppendLine($"[b]{skillGroup.Name}[/b]");

                        skillGroupAppended = true;
                    }

                    result.AppendLine($"[img]{NetworkConstants.EVECommunityBase}" +
                                      $"{NetworkConstants.MyEVELevelImage}{skill.Level}.gif[/img] {skill.Name}");
                }

                if (skillGroupAppended)
                {
                    result.AppendLine(
                        FormattableString.Invariant($"Total Skillpoints in Group: {skillGroup.TotalSP:N0}"));
                }
            }

            result
                .AppendLine()
                .AppendLine(FormattableString.Invariant($"Total Skillpoints: {character.SkillPoints:N0}"))
                .AppendLine(
                    $"Total Number of Skills: {character.KnownSkillCount}".PadLeft(5))
                .AppendLine();

            for (int i = 0; i <= 5; i++)
            {
                result
                    .AppendLine($"Skills at Level {i}: " +
                                $"{character.GetSkillCountAtLevel(i)}".PadLeft(5));
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the skill properties of a merged skill with a plan entry, if one is provided.
        /// If no plan is provided, the skill properties are returned unmodified.
        /// </summary>
        /// <param name="plan"></param>
        /// <param name="skill"></param>
        /// <returns>The skill properties after the merge</returns>
        private static SerializableCharacterSkill GetMergedSkill(Plan plan, Skill skill)
        {
            SerializableCharacterSkill mergedSkill = new SerializableCharacterSkill
            {
                ID = skill.ID,
                Name = skill.Name,
                IsKnown = skill.IsKnown,
                OwnsBook = skill.IsOwned,
                Level = skill.Level,
                Skillpoints = skill.SkillPoints
            };


            plan?.Merge(mergedSkill);

            return mergedSkill;
        }

        /// <summary>
        /// Creates formatted string for character exportation.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="character"></param>
        /// <param name="plan"></param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public static string Export(CharacterSaveFormat format, Character character, Plan plan = null)
        {
            switch (format)
            {
                case CharacterSaveFormat.Text:
                    return ExportAsText(character, plan);
                case CharacterSaveFormat.EFTCHR:
                    return ExportAsEFTCHR(character, plan);
                case CharacterSaveFormat.EVEMonXML:
                    return ExportAsEVEMonXML(character, plan);
                case CharacterSaveFormat.HTML:
                    return ExportAsHTML(character, plan);
                case CharacterSaveFormat.CCPXML:
                    return ExportAsCCPXML(character);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}