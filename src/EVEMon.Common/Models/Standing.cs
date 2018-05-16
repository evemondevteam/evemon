using System;
using System.Drawing;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class Standing
    {
        public event EventHandler StandingImageUpdated;


        #region Fields

        private readonly long m_entityID;
        private readonly Character m_character;
        private string m_entityName;
        private Image m_image;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="src"></param>
        internal Standing(Character character, SerializableStandingsListItem src)
        {
            m_character = character;

            m_entityID = src.ID;
            m_entityName = EveMonConstants.UnknownText;
            StandingValue = src.StandingValue;
            Group = src.Group;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string EntityName => m_entityName.IsEmptyOrUnknown() ?
            (m_entityName = EveIDToName.GetIDToName(m_entityID)) : m_entityName;

        /// <summary>
        /// Gets or sets the standing value.
        /// </summary>
        /// <value>The standing value.</value>
        public double StandingValue { get; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public StandingGroup Group { get; }

        /// <summary>
        /// Gets or sets the entity image.
        /// </summary>
        /// <value>The entity image.</value>
        public Image EntityImage
        {
            get
            {
                if (m_image != null)
                    return m_image;

                GetImageAsync().ConfigureAwait(false);

                return m_image ?? (m_image = GetDefaultImage());
            }
        }

        /// <summary>
        /// Gets the effective standing.
        /// </summary>
        /// <value>The effective standing.</value>
        public double EffectiveStanding
        {
            get
            {
                Int64 skillLevel = (StandingValue < 0
                    ? m_character.Skills[DBConstants.DiplomacySkillID]
                    : m_character.Skills[DBConstants.ConnectionsSkillID]).LastConfirmedLvl;
                return StandingValue + (10 - StandingValue) * (skillLevel * 0.04);
            }
        }

        /// <summary>
        /// Gets the standing status.
        /// </summary>
        /// <value>The status.</value>
        public static StandingStatus Status(double standing)
        {
            if (standing <= -5.5)
                return StandingStatus.Terrible;

            if (standing <= -0.5)
                return StandingStatus.Bad;

            if (standing < 0.5)
                return StandingStatus.Neutral;

            return standing < 5.5 ? StandingStatus.Good : StandingStatus.Excellent;
        }

        /// <summary>
        /// Gets the standing image.
        /// </summary>
        /// <param name="standing">The standing.</param>
        /// <returns></returns>
        public static Image GetStandingImage(int standing)
        {
            if (standing <= -5.5)
                return Properties.Resources.TerribleStanding;

            if (standing <= -0.5)
                return Properties.Resources.BadStanding;

            if (standing < 0.5)
                return Properties.Resources.NeutralStanding;

            return standing < 5.5 ? Properties.Resources.GoodStanding : Properties.Resources.ExcellentStanding;
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the entity image.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        private async Task GetImageAsync(bool useFallbackUri = false)
        {
            while (true)
            {
                Image img = await ImageService.GetImageAsync(GetImageUrl(useFallbackUri)).ConfigureAwait(false);

                if (img == null)
                {
                    if (useFallbackUri)
                        return;

                    useFallbackUri = true;
                    continue;
                }

                m_image = img;

                // Notify the subscriber that we got the image
                StandingImageUpdated?.ThreadSafeInvoke(this, EventArgs.Empty);
                break;
            }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns></returns>
        private Image GetDefaultImage()
        {
            switch (Group)
            {
                case StandingGroup.Agents:
                    return Properties.Resources.DefaultCharacterImage32;
                case StandingGroup.NPCCorporations:
                    return Properties.Resources.DefaultCorporationImage32;
                case StandingGroup.Factions:
                    return Properties.Resources.DefaultAllianceImage32;
            }
            return new Bitmap(32, 32);
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <returns></returns>
        private Uri GetImageUrl(bool useFallbackUri)
        {
            string path = Group == StandingGroup.Agents
                ? String.Format(CultureConstants.InvariantCulture,
                    NetworkConstants.CCPPortraits,
                    m_entityID, (int)EveImageSize.x32)
                : String.Format(CultureConstants.InvariantCulture,
                    NetworkConstants.CCPIconsFromImageServer,
                    Group == StandingGroup.Factions ? "alliance" : "corporation",
                    m_entityID, (int)EveImageSize.x32);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }

        #endregion
    }
}
