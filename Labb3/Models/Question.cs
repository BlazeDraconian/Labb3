namespace Labb3.Models
{

    internal class Question
    {
        public Question(string query, string correctAnswer, string IncorrectAnswer1, string IncorrectAnswer2, string IncorrectAnswer3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = [IncorrectAnswer1, IncorrectAnswer2, IncorrectAnswer3];
        }
        public string Query { get; set; }
        public string CorrectAnswer { get; set; }
        public string[] IncorrectAnswers { get; set; }
    }
}
