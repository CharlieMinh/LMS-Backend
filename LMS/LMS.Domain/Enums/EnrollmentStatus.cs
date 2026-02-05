namespace LMS.Domain.Enums
{
    /// <summary>
    /// Student enrollment status in a course
    /// </summary>
    public enum EnrollmentStatus
    {
        /// <summary>
        /// Student is actively enrolled in the course
        /// </summary>
        Active = 0,

        /// <summary>
        /// Student has completed the course
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Student has dropped/withdrawn from the course
        /// </summary>
        Dropped = 2
    }
}
