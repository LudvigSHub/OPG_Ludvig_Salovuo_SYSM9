using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using CookMaster.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class UserDetailsViewModel : ObservableObject
    {
        private readonly UserManager _users;
        private readonly RecipeManager _recipes;
        private readonly NavigationService _nav;
        private readonly User _currentUser;

        public UserDetailsViewModel(UserManager users, RecipeManager recipes, NavigationService nav)
        {

            _users = users;
            _recipes = recipes;
            _nav = nav;


           
            _currentUser = _users.CurrentUser ?? throw new InvalidOperationException("No current user.");

            CurrentUsername = _currentUser.Username;
            CurrentCountry = _currentUser.Country;

            NewUsername = _currentUser.Username;
            SelectedCountry = _currentUser.Country;

            Countries = new List<string> { "Sweden", "Norway", "Denmark", "Finland" };

            SaveCommand = new RelayCommand(_ => Save());
            CancelCommand = new RelayCommand(_ => Cancel());
        }

        // --- Visningsfält ---
        public string CurrentUsername { get; }
        public string CurrentCountry { get; }

        // --- Bindbara inputfält ---
        private string _newUsername = "";
        public string NewUsername
        {
            get => _newUsername;
            set { _newUsername = value; OnPropertyChanged(); }
        }

        private string _newPassword = "";
        public string NewPassword
        {
            get => _newPassword;
            set { _newPassword = value; OnPropertyChanged(); }
        }

        private string _confirmPassword = "";
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        private string _selectedCountry = "";
        public string SelectedCountry
        {
            get => _selectedCountry;
            set { _selectedCountry = value; OnPropertyChanged(); }
        }

        public List<string> Countries { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }


        public void Save()
        {
            if (!_users.UpdateUser(_currentUser, NewUsername, NewPassword, ConfirmPassword, SelectedCountry, out string error))
            {
                MessageBox.Show(error, "User update failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show("User update successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            var vm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(vm);


        }

        public void Cancel()
        {
            var vm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(vm);
        }

    }

    
}
