using EVEMon.Common.Serialization.Esi;

namespace EVEMon.Common.Models
{
    public sealed class EveMailBody
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EveMailBody"/> class.
        /// </summary>
        /// <param name="src">The SRC.</param>
        internal EveMailBody(long id, EsiAPIMailBody src)
        {
            MessageID = id;
            BodyText = src.Body;
        }

        /// <summary>
        /// Gets or sets the message ID.
        /// </summary>
        /// <value>The message ID.</value>
        public long MessageID { get; }

        /// <summary>
        /// Gets or sets the body text.
        /// </summary>
        /// <value>The body text.</value>
        public string BodyText { get; }
    }
}
