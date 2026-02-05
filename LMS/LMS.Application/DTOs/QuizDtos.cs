using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LMS.Domain.Enums;

namespace LMS.Application.DTOs
{
    public class QuizDto
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? TimeLimit { get; set; }
        public int TotalScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<QuestionDto> Questions { get; set; } = new();
    }

    public class QuestionDto
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public string Content { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public int OrderIndex { get; set; }
        public List<OptionDto> Options { get; set; } = new();
    }

    public class OptionDto
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class CreateQuizDto
    {
        [Required]
        public Guid LessonId { get; set; }
        [Required]
        public Guid CourseId { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? TimeLimit { get; set; }
        [Range(1, int.MaxValue)]
        public int TotalScore { get; set; }
        public List<CreateQuestionDto> Questions { get; set; } = new();
    }

    public class UpdateQuizDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? TimeLimit { get; set; }
        [Range(1, int.MaxValue)]
        public int TotalScore { get; set; }
    }

    public class CreateQuestionDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public QuestionType QuestionType { get; set; }
        public int OrderIndex { get; set; }
        public List<CreateOptionDto> Options { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public QuestionType QuestionType { get; set; }
        public int OrderIndex { get; set; }
        public List<CreateOptionDto> Options { get; set; } = new();
    }

    public class CreateOptionDto
    {
        [Required]
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }

    public class SubmitQuizDto
    {
        [Required]
        public List<SubmitQuizAnswerDto> Answers { get; set; } = new();
    }

    public class SubmitQuizAnswerDto
    {
        [Required]
        public Guid QuestionId { get; set; }
        [Required]
        public Guid SelectedOptionId { get; set; }
    }

    public class QuizResultDto
    {
        public Guid Id { get; set; }
        public Guid QuizId { get; set; }
        public Guid StudentId { get; set; }
        public int Score { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
