using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Labb3.Models
{

        public class Question : INotifyPropertyChanged
        {

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


        public Question() { }
        public Question(string query, string correctAnswer, string i1, string i2, string i3)
        {
            Query = query;
            CorrectAnswer = correctAnswer;
            IncorrectAnswer1 = i1;
            IncorrectAnswer2 = i2;
            IncorrectAnswer3 = i3; ;
        }
        private string _query = string.Empty;
        private string _correctAnswer = string.Empty;
        private string _incorrect1 = string.Empty;
        private string _incorrect2 = string.Empty;
        private string _incorrect3 = string.Empty;

        public string Query
        {
            get => _query;
            set { _query = value; OnPropertyChanged(nameof(Query)); }
        }

        public string CorrectAnswer
        {
            get => _correctAnswer;
            set { _correctAnswer = value; OnPropertyChanged(nameof(CorrectAnswer)); }
        }

        public string IncorrectAnswer1
        {
            get => _incorrect1;
            set { _incorrect1 = value; OnPropertyChanged(nameof(IncorrectAnswer1)); }
        }

        public string IncorrectAnswer2
        {
            get => _incorrect2;
            set { _incorrect2 = value; OnPropertyChanged(nameof(IncorrectAnswer2)); }
        }

        public string IncorrectAnswer3
        {
            get => _incorrect3;
            set { _incorrect3 = value; OnPropertyChanged(nameof(IncorrectAnswer3)); }
        }
    }
}
