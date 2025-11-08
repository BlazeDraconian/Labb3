using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb3.ViewModels
{
    class MainWindowViewModel: ViewModelBase
    {
		private QuestionPackViewModel _selectedPack;

		public QuestionPackViewModel SelectedPack
		{
			get => _selectedPack;
			set
			{
				_selectedPack = value;
				RaisePropertyChanged();
			}

		}

		public ViewModelBase Model { get; set; }
        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

		private QuestionPackViewModel _activePack;

		public QuestionPackViewModel ActivePack
		{
			get =>_activePack;
			set { 
				_activePack = value;
				RaisePropertyChanged();
				PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
				}
		}

        public PlayerViewModel? PlayerViewModel { get; set; }
		public ConfigurationViewModel ConfigurationViewModel { get;}

        public MainWindowViewModel()
		{
			PlayerViewModel = new PlayerViewModel(this);
			ConfigurationViewModel = new ConfigurationViewModel(this);
            new PlayerViewModel(this);
            var pack = new QuestionPack("MyQuestionPack");
            ActivePack = new QuestionPackViewModel(pack);
            ActivePack.Questions.Add(new Question($"Vad heter Sveriges huvudstad?", "Stockholm", "Göteborg", " Malmö", "Helsingborg"));
		}
	}
}
