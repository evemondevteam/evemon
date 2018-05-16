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
    public sealed class EmploymentRecord
    {
        public event EventHandler EmploymentRecordImageUpdated;


        #region Fields

        private readonly Character m_character;
        private readonly long m_corporationId;

        private string m_corporationName;
        private Image m_image;

        #endregion


        #region Constructor

        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public EmploymentRecord(Character character, SerializableEmploymentHistoryListItem src)
        {
            src.ThrowIfNull(nameof(src));

            m_character = character;
            m_corporationId = src.CorporationID;
            m_corporationName = string.IsNullOrWhiteSpace(src.CorporationName)
                ? EveIDToName.GetIDToName(src.CorporationID) : src.CorporationName;
            StartDate = src.StartDate;
        }

        /// <summary>
        /// Constructor from the settings.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <param name="src">The source.</param>
        /// <exception cref="System.ArgumentNullException">src</exception>
        public EmploymentRecord(Character character, SerializableEmploymentHistory src)
        {
            src.ThrowIfNull(nameof(src));

            m_character = character;
            m_corporationId = src.CorporationID;
            m_corporationName = src.CorporationName;
            StartDate = src.StartDate;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the corporation.
        /// </summary>
        /// <value>The name of the corporation.</value>
        public string CorporationName => m_corporationName.IsEmptyOrUnknown() ?
            (m_corporationName = EveIDToName.GetIDToName(m_corporationId)) : m_corporationName;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate { get; }

        /// <summary>
        /// Gets the corporation image.
        /// </summary>
        /// <value>The corporation image.</value>
        public Image CorporationImage
        {
            get
            {
                if (m_image != null)
                    return m_image;

                GetImageAsync().ConfigureAwait(false);

                return m_image ?? (m_image = Properties.Resources.DefaultCorporationImage32);
            }
        }

        #endregion


        #region Helper Methods

        /// <summary>
        /// Gets the corporation image.
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

                EmploymentRecordImageUpdated?.ThreadSafeInvoke(this, EventArgs.Empty);
                break;
            }
        }

        /// <summary>
        /// Gets the image URL.
        /// </summary>
        /// <param name="useFallbackUri">if set to <c>true</c> [use fallback URI].</param>
        /// <returns></returns>
        private Uri GetImageUrl(bool useFallbackUri)
        {
            string path = string.Format(CultureConstants.InvariantCulture,
                NetworkConstants.CCPIconsFromImageServer, "corporation", m_corporationId, (int)EveImageSize.x32);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }


        #endregion


        #region Export Method

        /// <summary>
        /// Exports the given object to a serialization object.
        /// </summary>
        public SerializableEmploymentHistory Export()
        {
            SerializableEmploymentHistory serial = new SerializableEmploymentHistory
            {
                CorporationID = m_corporationId,
                CorporationName = CorporationName,
                StartDate = StartDate
            };
            return serial;
        }

        #endregion

    }
}
