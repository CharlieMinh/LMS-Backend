using LMS.Application.Interfaces.Repositories;
using LMS.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<ICourseProgressRepository, CourseProgressRepository>();
            services.AddScoped<ILessonProgressRepository, LessonProgressRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();
            services.AddScoped<ILessonResourceRepository, LessonResourceRepository>();
            return services;
        }
    }
}
