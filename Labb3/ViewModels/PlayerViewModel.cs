using Labb3.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Labb3.ViewModels
{
    class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;

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

            AddQuestion = new DelegateCommand(addQuestion);
            RemoveQuestion = new DelegateCommand(removeQuestion);
            EditQuestion = new DelegateCommand(editQuestion);
            FullScreen = new DelegateCommand(fullScreen);


            DemoText = string.Empty;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick; 
            _timer.Start();    
        }
        public readonly DispatcherTimer _timer;

        private void Timer_Tick(object? sender, EventArgs e)
        {
            ActivePack.TimeLimitInSeconds -= 1;
            if (ActivePack.TimeLimitInSeconds <= 1)
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

        public void newQuestionPack(object? obj)
        {

        }

        public void selectQuestionPack(object? obj)
        {

        }

        public void deleteQuestionPack(object? obj)
        {

        }

        public void importQuestionPack(object? obj)
        {

        }

        public void addQuestion(object? obj)
        {

        }

        public void removeQuestion(object? obj)
        {

        }

        public void editQuestion(object? obj)
        {

        }

        private void PlayGame(object? obj)
        {
          
        }

        private void EndGame(object? obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void fullScreen(object? obj)
        {

        }

        private bool CanSetPackName(object? arg)
        {
            return DemoText.Length > 0;
        }

        private void SetpackName(object? obj)
        {
            ActivePack.Name = DemoText;
        }
    }
}
