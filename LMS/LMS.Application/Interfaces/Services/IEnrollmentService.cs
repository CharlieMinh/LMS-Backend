using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Application.DTOs;

namespace LMS.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> EnrollAsync(Guid studentId, Guid courseId);
        Task<PagedResult<EnrollmentDto>> GetMyEnrollmentsAsync(Guid studentId, PagedRequest request);
        Task<PagedResult<EnrollmentDto>> GetEnrollmentsByCourseAsync(Guid courseId, PagedRequest request);
    }
}
