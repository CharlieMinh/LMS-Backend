using FluentValidation;
using LMS.Application.DTOs;

namespace LMS.API.Validators
{
    public class CreateLessonDtoValidator : AbstractValidator<CreateLessonDto>
    {
        public CreateLessonDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Content).MaximumLength(8000);
            RuleFor(x => x.CourseId).NotEmpty();
            RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
        }
    }

    public class UpdateLessonDtoValidator : AbstractValidator<UpdateLessonDto>
    {
        public UpdateLessonDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Content).MaximumLength(8000);
            RuleFor(x => x.OrderIndex).GreaterThanOrEqualTo(0);
        }
    }
}
