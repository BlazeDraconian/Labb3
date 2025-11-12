using Labb3.Command;
using Labb3.Models;
using Labb3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Labb3.Dialogs
{
    public partial class CreateNewPackDialog : Window
    {
        public CreateNewPackDialogViewModel ViewModel { get; }

        public CreateNewPackDialog()
        {
            InitializeComponent();
            ViewModel = new CreateNewPackDialogViewModel();
            DataContext = ViewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }
        public CreateNewPackDialog(QuestionPackViewModel existingPack) : this()
        {
            InitializeComponent();
            ViewModel = new CreateNewPackDialogViewModel(existingPack);
            DataContext = ViewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            CreateButton.Content = "Update";
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.DialogResult))
            {
                if (ViewModel.DialogResult.HasValue)
                {

                    this.DialogResult = ViewModel.DialogResult;
                    this.Close();

                }
            }


        }


    }


}
