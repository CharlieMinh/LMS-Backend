namespace LMS.Domain.Enums
{
    /// <summary>
    /// Types of quiz questions
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// Multiple choice question with one or more correct answers
        /// </summary>
        MultipleChoice = 0,

        /// <summary>
        /// True or False question
        /// </summary>
        TrueFalse = 1,

        /// <summary>
        /// Essay/text-based question requiring manual grading
        /// </summary>
        Essay = 2
    }
}
