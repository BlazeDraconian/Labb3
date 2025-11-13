using Labb3.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace Labb3.ViewModels
{

        public class QuestionPackViewModel : ViewModelBase
        {
        
        private readonly QuestionPack _model;
       
        public QuestionPackViewModel(QuestionPack model)
        {
            _model = model;
            Questions = new ObservableCollection<Question>(_model.Questions);
            Questions.CollectionChanged += Questions_CollectionChanged;

            foreach (var q in Questions)
                q.PropertyChanged += Question_PropertyChanged;
        }

        private void Questions_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (Question q in e.NewItems)
                {
                    _model.Questions.Add(q);
                    q.PropertyChanged += Question_PropertyChanged;
                }      
            }

            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (Question q in e.OldItems)
                {
                    _model.Questions.Remove(q);
                    q.PropertyChanged -= Question_PropertyChanged;
                }
                    
            }

            if (e.Action == NotifyCollectionChangedAction.Replace && e.OldItems != null && e.NewItems != null)
            {
                _model.Questions[e.OldStartingIndex] = (Question)e.NewItems[0]!;
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _model.Questions.Clear();
            }

            var mwvm = App.Current.MainWindow.DataContext as MainWindowViewModel;
            mwvm?.SavePacksToFile();
        }

        public string Name
        {
            get => _model.Name;
            set {_model.Name = value; 
                
                RaisePropertyChanged(); }
           
        }

        public Difficulty difficulty
        {
            get => _model.Difficulty;
            set { _model.Difficulty = value;
                RaisePropertyChanged();
            }
        }

        public int TimeLimitInSeconds
        {
            get => _model.TimeLimitInSeconds;
            set
            {
                if (_model.TimeLimitInSeconds != value)
                {
                    _model.TimeLimitInSeconds = value;
                    RaisePropertyChanged();
                }
               
            }
        }

        private Question? _selectedQuestion;

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                RaisePropertyChanged();
            }

        }

        public Difficulty Difficulty
        {
            get =>_model.Difficulty;
            set
            {
                _model.Difficulty = value;
                RaisePropertyChanged();
            }
        }

        private void Question_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            
            var mwvm = App.Current.MainWindow.DataContext as MainWindowViewModel;
            mwvm?.SavePacksToFile();
        }


        public ObservableCollection<Question> Questions { get; set; }

        public QuestionPack Model => _model;
    }
}
