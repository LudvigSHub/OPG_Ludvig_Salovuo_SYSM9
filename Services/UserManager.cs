using CookMaster.Infrastructure;
using CookMaster.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CookMaster.Services
{
    public class UserManager : ObservableObject
    {
        // Skapar en ny privat, modifierbar samling av users
        private readonly ObservableCollection<User> _users = new();

        // Exponerar en ReadOnlyObservableCollection av users
        public ReadOnlyObservableCollection<User> Users { get; }

        private User? _currentUser;
        public User? CurrentUser
        {
            get => _currentUser;
            private set
            {
                if (_currentUser == value) return;
                _currentUser = value;
                OnPropertyChanged();                    
                OnPropertyChanged(nameof(IsAuthenticated));
            }
        }

        // Property som indikerar om en användare är inloggad
        // Returnerar true om CurrentUser inte är null
        public bool IsAuthenticated => CurrentUser is not null;

        // Konstruktorn initierar UserManager med några fördefinierade användare
        public UserManager()
        {
            Users = new ReadOnlyObservableCollection<User>(_users);

            _users.Add(new Admin { Username = "admin", Password = "1234", Country = "Sweden",
                SecurityQuestion = "Your favorite color?",
                SecurityAnswer = "blue"
            });


            _users.Add(new User { Username = "user", Password = "1234", Country = "Sweden",
                SecurityQuestion = "Your favorite color?",
                SecurityAnswer = "blue"
            });
        }

        // Metod för att logga in en användare
        // används i MainViewModel
        public bool Login(string username, string password)
        {
            var u = _users.FirstOrDefault(x =>
                x.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                x.Password == password);

            if (u is null) return false;
            CurrentUser = u;
            return true;
        }




        // Metod för att logga ut den aktuella användaren
        
        public void Logout() => CurrentUser = null;

        // Metod för att registrera en ny användare.
        public bool Register(User newUser, out string error)
        {

            error = string.Empty;
            // flera if satser för att validera lösenordet
            // ger olika felmeddelanden beroende på vad som saknas

            // Kontrollera att newUser inte är null
            if (string.IsNullOrWhiteSpace(newUser.Password))
            {
                error = "Password is required.";
                return false;
            }
            

            // Lösenordskrav: minst 8 tecken
            if (newUser?.Password.Length < 8)
            {
                error = "Password has to be atleast 8 characters!";
                return false;
            }

            // Lösenordskrav: minst en stor bokstav
            if (!newUser.Password.Any(char.IsDigit))
            {
                error = "Password must contain at least one number.";
                return false;
            }

            // Lösenordskrav: måste innehålla ett specialtecken
            string specialChars = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/`~";
            if (!newUser.Password.Any(specialChars.Contains))
            {
                error = "Password must contain at least one special character.";
                return false;
            }

            // Kolla att användarnamnet inte redan finns (case-insensitive)
            // Använder StringComparison.OrdinalIgnoreCase för att ignorera skillnad mellan stora och små bokstäver
            // exempel: "User" och "user" betraktas som samma användarnamn
            // Linq-metoden Any returnerar true om någon användare matchar villkoret
            if (_users.Any(x => x.Username.Equals(newUser.Username, StringComparison.OrdinalIgnoreCase))) { 
                error = "Username already exists.";
            return false;
            }

            // Lägg till den nya användaren i listan
            _users.Add(newUser);
            return true;
        }

        // Metod för att uppdatera en befintlig användare
        // Validerar nya värden och uppdaterar användarens egenskaper om allt är okej
        // Används i UserDetailsViewModel
        public bool UpdateUser(User user,
                       string? newUsername,
                       string? newPassword,
                       string? confirmPassword,
                       string? newCountry,
                       out string error)
        {
            // Om user är null, returnera false med felmeddelande
            error = string.Empty;
            if (user is null) { error = "No user."; return false; }

            // Username 
            if (!string.IsNullOrWhiteSpace(newUsername) &&
                !newUsername.Equals(user.Username, StringComparison.Ordinal))
            {
                var u = newUsername.Trim();
                if (u.Length < 3)
                {
                    error = "Username must be at least 3 characters.";
                    return false;
                }

                // ej tillåtet om taget av annan
                if (_users.Any(x => !ReferenceEquals(x, user) &&
                                    x.Username.Equals(u, StringComparison.OrdinalIgnoreCase)))
                {
                    error = "Username already exists.";
                    return false;
                }
            }

            //  Password 
            var pwChangeRequested = !string.IsNullOrEmpty(newPassword) || !string.IsNullOrEmpty(confirmPassword);
            if (pwChangeRequested)
            {
                
                // tecken, minst en siffra, minst ett specialtecken)
                var pw = newPassword ?? string.Empty;

                if (string.IsNullOrWhiteSpace(pw))
                {
                    error = "Password is required.";
                    return false;
                }

                if (pw.Length < 8)
                {
                    error = "Password has to be at least 8 characters!";
                    return false;
                }

                if (!pw.Any(char.IsDigit))
                {
                    error = "Password must contain at least one number.";
                    return false;
                }

                const string specialChars = "!@#$%^&*()-_=+[]{}|;:'\",.<>?/`~";
                if (!pw.Any(specialChars.Contains))
                {
                    error = "Password must contain at least one special character.";
                    return false;
                }

                if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
                {
                    error = "New password and confirmation do not match.";
                    return false;
                }
            }

            // Country
            // måste vara ifyllt
            if (string.IsNullOrWhiteSpace(newCountry))
            {
                error = "Country is required.";
                return false;
            }

            // om allt ok, uppdatera användaren
            if (!string.IsNullOrWhiteSpace(newUsername) &&
                !newUsername.Equals(user.Username, StringComparison.Ordinal))
            {
                user.Username = newUsername.Trim();
            }

            if (pwChangeRequested)
                user.Password = newPassword!;

            user.Country = newCountry;

            return true;
        }

        // Hämtar säkerhetsfrågan för en given användare
        // Returnerar null om användaren inte finns eller ingen fråga är satt
        // Används i ForgotPasswordViewModel
        public string? GetSecurityQuestion(string username) =>
            _users.FirstOrDefault(u => u.Username == username)?.SecurityQuestion;

        // Verifierar om det angivna svaret matchar användarens sparade svar
        // Returnerar true om svaret är korrekt, annars false
        // används i ForgotPasswordViewModel
        public bool VerifySecurityAnswer(string username, string answer) =>
            string.Equals(_users.FirstOrDefault(u => u.Username == username)?.SecurityAnswer,
                          answer, StringComparison.Ordinal);

        // Uppdaterar lösenordet för en given användare
        // används i ForgotPasswordViewModel
        public void UpdatePassword(string username, string newPassword)
        {
            var u = _users.FirstOrDefault(x => x.Username == username);
            if (u is null) return;
            u.Password = newPassword;
        }

        // Hittar och returnerar en användare baserat på användarnamn
        public User? FindUser(string username) =>
            _users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}



