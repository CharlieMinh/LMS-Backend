using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Application.Common;
using LMS.Application.DTOs;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Interfaces.Services;
using LMS.Domain.Entities;
using LMS.Domain.Enums;

namespace LMS.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public QuizService(
            IQuizRepository quizRepository,
            ICourseRepository courseRepository,
            ILessonRepository lessonRepository,
            IEnrollmentRepository enrollmentRepository)
        {
            _quizRepository = quizRepository;
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<QuizDto?> GetQuizAsync(Guid id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id, includeDetails: true);
            return quiz == null ? null : MapToDto(quiz);
        }

        public async Task<PagedResult<QuizDto>> GetQuizzesByCourseAsync(Guid courseId, PagedRequest request)
        {
            var quizzes = await _quizRepository.GetByCourseAsync(courseId, includeDetails: true);
            var query = quizzes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var term = request.Search.ToLower();
                query = query.Where(q => q.Title.ToLower().Contains(term));
            }

            var sorted = query.ApplySorting(request);
            var paged = sorted.ToPagedResult(request);

            return new PagedResult<QuizDto>
            {
                Items = paged.Items.Select(MapToDto).ToList(),
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
        }

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto dto)
        {
            await EnsureCourseAndLessonAsync(dto.CourseId, dto.LessonId);

            var quiz = new Quiz
            {
                CourseId = dto.CourseId,
                LessonId = dto.LessonId,
                Title = dto.Title,
                Description = dto.Description,
                TimeLimit = dto.TimeLimit,
                TotalScore = dto.TotalScore,
                Questions = dto.Questions.Select(q => new Question
                {
                    Content = q.Content,
                    QuestionType = q.QuestionType,
                    OrderIndex = q.OrderIndex,
                    Options = q.Options.Select(o => new Option
                    {
                        Content = o.Content,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                }).ToList()
            };

            await _quizRepository.AddAsync(quiz);
            return MapToDto(quiz);
        }

        public async Task UpdateQuizAsync(Guid id, UpdateQuizDto dto)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null) throw new KeyNotFoundException("Quiz not found");

            quiz.Title = dto.Title;
            quiz.Description = dto.Description;
            quiz.TimeLimit = dto.TimeLimit;
            quiz.TotalScore = dto.TotalScore;
            quiz.UpdatedAt = DateTime.UtcNow;

            await _quizRepository.UpdateAsync(quiz);
        }

        public async Task DeleteQuizAsync(Guid id)
        {
            var quiz = await _quizRepository.GetByIdAsync(id);
            if (quiz == null) throw new KeyNotFoundException("Quiz not found");
            await _quizRepository.DeleteAsync(quiz);
        }

        public async Task<QuestionDto> AddQuestionAsync(Guid quizId, CreateQuestionDto dto)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null) throw new KeyNotFoundException("Quiz not found");

            var question = new Question
            {
                QuizId = quizId,
                Content = dto.Content,
                QuestionType = dto.QuestionType,
                OrderIndex = dto.OrderIndex,
                Options = dto.Options.Select(o => new Option
                {
                    Content = o.Content,
                    IsCorrect = o.IsCorrect
                }).ToList()
            };

            await _quizRepository.AddQuestionAsync(question);
            return MapQuestionToDto(question);
        }

        public async Task UpdateQuestionAsync(Guid quizId, Guid questionId, UpdateQuestionDto dto)
        {
            var question = await _quizRepository.GetQuestionByIdAsync(questionId, includeOptions: true);
            if (question == null || question.QuizId != quizId)
            {
                throw new KeyNotFoundException("Question not found");
            }

            question.Content = dto.Content;
            question.QuestionType = dto.QuestionType;
            question.OrderIndex = dto.OrderIndex;
            question.UpdatedAt = DateTime.UtcNow;

            var newOptions = dto.Options.Select(o => new Option
            {
                Id = Guid.NewGuid(),
                QuestionId = question.Id,
                Content = o.Content,
                IsCorrect = o.IsCorrect
            }).ToList();

            await _quizRepository.ReplaceQuestionOptionsAsync(question, newOptions);
        }

        public async Task DeleteQuestionAsync(Guid quizId, Guid questionId)
        {
            var question = await _quizRepository.GetQuestionByIdAsync(questionId);
            if (question == null || question.QuizId != quizId)
            {
                throw new KeyNotFoundException("Question not found");
            }

            await _quizRepository.DeleteQuestionAsync(question);
        }

        public async Task<QuizResultDto> SubmitAsync(Guid quizId, Guid studentId, SubmitQuizDto dto)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId, includeDetails: true);
            if (quiz == null) throw new KeyNotFoundException("Quiz not found");

            var enrollment = await _enrollmentRepository.GetByStudentAndCourseAsync(studentId, quiz.CourseId);
            if (enrollment == null) throw new InvalidOperationException("Student is not enrolled in this course");

            var questionDict = quiz.Questions.ToDictionary(q => q.Id);
            if (dto.Answers.Any(a => !questionDict.ContainsKey(a.QuestionId)))
            {
                throw new InvalidOperationException("Invalid question in answers");
            }

            var answers = new List<QuizAnswer>();
            var correctCount = 0;

            foreach (var answer in dto.Answers)
            {
                var question = questionDict[answer.QuestionId];
                var option = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
                if (option == null)
                {
                    throw new InvalidOperationException("Selected option does not belong to question");
                }

                answers.Add(new QuizAnswer
                {
                    QuizResultId = Guid.Empty, // will be set after result created
                    QuestionId = question.Id,
                    SelectedOptionId = option.Id
                });

                if (option.IsCorrect)
                {
                    correctCount++;
                }
            }

            var questionCount = quiz.Questions.Count;
            if (questionCount == 0) throw new InvalidOperationException("Quiz has no questions");

            var scorePerQuestion = quiz.TotalScore / (double)questionCount;
            var score = (int)Math.Round(correctCount * scorePerQuestion);

            var previousResults = await _quizRepository.GetResultsForStudentAsync(quizId, studentId);
            var bestExisting = previousResults.Any() ? previousResults.Max(r => r.Score) : 0;
            var finalScore = Math.Max(score, bestExisting);

            var result = new QuizResult
            {
                Id = Guid.NewGuid(),
                QuizId = quizId,
                StudentId = studentId,
                Score = finalScore,
                SubmittedAt = DateTime.UtcNow,
                Answers = new List<QuizAnswer>()
            };

            await _quizRepository.AddResultAsync(result);

            foreach (var ans in answers)
            {
                ans.QuizResultId = result.Id;
            }

            if (answers.Count > 0)
            {
                await _quizRepository.AddAnswersAsync(answers);
            }

            return new QuizResultDto
            {
                Id = result.Id,
                QuizId = quizId,
                StudentId = studentId,
                Score = finalScore,
                SubmittedAt = result.SubmittedAt
            };
        }

        private static QuizDto MapToDto(Quiz quiz)
        {
            return new QuizDto
            {
                Id = quiz.Id,
                CourseId = quiz.CourseId,
                LessonId = quiz.LessonId,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.TimeLimit,
                TotalScore = quiz.TotalScore,
                CreatedAt = quiz.CreatedAt,
                UpdatedAt = quiz.UpdatedAt,
                Questions = quiz.Questions
                    .OrderBy(q => q.OrderIndex)
                    .Select(MapQuestionToDto)
                    .ToList()
            };
        }

        private static QuestionDto MapQuestionToDto(Question question)
        {
            return new QuestionDto
            {
                Id = question.Id,
                QuizId = question.QuizId,
                Content = question.Content,
                QuestionType = question.QuestionType,
                OrderIndex = question.OrderIndex,
                Options = question.Options
                    .Select(o => new OptionDto
                    {
                        Id = o.Id,
                        QuestionId = o.QuestionId,
                        Content = o.Content,
                        IsCorrect = o.IsCorrect
                    })
                    .ToList()
            };
        }

        private async Task EnsureCourseAndLessonAsync(Guid courseId, Guid lessonId)
        {
            var lesson = await _lessonRepository.GetByIdAsync(lessonId);
            if (lesson == null || lesson.CourseId != courseId)
            {
                throw new KeyNotFoundException("Lesson not found in course");
            }

            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
            {
                throw new KeyNotFoundException("Course not found");
            }
        }
    }
}
