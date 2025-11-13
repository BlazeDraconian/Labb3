using Labb3.Command;
using Labb3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace Labb3.ViewModels
{
    class MainWindowViewModel: ViewModelBase
    {
        private const string SaveFilePath = "questionpacks.json";
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

        public ViewModelBase Model
        {
            get => _model;
            set
            {
                _model = value;
                RaisePropertyChanged();
            }
        }
        private ViewModelBase _model;
        public ObservableCollection<QuestionPackViewModel> Packs { get; } = new();

		private QuestionPackViewModel _activePack;

		public QuestionPackViewModel ActivePack
		{
			get =>_activePack;
			set { 
				_activePack = value;
				RaisePropertyChanged();
				PlayerViewModel.RaisePropertyChanged(nameof(PlayerViewModel.ActivePack));
                ConfigurationViewModel.RaisePropertyChanged(nameof(ConfigurationViewModel.ActivePack));
				}
		}



        public PlayerViewModel? PlayerViewModel { get; set; }
		public ConfigurationViewModel ConfigurationViewModel { get;}

        public DelegateCommand PlayCommand { get; }

       



        public MainWindowViewModel()
		{
			PlayerViewModel = new PlayerViewModel(this);
			ConfigurationViewModel = new ConfigurationViewModel(this);
           
            var pack = new QuestionPack("MyQuestionPack");
            ActivePack = new QuestionPackViewModel(pack);
			//ActivePack.Questions.Add(new Question($"Vad heter Sveriges huvudstad?", "Stockholm", "Göteborg", " Malmö", "Helsingborg"));
            Model = ConfigurationViewModel;
            PlayCommand = new DelegateCommand(PlayGame);
            LoadPacksFromFile();

        }


        private void PlayGame(object? obj)
        {
            Model = PlayerViewModel;
            PlayerViewModel.StartGame();
        }


        public void SavePacksToFile()
        {
            try
            {
                var packs = Packs.Select(p => p.Model).ToList(); 
                var json = JsonSerializer.Serialize(packs, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(SaveFilePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Fel vid sparning: " + ex.Message);
            }
        }

        public void LoadPacksFromFile()
        {
            if (!File.Exists(SaveFilePath))
                return;

            try
            {
                var json = File.ReadAllText(SaveFilePath);
                var packs = JsonSerializer.Deserialize<List<QuestionPack>>(json);
                if (packs == null)
                    return;

                Packs.Clear();
                foreach (var pack in packs)
                    Packs.Add(new QuestionPackViewModel(pack));

                ActivePack = Packs.FirstOrDefault();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Fel vid inläsning: " + ex.Message);
            }
        }
    }


}

