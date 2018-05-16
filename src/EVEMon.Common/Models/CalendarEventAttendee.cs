using EVEMon.Common.Extensions;
using EVEMon.Common.Serialization.Eve;
using EVEMon.Common.Service;

namespace EVEMon.Common.Models
{
    public sealed class CalendarEventAttendee
    {
        private string m_characterName;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEventAttendee"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal CalendarEventAttendee(SerializableCalendarEventAttendeeListItem src)
        {
            CharacterID = src.CharacterID;
            m_characterName = EveIDToName.GetIDToName(src.CharacterID);
            Response = src.Response;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the character ID.
        /// </summary>
        public long CharacterID { get; }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string CharacterName => (m_characterName.IsEmptyOrUnknown()) ?
            (m_characterName = EveIDToName.GetIDToName(CharacterID)) : m_characterName;

        /// <summary>
        /// Gets the response.
        /// </summary>
        public string Response { get; }

        #endregion

    }
}
