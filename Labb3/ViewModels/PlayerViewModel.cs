using Labb3.Command;
using Labb3.Dialogs;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
       
        public DelegateCommand RandomiseActiveQuestionAnswers {  get;}
        public DelegateCommand RandomiseActivePack {  get;}
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
            //RandomiseActivePack = new DelegateCommand(randomiseActivePack);
            //RandomiseActiveQuestionAnswers = new DelegateCommand(randomiseActiveQuestionAnswers);


            DemoText = string.Empty;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick; 
            _timer.Start();    
        }
        public readonly DispatcherTimer _timer;

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (ActivePack == null)
            {
                _timer.Stop();
                return;
            }
            ActivePack.TimeLimitInSeconds= ActivePack.TimeLimitInSeconds -= 1;
            if (ActivePack.TimeLimitInSeconds < 0)
            {
                _timer.Stop();
            }
        }

        private string _demoText;
       

        public string DemoText
        {
            get { return _demoText; }
            set { 
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
                    Difficulty= vm.NewPack.Difficulty,
                    TimeLimitInSeconds= vm.NewPack.TimeLimitInSeconds,
                };
                var newPackVM = new QuestionPackViewModel(newPack);

                _mainWindowViewModel.Packs.Add(newPackVM);
                _mainWindowViewModel.ActivePack = newPackVM;
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
        }

        public void deleteQuestionPack(object? obj)
        {
            if (_mainWindowViewModel is null)
            {
                return;
            }

            _mainWindowViewModel.Packs.Remove(_mainWindowViewModel.ActivePack);

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
            ActivePack.Questions.Add(new Question("" , "", "", "", ""));
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
            _mainWindowViewModel!.Model = _mainWindowViewModel.PlayerViewModel; 
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
        

        //public void randomiseActivePack()
        //{
        //    RandomQuestions = ActivePack?.Questions.ToList() ?? new List<Question>();
        //    RandomQuestions = RandomQuestions.OrderBy(q => random.Next()).ToList();

        //    if (RandomQuestions.Count > 0)
        //    {
        //        CurrentQuestionIndex = 0;
        //        RaisePropertyChanged(nameof(SelectedQuestion));
        //        RandomiseActiveQuestionAnswers(CurrentQuestionIndex);
        //    }

        //    if (ActivePack != null)
        //    {
        //        TimeLimitInSeconds = ActivePack.TimeLimitInSeconds;
        //        _timer.Start();
        //    }
        //}
        //public void randomiseActiveQuestionAnswers(int questionIndex)
        //{
        //    if (RandomQuestions == null || RandomQuestions.Count == 0) return;
        //    var currentQuestion = RandomQuestions[questionIndex];

        //    var allAnswers = new List<AnswerViewModel>
        //    {
        //        new AnswerViewModel(currentQuestion.CorrectAnswer),
        //        new AnswerViewModel(currentQuestion.IncorrectAnswers[0]),
        //        new AnswerViewModel(currentQuestion.IncorrectAnswers[1]),
        //        new AnswerViewModel(currentQuestion.IncorrectAnswers[2])
        //    };
        //    AnswerViewModels.Clear();
        //    foreach (var ans in allAnswers.OrderBy(a => random.Next()))
        //    {
        //        AnswerViewModels.Add(ans);
        //    }
        //}
        
    }
}
