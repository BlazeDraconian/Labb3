using Labb3.Command;
using Labb3.Models;

namespace Labb3.ViewModels
{

    public class CreateNewPackDialogViewModel : ViewModelBase
    {

        public QuestionPackViewModel NewPack { get; set; } = new QuestionPackViewModel(new QuestionPack("Enter Name"));



        public DelegateCommand CreateCommand { get; }
        public DelegateCommand CancelCommand { get; }



        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set
            {
                if (_dialogResult == value) return;
                _dialogResult = value;
                RaisePropertyChanged(nameof(DialogResult));
            }
        }

        public CreateNewPackDialogViewModel()
        {
            CreateCommand = new DelegateCommand(Create);
            CancelCommand = new DelegateCommand(Cancel);
        }


        private void Create(object? obj)
        {
            DialogResult = true;

        }

        private void Cancel(object? obj)
        {
            DialogResult = false;

        }

        public CreateNewPackDialogViewModel(QuestionPackViewModel existingPack)
        {
            NewPack.Name = existingPack.Name;
            NewPack.Difficulty = existingPack.Difficulty;
            NewPack.TimeLimitInSeconds = existingPack.TimeLimitInSeconds;

            CreateCommand = new DelegateCommand((_) =>
            {

                existingPack.Name = NewPack.Name;
                existingPack.Difficulty = NewPack.Difficulty;
                existingPack.TimeLimitInSeconds = NewPack.TimeLimitInSeconds;

                DialogResult = true;
                RaisePropertyChanged(nameof(DialogResult));
            });

            CancelCommand = new DelegateCommand((_) =>
            {
                DialogResult = false;
                RaisePropertyChanged(nameof(DialogResult));
            });
        }
    }
}


