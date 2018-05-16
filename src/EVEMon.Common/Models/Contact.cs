using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using EVEMon.Common.Constants;
using EVEMon.Common.Data;
using EVEMon.Common.Enumerations;
using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class Contact
    {
        public event EventHandler ContactImageUpdated;


        #region Fields

        private readonly long m_contactID;
        private readonly ContactType m_contactType;
        private Image m_image;
        private string m_contactName;

        #endregion


        /// <summary>
        /// Constructor from the API.
        /// </summary>
        /// <param name="src"></param>
        internal Contact(SerializableContactListItem src)
        {
            m_contactID = src.ContactID;
            m_contactName = EveIDToName.GetIDToName(m_contactID);
            IsInWatchlist = src.InWatchlist;
            Standing = src.Standing;
            Group = src.Group == ContactGroup.Personal && StaticGeography.AllAgents.Any(
                x => x.ID == m_contactID) ? ContactGroup.Agent : src.Group;

            switch (src.ContactTypeID)
            {
            case DBConstants.CorporationID:
                m_contactType = ContactType.Corporation;
                break;
            case DBConstants.AllianceID:
                m_contactType = ContactType.Alliance;
                break;
            default:
                m_contactType = ContactType.Character;
                break;
            }
        }


        #region Public Properties

        /// <summary>
        /// Gets the name of the contact.
        /// </summary>
        /// <value>
        /// The name of the contact.
        /// </value>
        public string Name => (m_contactName.IsEmptyOrUnknown()) ? (m_contactName =
            EveIDToName.GetIDToName(m_contactID)) : m_contactName;

        /// <summary>
        /// Gets a value indicating whether the contact is in the watchlist.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the contact is in the watchlist; otherwise, <c>false</c>.
        /// </value>
        public bool IsInWatchlist { get; }

        /// <summary>
        /// Gets the standing.
        /// </summary>
        public double Standing { get; }

        /// <summary>
        /// Gets the group.
        /// </summary>
        public ContactGroup Group { get; }


        /// <summary>
        /// Gets the entity image.
        /// </summary>
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
                ContactImageUpdated?.ThreadSafeInvoke(this, EventArgs.Empty);
                break;
            }
        }

        /// <summary>
        /// Gets the default image.
        /// </summary>
        /// <returns></returns>
        private Image GetDefaultImage()
        {
            switch (m_contactType)
            {
                case ContactType.Character:
                    return Properties.Resources.DefaultCharacterImage32;
                case ContactType.Corporation:
                    return Properties.Resources.DefaultCorporationImage32;
                case ContactType.Alliance:
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
            string path = m_contactType == ContactType.Character
                ? String.Format(CultureConstants.InvariantCulture,
                    NetworkConstants.CCPPortraits,
                    m_contactID, (int)EveImageSize.x32)
                : String.Format(CultureConstants.InvariantCulture,
                    NetworkConstants.CCPIconsFromImageServer,
                    m_contactType == ContactType.Alliance ? "alliance" : "corporation",
                    m_contactID, (int)EveImageSize.x32);

            return useFallbackUri
                ? ImageService.GetImageServerBaseUri(path)
                : ImageService.GetImageServerCdnUri(path);
        }

        #endregion
    }
}
