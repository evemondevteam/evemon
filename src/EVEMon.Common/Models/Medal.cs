using System;
using System.Linq;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;
using EVEMon.Common.Extensions;

namespace EVEMon.Common.Models
{
    public sealed class Medal
    {
        #region Fields

        private CCPCharacter m_ccpCharacter;

        private string m_issuer;
        private string m_corporationName;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="ccpCharacter">The CCP character.</param>
        /// <param name="src">The source.</param>
        internal Medal(CCPCharacter ccpCharacter, SerializableMedalsListItem src)
        {
            m_ccpCharacter = ccpCharacter;

            ID = src.MedalID;
            Reason = src.Reason;
            Status = src.Status;
            IssuerID = src.IssuerID;
            CorporationID = src.CorporationID;
            Description = src.Description;
            Title = src.Title;
            Issued = src.Issued;
            Group = src.Group;

            m_issuer = EveIDToName.GetIDToName(src.IssuerID);
            m_corporationName = EveIDToName.GetIDToName(CorporationID);
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public long ID { get; }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public string Status { get; }

        /// <summary>
        /// Gets the issuer ID.
        /// </summary>
        public long IssuerID { get; }

        /// <summary>
        /// Gets the corporation ID.
        /// </summary>
        public long CorporationID { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the issued.
        /// </summary>
        public DateTime Issued { get; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public MedalGroup Group { get; }

        /// <summary>
        /// Gets or sets the number of times this medal was awarded.
        /// </summary>
        public int TimesAwarded { get; set; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        public string Issuer => m_issuer.IsEmptyOrUnknown() ? (m_issuer =
            EveIDToName.GetIDToName(IssuerID)) : m_issuer;

        /// <summary>
        /// Gets the corporation name.
        /// </summary>
        public string CorporationName => m_corporationName.IsEmptyOrUnknown() ?
            (m_corporationName = EveIDToName.GetIDToName(IssuerID)) : m_corporationName;

        #endregion


        #region Helper Methods

        /// <summary>
        /// Tries the assign missing title and description.
        /// </summary>
        /// <returns></returns>
        public bool TryAssignMissingTitleAndDescription()
        {
            if (!String.IsNullOrEmpty(Title) && !String.IsNullOrEmpty(Description))
                return true;

            // Find the related medal in the corporation's medals
            Medal corporationMedal = m_ccpCharacter.CorporationMedals.SingleOrDefault(corpMedal => corpMedal.ID == ID);

            if (corporationMedal == null)
                return false;

            if (String.IsNullOrEmpty(Title))
                Title = corporationMedal.Title;

            if (String.IsNullOrEmpty(Description))
                Description = corporationMedal.Description;

            return true;
        }

        #endregion
    }
}
