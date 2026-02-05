using System;
using System.Linq;
using System.Threading.Tasks;
using LMS.Application.Common;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;

namespace LMS.Application.Services
{
    public class LessonResourceService : ILessonResourceService
    {
        private readonly ILessonResourceRepository _resourceRepository;
        private readonly ILessonRepository _lessonRepository;

        public LessonResourceService(ILessonResourceRepository resourceRepository, ILessonRepository lessonRepository)
        {
            _resourceRepository = resourceRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<PagedResult<LessonResourceDto>> GetByLessonAsync(Guid lessonId, PagedRequest request)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId) ?? throw new KeyNotFoundException("Lesson not found");
            var resources = await _resourceRepository.GetByLessonAsync(lesson.Id);
            var query = resources.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.ToLower();
                query = query.Where(r => r.Title.ToLower().Contains(term));
            }

            var sorted = query.ApplySorting(request);
            var paged = sorted.ToPagedResult(request);

            return new PagedResult<LessonResourceDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<LessonResourceDto> CreateAsync(CreateLessonResourceDto dto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(dto.LessonId) ?? throw new KeyNotFoundException("Lesson not found");

            var resource = new LessonResource
            {
                LessonId = lesson.Id,
                Title = dto.Title,
                Url = dto.Url,
                ResourceType = dto.ResourceType,
                Description = dto.Description
            };

            await _resourceRepository.AddAsync(resource);
            return MapToDto(resource);
        }

        public async Task UpdateAsync(Guid id, UpdateLessonResourceDto dto)
        {
            var resource = await _resourceRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Resource not found");

            resource.Title = dto.Title;
            resource.Url = dto.Url;
            resource.ResourceType = dto.ResourceType;
            resource.Description = dto.Description;
            resource.UpdatedAt = DateTime.UtcNow;

            await _resourceRepository.UpdateAsync(resource);
        }

        public async Task DeleteAsync(Guid id)
        {
            var resource = await _resourceRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Resource not found");
            await _resourceRepository.DeleteAsync(resource);
        }

        private static LessonResourceDto MapToDto(LessonResource resource) => new LessonResourceDto
        {
            Id = resource.Id,
            LessonId = resource.LessonId,
            Title = resource.Title,
            Url = resource.Url,
            ResourceType = resource.ResourceType,
            Description = resource.Description,
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt
        };
    }
}
