using Labb3.Command;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Labb3.ViewModels
{

       public class CreateNewPackDialogViewModel : ViewModelBase
        {
            public string PackName { get; set; } = "";
            public List<Difficulty> Difficulties { get; } = new List<Difficulty>() { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard };
          
            public Difficulty Difficulty { get; set; } = Difficulty.Medium;

            private int _timeLimitInSeconds = 30;
            public int TimeLimitInSeconds
            {
                get => _timeLimitInSeconds;
                set
                {
                if (_timeLimitInSeconds == value) return;
                _timeLimitInSeconds = value;
                RaisePropertyChanged(); 
                }
            }



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
            PackName = existingPack.Name;
            Difficulty = existingPack.Difficulty;
            TimeLimitInSeconds = existingPack.TimeLimitInSeconds;

            CreateCommand = new DelegateCommand((_) =>
            {
                
                existingPack.Name = PackName;
                existingPack.Difficulty = Difficulty;
                existingPack.TimeLimitInSeconds = TimeLimitInSeconds;

                DialogResult = true;
            });

            CancelCommand = new DelegateCommand((_) =>
            {
                DialogResult = false;
                
            });
        }
    }
}


