using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EVEMon.Common.Constants;
using EVEMon.Common.Models;
using EVEMon.Common.Serialization.Eve;

namespace EVEMon.Controls
{
    public partial class KillReportInvolvedParties : UserControl
    {
        private KillLog m_killLog;


        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="KillReportInvolvedParties"/> class.
        /// </summary>
        public KillReportInvolvedParties()
        {
            InitializeComponent();
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets or sets the kill log.
        /// </summary>
        /// <value>
        /// The kill log.
        /// </value>
        internal KillLog KillLog
        {
            get { return m_killLog; }
            set
            {
                m_killLog = value;
                UpdateContent();
            }
        }

        #endregion


        #region Content Management

        /// <summary>
        /// Updates the content.
        /// </summary>
        private void UpdateContent()
        {
            int topDamageDone = m_killLog.Attackers.Max(y => y.DamageDone);
            SerializableKillLogAttackersListItem finalBlowAttacker = m_killLog.Attackers.Single(x => x.FinalBlow);
            SerializableKillLogAttackersListItem topDamageAttacker = m_killLog.Attackers.Single(x => x.DamageDone == topDamageDone);

            FinalBlowAttacker.KillLog = m_killLog;
            FinalBlowAttacker.Attacker = finalBlowAttacker;
            TopDamageAttacker.KillLog = m_killLog;
            TopDamageAttacker.Attacker = topDamageAttacker;

            InvolvedPartiesLabel.Text = string.Format(CultureConstants.DefaultCulture, InvolvedPartiesLabel.Text,
                                                      m_killLog.Attackers.Count());
            TotalDamageTakenLabel.Text = string.Format(CultureConstants.DefaultCulture, TotalDamageTakenLabel.Text,
                                                       m_killLog.Victim.DamageTaken);

            // Add the rest of the gang
            IList<SerializableKillLogAttackersListItem> remainingAttackers = m_killLog.Attackers
                .Except(new[] { finalBlowAttacker, topDamageAttacker }).ToList();

            if (remainingAttackers.Any())
                InvolvedPartiesPanel.BorderStyle = BorderStyle.Fixed3D;

            foreach (SerializableKillLogAttackersListItem attacker in remainingAttackers.OrderBy(x => x.DamageDone))
            {
                KillReportAttacker attackerControl = new KillReportAttacker();
                InvolvedPartiesPanel.Controls.Add(attackerControl);
                attackerControl.Dock = DockStyle.Top;
                attackerControl.KillLog = m_killLog;
                attackerControl.Attacker = attacker;
            }
        }

        #endregion

    }
}
