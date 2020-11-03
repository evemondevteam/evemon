namespace EVEMon.Common.Models
{
    /// <summary>
    /// Stores the automatically determined account status of a character - Omega or Alpha.
    /// </summary>
    public enum AccountStatus
    {
        Unknown, Alpha, Omega
    }

    /// <summary>
    /// Stores the manually set account status of a character.
    /// </summary>
    public enum AccountStatusMode
    {
        Auto, Alpha, Omega
    }
}
