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
            var dialog = new CreateNewPackDialog();
            var result = dialog.ShowDialog();

            if (result == true)
            {
                var vm = dialog.ViewModel;

                var newPack = new QuestionPack(vm.PackName, vm.Difficulty, vm.TimeLimitInSeconds);
                var newPackVM = new QuestionPackViewModel(newPack);

                _mainWindowViewModel.Packs.Add(newPackVM);
                _mainWindowViewModel.ActivePack = newPackVM;
            }
        }  
        

        public void packOptions(object? obj)
        {
            var dialog = new CreateNewPackDialog(_mainWindowViewModel.ActivePack);
            dialog.ShowDialog();
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

            _mainWindowViewModel.ActivePack = _mainWindowViewModel.Packs.First();
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
            if (ActivePack.SelectedQuestion != null)
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
    }
}
