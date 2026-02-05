namespace LMS.Domain.Enums
{
    /// <summary>
    /// Course publication status
    /// </summary>
    public enum CourseStatus
    {
        /// <summary>
        /// Course is in draft mode, not visible to students
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Course is published and available for enrollment
        /// </summary>
        Published = 1,

        /// <summary>
        /// Course is archived, no longer accepting new enrollments
        /// </summary>
        Archived = 2
    }
}
