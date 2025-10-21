using CookMaster.Infrastructure;
using CookMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly UserManager _users;

        // ⬅️ DEN HÄR SAKNADES
        public MainViewModel(UserManager users)
        {
            _users = users;
            LoginCommand = new RelayCommand(_ => Login(), _ => CanLogin);
        }

        // --- enkel login-skelett ---
        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanLogin)); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanLogin)); }
        }

        public bool CanLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        public ICommand LoginCommand { get; }

        private string _message = "";
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }

        private void Login()
        {
            Message = _users.Login(Username, Password) ? "Login OK" : "Fel användarnamn/lösenord.";
        }

    }
}
