using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.API.Validators
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(4000);
            RuleFor(x => x.InstructorId).NotEmpty();
        }
    }

    public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(4000);
        }
    }
}
