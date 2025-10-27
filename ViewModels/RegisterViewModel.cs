using CookMaster.Infrastructure;
using CookMaster.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class RegisterViewModel : ObservableObject
    {
        private readonly UserManager _users;

        

        private readonly NavigationService _nav;

        public List<string> Countries { get; } = new() 
        { "Sweden", "Norway", "Finland", "Denmark" };
        public string SelectedCountry { get; set; }

        public ICommand RegisterCommand { get; }

        public RegisterViewModel(UserManager users, NavigationService nav)
        {
            _users = users;
            RegisterCommand = new RelayCommand(_ => Register(),_ => CanRegister);
            _nav = nav;
        }
        private string _username = string.Empty;

        // Property som din TextBox för användarnamn binder till.
        // På set: meddela UI att Username ändrats OCH att CanLogin kan ha ändrats,
        // så att t.ex. en "Logga in"-knapp kan aktiveras/inaktiveras direkt.
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanRegister)); }
        }

        private string _password = string.Empty;

        // Property som din PasswordBox/TextBox binder till för lösenord.
        // Samma logik: uppdatera UI och trigga omvärdering av CanLogin.
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanRegister)); }
        }
        public bool CanRegister =>
          !string.IsNullOrWhiteSpace(Username) &&
          !string.IsNullOrWhiteSpace(Password) &&
          SelectedCountry is not null;

        private void Register()
        {
            throw new NotImplementedException();
        }
    }
}
