using Labb3.Command;
using Labb3.Dialogs;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Labb3.ViewModels
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

        private int _score = 0;
        public int Score
        {
            get => _score;
            set { _score = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<QuestionPackViewModel> Packs { get; set; } = new();
        public DelegateCommand RandomiseActiveQuestionAnswers { get; }
        public DelegateCommand RandomiseActivePack { get; }
        public DelegateCommand SetPackNameCommand { get; }
        public DelegateCommand NewQuestionPack { get; }
        public DelegateCommand PlayCommand { get; }
        public DelegateCommand ExitCommand { get; }
        public DelegateCommand SelectQuestionPack { get; }

        public DelegateCommand DeleteQuestionPack { get; }

        public DelegateCommand ImportQuestionPack { get; }

        public DelegateCommand AddQuestion { get; }

        public DelegateCommand RemoveQuestion { get; }

        public DelegateCommand EditQuestion { get; }

        public DelegateCommand PackOptions { get; }

        public DelegateCommand FullScreen { get; }

        public DelegateCommand AnswerCommand { get; }

        private List<(string Text, bool IsCorrect)> _shuffledAnswers = new();

        private Brush _buttonColor1 = Brushes.LightGray;
        private Brush _buttonColor2 = Brushes.LightGray;
        private Brush _buttonColor3 = Brushes.LightGray;
        private Brush _buttonColor4 = Brushes.LightGray;

        private List<Question> _playQuestions;

        public Brush ButtonColor1
        {
            get => _buttonColor1;
            set { _buttonColor1 = value; RaisePropertyChanged(); }
        }
        public Brush ButtonColor2
        {
            get => _buttonColor2;
            set { _buttonColor2 = value; RaisePropertyChanged(); }
        }
        public Brush ButtonColor3
        {
            get => _buttonColor3;
            set { _buttonColor3 = value; RaisePropertyChanged(); }
        }
        public Brush ButtonColor4
        {
            get => _buttonColor4;
            set { _buttonColor4 = value; RaisePropertyChanged(); }
        }


        public string Answer1 { get; set; }
        public string Answer2 { get; set; }

        public string Answer3 { get; set; }

        public string Answer4 { get; set; }


        public QuestionPackViewModel ActivePack { get => _mainWindowViewModel.ActivePack; }

        public PlayerViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;
            SetPackNameCommand = new DelegateCommand(SetpackName, CanSetPackName);
            PlayCommand = new DelegateCommand(PlayGame);
            ExitCommand = new DelegateCommand(EndGame);
            NewQuestionPack = new DelegateCommand(newQuestionPack);
            SelectQuestionPack = new DelegateCommand(selectQuestionPack);
            DeleteQuestionPack = new DelegateCommand(deleteQuestionPack);
            ImportQuestionPack = new DelegateCommand(importQuestionPack);
            PackOptions = new DelegateCommand(packOptions);

            AddQuestion = new DelegateCommand(addQuestion);
            RemoveQuestion = new DelegateCommand(removeQuestion);
            EditQuestion = new DelegateCommand(editQuestion);
            FullScreen = new DelegateCommand(fullScreen);
            AnswerCommand = new DelegateCommand(HandleAnswer);
            


            DemoText = string.Empty;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
           
        }
        public readonly DispatcherTimer _timer;



       
            private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_mainWindowViewModel == null)
                return;


            if (_mainWindowViewModel.Model != this)
                return;

                RemainingTime--;

            if (RemainingTime <= 0)
            {
                _timer.Stop();
                NextQuestion();
            }
        }
        

        private int _remainingTime;
        public int RemainingTime
        {
            get => _remainingTime;
            set
            {
                _remainingTime = value;
                RaisePropertyChanged();
            }
        }

        private string _demoText;


        public string DemoText
        {
            get { return _demoText; }
            set
            {
                _demoText = value;
                RaisePropertyChanged();
                SetPackNameCommand.RaiseCanExecuteChanged();
            }
        }

        public Random RandomQuestions { get; private set; }

        public void newQuestionPack(object? obj)
        {
            var dialog = new CreateNewPackDialog();
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                var vm = dialog.ViewModel;

                var newPack = new QuestionPack(vm.NewPack.Name)
                {
                    Difficulty = vm.NewPack.Difficulty,
                    TimeLimitInSeconds = vm.NewPack.TimeLimitInSeconds,
                };
                var newPackVM = new QuestionPackViewModel(newPack);

                _mainWindowViewModel.Packs.Add(newPackVM);
                _mainWindowViewModel.ActivePack = newPackVM;
                _mainWindowViewModel.SavePacksToFile();

            }
        }


        public void packOptions(object? obj)
        {
            var dialog = new CreateNewPackDialog(_mainWindowViewModel.ActivePack);
            dialog.ShowDialog();

            if (dialog.ViewModel.DialogResult != true)
                return;

            _mainWindowViewModel.ActivePack.RaisePropertyChanged(nameof(_mainWindowViewModel.ActivePack.Name));
            _mainWindowViewModel.ActivePack.RaisePropertyChanged(nameof(_mainWindowViewModel.ActivePack.Difficulty));
            _mainWindowViewModel.ActivePack.RaisePropertyChanged(nameof(_mainWindowViewModel.ActivePack.TimeLimitInSeconds));

        }

        public void selectQuestionPack(object? obj)
        {
            _mainWindowViewModel!.Model = _mainWindowViewModel.ConfigurationViewModel;
            _mainWindowViewModel.ActivePack = (QuestionPackViewModel)obj;
        }

        private void deleteQuestionPack(object? obj)
        {
            if (_mainWindowViewModel is null)
            {
                return;
            }

            _mainWindowViewModel.Packs.Remove(_mainWindowViewModel.ActivePack);
            _mainWindowViewModel.SavePacksToFile();

            if (_mainWindowViewModel.Packs.Any())
            {
                _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs.First();
            }
            else
            {
                _mainWindowViewModel.ActivePack = null;
                _mainWindowViewModel.Model = _mainWindowViewModel.ConfigurationViewModel;
            }
        }

        public void importQuestionPack(object? obj)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var json = File.ReadAllText(dialog.FileName);
                var pack = System.Text.Json.JsonSerializer.Deserialize<QuestionPack>(json);

                var newPack = new QuestionPackViewModel(pack);
                _mainWindowViewModel!.Packs.Add(newPack);
                _mainWindowViewModel.ActivePack = newPack;
            }
        }

        public void addQuestion(object? obj)
        {
            if (_mainWindowViewModel?.ActivePack == null)
                return;

            var newQuestion = new Question
            {
                Query = "New Question (Please fill in)",
                CorrectAnswer = "",
                IncorrectAnswer1 = "",
                IncorrectAnswer2 = "",
                IncorrectAnswer3 = ""
            };

            _mainWindowViewModel.ActivePack.Questions.Add(newQuestion);
            _mainWindowViewModel.SavePacksToFile();
        }

        public void removeQuestion(object? obj)
        {
            if (ActivePack == null || ActivePack == null)
                return;

            else if (ActivePack.SelectedQuestion != null)
            {
                ActivePack.Questions.Remove(ActivePack.SelectedQuestion);
            }
        }

        public void editQuestion(object? obj)
        {
            _mainWindowViewModel!.Model = _mainWindowViewModel.ConfigurationViewModel;
        }

        private void PlayGame(object? obj)
        {
            _currentQuestionIndex = 0;
            _mainWindowViewModel!.Model = _mainWindowViewModel.PlayerViewModel;
            
            ShuffleAnswers();
            RaisePropertyChanged(nameof(CurrentQuestion));
            RaisePropertyChanged(nameof(QuestionCounterText));
            
            ResetButtonColors();

            
        }

        private void EndGame(object? obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void fullScreen(object? obj)
        {
            var window = System.Windows.Application.Current.MainWindow;

            if (window.WindowState == System.Windows.WindowState.Normal)
            {
                window.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                window.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private bool CanSetPackName(object? arg)
        {
            return DemoText.Length > 0;
        }

        private void SetpackName(object? obj)
        {
            ActivePack.Name = DemoText;
        }


        private int _currentQuestionIndex;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                ActivePack.SelectedQuestion = CurrentQuestion;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentQuestion));
                RaisePropertyChanged(nameof(QuestionCounterText));
                RaisePropertyChanged(nameof(ActivePack.SelectedQuestion));
                ShuffleAnswers();
            }
        }

        public Question CurrentQuestion => ActivePack.Questions[CurrentQuestionIndex];

        public string QuestionCounterText =>
            $"Fråga {CurrentQuestionIndex + 1} av {ActivePack.Questions.Count}";

        private async void CheckAnswer(object chosenIndexObj)
        {
            if (chosenIndexObj == null)
                return;

            if (!int.TryParse(chosenIndexObj.ToString(), out int chosenIndex))
                return;

            if (_shuffledAnswers == null || _shuffledAnswers.Count == 0)
                return;

            if (chosenIndex < 0 || chosenIndex >= _shuffledAnswers.Count)
                return;

            bool isCorrect = _shuffledAnswers[chosenIndex].IsCorrect;

            HighlightAnswers();

            await Task.Delay(5000);

            NextQuestion();
        }

        private void HighlightAnswers()
        {
            ButtonColor1 = _shuffledAnswers[0].IsCorrect ? Brushes.LightGreen : Brushes.LightCoral;
            ButtonColor2 = _shuffledAnswers[1].IsCorrect ? Brushes.LightGreen : Brushes.LightCoral;
            ButtonColor3 = _shuffledAnswers[2].IsCorrect ? Brushes.LightGreen : Brushes.LightCoral;
            ButtonColor4 = _shuffledAnswers[3].IsCorrect ? Brushes.LightGreen : Brushes.LightCoral;

            RaisePropertyChanged(nameof(ButtonColor1));
            RaisePropertyChanged(nameof(ButtonColor2));
            RaisePropertyChanged(nameof(ButtonColor3));
            RaisePropertyChanged(nameof(ButtonColor4));
        }

        private void ResetButtonColors()
        {
            ButtonColor1 = ButtonColor2 = ButtonColor3 = ButtonColor4 = Brushes.LightGray;

            RaisePropertyChanged(nameof(ButtonColor1));
            RaisePropertyChanged(nameof(ButtonColor2));
            RaisePropertyChanged(nameof(ButtonColor3));
            RaisePropertyChanged(nameof(ButtonColor4));
        }


        public void StartGame()
        {
            _playQuestions = ActivePack.Questions.ToList();  
            Shuffle(_playQuestions);

            if (ActivePack == null || ActivePack.Questions.Count == 0)
                return;

            if (ActivePack.SelectedQuestion == null)
                ActivePack.SelectedQuestion = ActivePack.Questions[0];

            _currentQuestionIndex = ActivePack.Questions.IndexOf(ActivePack.SelectedQuestion);

            LoadQuestion(ActivePack.SelectedQuestion);
        }

        private void LoadQuestion(Question q)
        {
            RemainingTime = ActivePack.TimeLimitInSeconds;
            RaisePropertyChanged(nameof(RemainingTime));

            _timer.Start();


            var answers = new List<(string Text, bool IsCorrect)>
        {
            (q.CorrectAnswer, true),
            (q.IncorrectAnswer1, false),
            (q.IncorrectAnswer2, false),
            (q.IncorrectAnswer3, false)
        };
               
            _shuffledAnswers = answers.OrderBy(a => Guid.NewGuid()).ToList();

            Answer1 = _shuffledAnswers[0].Text;
            Answer2 = _shuffledAnswers[1].Text;
            Answer3 = _shuffledAnswers[2].Text;
            Answer4 = _shuffledAnswers[3].Text;

            ResetButtonColors();

            RaisePropertyChanged(nameof(Answer1));
            RaisePropertyChanged(nameof(Answer2));
            RaisePropertyChanged(nameof(Answer3));
            RaisePropertyChanged(nameof(Answer4));
        }

        private async void HandleAnswer(object? chosenIndexObj)
        {
            if (chosenIndexObj == null || _shuffledAnswers == null)
                return;

            int chosenIndex = int.Parse(chosenIndexObj.ToString());
            bool isCorrect = _shuffledAnswers[chosenIndex].IsCorrect;
            if (isCorrect)
                Score++;

            for (int i = 0; i < 4; i++)
            {
                var color = _shuffledAnswers[i].IsCorrect ? Brushes.LightGreen : Brushes.IndianRed;
                switch (i)
                {
                    case 0: ButtonColor1 = color; break;
                    case 1: ButtonColor2 = color; break;
                    case 2: ButtonColor3 = color; break;
                    case 3: ButtonColor4 = color; break;
                }
            }

            RaisePropertyChanged(nameof(ButtonColor1));
            RaisePropertyChanged(nameof(ButtonColor2));
            RaisePropertyChanged(nameof(ButtonColor3));
            RaisePropertyChanged(nameof(ButtonColor4));

            await Task.Delay(2000); 
            NextQuestion();
        }

        private void NextQuestion()
        {
            
            _currentQuestionIndex++;
            if (_currentQuestionIndex >= ActivePack.Questions.Count)
            {
                _timer.Stop();
                System.Windows.MessageBox.Show($"Du fick {Score} rätt av {ActivePack.Questions.Count}!", "Resultat");
                Score = 0;
                _mainWindowViewModel.Model = _mainWindowViewModel.ConfigurationViewModel;
                return;
            }

            

            var next = _playQuestions[_currentQuestionIndex];
            ActivePack.SelectedQuestion = next;
            LoadQuestion(next);

            RaisePropertyChanged(nameof(CurrentQuestion));
            RaisePropertyChanged(nameof(QuestionCounterText));
            RaisePropertyChanged(nameof(ActivePack));
            _timer.Start();
        }

        private static void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }

        private void ShuffleAnswers()
        {
                var q = CurrentQuestion;

                _shuffledAnswers = new List<(string Text, bool IsCorrect)>
    {
            (q.CorrectAnswer??"", true),
            (q.IncorrectAnswer1?? "", false),
            (q.IncorrectAnswer2?? "",false),
            (q.IncorrectAnswer3?? "", false)
    };

                _shuffledAnswers = _shuffledAnswers.OrderBy(x => Guid.NewGuid()).ToList();

                Answer1 = _shuffledAnswers[0].Text;
                Answer2 = _shuffledAnswers[1].Text;
                Answer3 = _shuffledAnswers[2].Text;
                Answer4 = _shuffledAnswers[3].Text;

        }
    }
}
