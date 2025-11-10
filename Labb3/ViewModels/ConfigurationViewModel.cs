using Labb3.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Labb3.Command;
namespace Labb3.ViewModels
{
    class ConfigurationViewModel: ViewModelBase
    {
        private readonly MainWindowViewModel? _mainWindowViewModel;
        public DelegateCommand ChoosePackCommand { get; }

        public ConfigurationViewModel(MainWindowViewModel? mainWindowViewModel)
        {
            this._mainWindowViewModel = mainWindowViewModel;

            ChoosePackCommand = new DelegateCommand(ChoosePack);
        }

        public QuestionPackViewModel? ActivePack
        {
            get => _mainWindowViewModel.ActivePack;
            set
            {
                _mainWindowViewModel.ActivePack = value;
                RaisePropertyChanged();
            }
        }


        private void ChoosePack(object? obj)
        {
            if (obj is QuestionPackViewModel pack)
            {
                ActivePack = pack;
            }
            
        }
    }
}
