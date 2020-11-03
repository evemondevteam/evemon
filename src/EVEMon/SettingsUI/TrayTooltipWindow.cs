using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EVEMon.Common;
using EVEMon.Common.Constants;
using EVEMon.Common.Controls;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Models;

namespace EVEMon.SettingsUI
{
    /// <summary>
    /// Displays a Windows-style tooltip.
    /// </summary>
    public sealed class TrayTooltipWindow : TrayBaseWindow
    {
        private readonly List<Character> m_characters = new List<Character>();
        private string m_tooltipFormat;


        #region Inherited Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            EveMonClient.TimerTick += EveMonClient_TimerTick;
        }

        /// <summary>
        /// On close, stops the timer.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            EveMonClient.TimerTick -= EveMonClient_TimerTick;
        }

        #endregion


        #region Content creation and refresh

        /// <summary>
        /// Updates the tooltip's content.
        /// </summary>
        protected override void UpdateContent()
        {
            if (!Visible)
            {
                UpdatePending = true;
                return;
            }
            UpdatePending = false;

            UpdateCharactersList();

            // Replaces the fragments like "%10546464r" (the number being the character ID) by the remaining time
            string tooltip = m_tooltipFormat ?? string.Empty;
            foreach (Character character in m_characters)
            {
                if (character.IsTraining)
                {
                    QueuedSkill trainingSkill = character.CurrentlyTrainingSkill;
                    TimeSpan remainingTime = trainingSkill.EndTime.Subtract(DateTime.UtcNow);

                    tooltip = Regex.Replace(tooltip,
                        $"%{character.CharacterID}r",
                        remainingTime.ToDescriptiveText(DescriptiveTextOptions.IncludeCommas),
                        RegexOptions.Compiled);
                }

                CCPCharacter ccpCharacter = character as CCPCharacter;
                if (ccpCharacter != null && ccpCharacter.SkillQueue.IsPaused)
                {
                    tooltip = Regex.Replace(tooltip,
                        $"%{character.CharacterID}r",
                        "(Paused)", RegexOptions.Compiled);
                }
            }

            // Updates the tooltip and its location
            ToolTipLabel.Text = tooltip;
            TrayIcon.SetToolTipLocation(this);
        }

        /// <summary>
        /// Updates the tooltip format we use as a base for update on every second, along with the characters list.
        /// </summary>
        private void UpdateCharactersList()
        {
            // Update characters list and selects display order
            m_characters.Clear();
            m_characters.AddRange(Settings.UI.SystemTrayTooltip.DisplayOrder
                                      ? TrayPopupWindow.GetCharacters
                                      : EveMonClient.MonitoredCharacters.Where(x => x.IsTraining));

            // Assembles the tooltip format
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(Settings.UI.SystemTrayTooltip.Format))
            {
                // Bad tooltip format
                sb.Append("You can configure this tooltip in the options/general panel");
            }
            else if (m_characters.Count == 0)
            {
                // No character in training
                sb.Append("No Characters in training!");
            }
            else
            {
                // Assemble tooltip base format with character informations
                foreach (Character character in m_characters)
                {
                    if (sb.Length != 0)
                        sb.Append("\n");

                    sb.Append(FormatTooltipText(Settings.UI.SystemTrayTooltip.Format, character));
                }
            }
            m_tooltipFormat = sb.ToString();
        }

        /// <summary>
        /// Formats the tooltip text.
        /// </summary>
        /// <param name="toolTipFormat">The tool tip format.</param>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        private static string FormatTooltipText(string toolTipFormat, Character character)
        {
            StringBuilder sb = new StringBuilder();

            string tooltipText = Regex.Replace(toolTipFormat, "%([nbsdr]|[ct][ir])",
                                               m => TransformTooltipText(character, m), RegexOptions.Compiled);

            sb.Append(tooltipText);
            return sb.ToString();
        }

        /// <summary>
        /// Transforms the tooltip text.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="m">The regex match.</param>
        /// <returns></returns>
        private static string TransformTooltipText(Character character, Match m)
        {
            // First group
            switch (m.Groups[1].Value[0])
            {
                case 'n':
                    return character.Name;
                case 'b':
                    return character.Balance.ToNumericString(2);
            }

            CCPCharacter ccpCharacter = character as CCPCharacter;
            if (ccpCharacter == null || (!ccpCharacter.IsTraining && !ccpCharacter.SkillQueue.IsPaused))
                return string.Empty;

            int level;
            switch (m.Groups[1].Value[0])
            {
                case 'r':
                    return $"%{character.CharacterID}r";
                case 's':
                    return character.CurrentlyTrainingSkill.SkillName;
                case 'd':
                    return $"{character.CurrentlyTrainingSkill.EndTime:g}";
                case 'c':
                    level = character.CurrentlyTrainingSkill.Level - 1;
                    break;
                case 't':
                    level = character.CurrentlyTrainingSkill.Level;
                    break;
                default:
                    return string.Empty;
            }

            // Second group
            if (level < 0 || m.Groups[1].Value.Length <= 1)
                return string.Empty;

            switch (m.Groups[1].Value[1])
            {
                case 'i':
                    return level.ToString(CultureConstants.DefaultCulture);
                case 'r':
                    return Skill.GetRomanFromInt(level);
                default:
                    return string.Empty;
            }
        }

        #endregion


        #region Global events

        /// <summary>
        /// Every second, when characters are in training, we update the tooltip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EveMonClient_TimerTick(object sender, EventArgs e)
        {
            UpdateContent();
        }

        #endregion
    }
}
