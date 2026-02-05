using LMS.Application.Interfaces.Services;
using LMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IProgressService, ProgressService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<ILessonResourceService, LessonResourceService>();
            return services;
        }
    }
}
