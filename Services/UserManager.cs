using CookMaster.Infrastructure;
using CookMaster.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Services
{
    public class UserManager : ObservableObject
    {
        private readonly List<User> _users = new();

        public IReadOnlyList<User> Users => _users;

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            private set
            {
                if (_currentUser == value) return;
                _currentUser = value;
                OnPropertyChanged();                    // CurrentUser
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        public bool IsAuthenticated => CurrentUser is not null;

        public UserManager()
        {
            // seed så du kan testa direkt
            _users.Add(new User { Username = "admin", Password = "1234", Country = "SE" });
            _users.Add(new User { Username = "user", Password = "1234", Country = "SE" });
        }

        public bool Login(string username, string password)
        {
            var u = _users.FirstOrDefault(x =>
                x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);

            if (u is null) return false;
            CurrentUser = u;
            return true;
        }

        public void Logout() => CurrentUser = null;

        public bool Register(User newUser)
        {
            if (string.IsNullOrWhiteSpace(newUser?.Username) || string.IsNullOrWhiteSpace(newUser?.Password))
                return false;

            if (_users.Any(x => x.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase)))
                return false;

            _users.Add(newUser);
            return true;
        }

        public User? FindUser(string username) =>
            _users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}
