namespace LMS.Domain.Enums
{
    /// <summary>
    /// User account status
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// User account is active and can access the system
        /// </summary>
        Active = 0,

        /// <summary>
        /// User account is temporarily inactive
        /// </summary>
        Inactive = 1,

        /// <summary>
        /// User account is banned from the system
        /// </summary>
        Banned = 2
    }
}
