using System;
using LMS.Domain.Enums;

namespace LMS.Application.DTOs
{
    public class EnrollmentDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    public class CreateEnrollmentDto
    {
        public Guid CourseId { get; set; }
    }
}
