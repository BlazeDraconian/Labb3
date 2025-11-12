using Labb3.Command;
using Labb3.Dialogs;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Labb3.ViewModels
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

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

        public string ButtonColor1 { get; set; } = "LightGray";
        public string ButtonColor2 { get; set; } = "LightGray";
        public string ButtonColor3 { get; set; } = "LightGray";
        public string ButtonColor4 { get; set; } = "LightGray";

        private string _answer1;
        public string Answer1
        {
            get => _answer1;
            set
            {
                _answer1 = value;
                RaisePropertyChanged();
            }
        }

        private string _answer2;
        public string Answer2
        {
            get => _answer2;
            set
            {
                _answer2 = value;
                RaisePropertyChanged();
            }
        }

        private string _answer3;
        public string Answer3
        {
            get => _answer3;
            set
            {
                _answer3 = value;
                RaisePropertyChanged();
            }
        }

        private string _answer4;
        public string Answer4
        {
            get => _answer4;
            set
            {
                _answer4 = value;
                RaisePropertyChanged();
            }
        }



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
            AnswerCommand = new DelegateCommand(CheckAnswer);
            


            DemoText = string.Empty;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        public readonly DispatcherTimer _timer;

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (ActivePack == null || _mainWindowViewModel.Model == _mainWindowViewModel.ConfigurationViewModel)
            {
                _timer.Stop();
                return;
            }
            ActivePack.TimeLimitInSeconds = ActivePack.TimeLimitInSeconds -= 1;
            if (ActivePack.TimeLimitInSeconds < 0)
            {
                _timer.Stop();
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
            ActivePack.Questions.Add(new Question("", "", "", "", ""));
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
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(CurrentQuestion));
                RaisePropertyChanged(nameof(QuestionCounterText));
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

            GoToNextQuestion();
        }

        private void HighlightAnswers()
        {
            ButtonColor1 = _shuffledAnswers[0].IsCorrect ? "LightGreen" : "LightCoral";
            ButtonColor2 = _shuffledAnswers[1].IsCorrect ? "LightGreen" : "LightCoral";
            ButtonColor3 = _shuffledAnswers[2].IsCorrect ? "LightGreen" : "LightCoral";
            ButtonColor4 = _shuffledAnswers[3].IsCorrect ? "LightGreen" : "LightCoral";

            RaisePropertyChanged(nameof(ButtonColor1));
            RaisePropertyChanged(nameof(ButtonColor2));
            RaisePropertyChanged(nameof(ButtonColor3));
            RaisePropertyChanged(nameof(ButtonColor4));
        }

        private void ResetButtonColors()
        {
            ButtonColor1 = ButtonColor2 = ButtonColor3 = ButtonColor4 = "LightGray";

            RaisePropertyChanged(nameof(ButtonColor1));
            RaisePropertyChanged(nameof(ButtonColor2));
            RaisePropertyChanged(nameof(ButtonColor3));
            RaisePropertyChanged(nameof(ButtonColor4));
        }

        private void GoToNextQuestion()
        {
            ResetButtonColors();

            if (CurrentQuestionIndex < ActivePack.Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                ShuffleAnswers();
            }
            else
            {
                _mainWindowViewModel!.Model = _mainWindowViewModel.ConfigurationViewModel;
            }

        }

        private void ShuffleAnswers()
        {
                var q = CurrentQuestion;

                _shuffledAnswers = new List<(string Text, bool IsCorrect)>
    {
            (q.CorrectAnswer??"", true),
            (q.IncorrectAnswers[0]?? "", false),
            (q.IncorrectAnswers[1]?? "",false),
            (q.IncorrectAnswers[2]?? "", false)
    };

                _shuffledAnswers = _shuffledAnswers.OrderBy(x => Guid.NewGuid()).ToList();

                Answer1 = _shuffledAnswers[0].Text;
                Answer2 = _shuffledAnswers[1].Text;
                Answer3 = _shuffledAnswers[2].Text;
                Answer4 = _shuffledAnswers[3].Text;

        }
    }
}
