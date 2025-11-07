using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using CookMaster.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly UserManager _users;

        private readonly NavigationService _nav;
        private readonly RecipeManager _recipes;

        // Konstruktorn får in UserManager via dependency injection.
        // Skapar även LoginCommand: kopplar "vad som händer" (Login)
        // och "om det är tillåtet" (CanLogin) för t.ex. en Button i XAML.
        public MainViewModel(UserManager users, NavigationService nav, RecipeManager recipes)
        {
            _users = users;
            _nav = nav;
            _recipes = recipes;
            LoginCommand = new RelayCommand(_ => Login(), _ => CanLogin);
            RegisterCommand = new RelayCommand(_ => OpenRegister());
            GetF2ACodeCommand = new RelayCommand(_ => Show2FAMessage(), _ => CanLogin);

            OpenForgotPasswordCommand = new RelayCommand(_ => OpenForgotPassword());




        }

        // --- enkel login-skelett ---
        private string _username = string.Empty;

        // Property som din TextBox för användarnamn binder till.
        // På set: meddela UI att Username ändrats OCH att CanLogin kan ha ändrats,
        // så att t.ex. en "Logga in"-knapp kan aktiveras/inaktiveras direkt.
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanLogin)); }
        }

        private string _password = string.Empty;

        // Property som din PasswordBox/TextBox binder till för lösenord.
        // Samma logik: uppdatera UI och trigga omvärdering av CanLogin.
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanLogin)); }
        }

        // Property för F2A-kod
        private string? _f2aCode;
        public string? F2ACode
        {
            get => _f2aCode;
            set { _f2aCode = value; OnPropertyChanged(); }
        }

        // Read-only property som talar om ifall login är möjligt.
        // Används av kommandots CanExecute (andra parametern till RelayCommand).
        // I XAML påverkar detta t.ex. IsEnabled på en knapp som binder till LoginCommand.
        public bool CanLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        // Själva kommandot som din Login-knapp binder till: Command="{Binding LoginCommand}"
        public ICommand LoginCommand { get; }
        // Kommandot för att öppna registreringsvyn
        public ICommand RegisterCommand { get; }

        public ICommand GetF2ACodeCommand { get; }
        public ICommand OpenForgotPasswordCommand { get; }


        private string _message = string.Empty;
        public string Message
        {
            get => _message;
            set { _message = value; OnPropertyChanged(); }
        }


        // Metod för att öppna registreringsvyn
        private void OpenRegister()
        {
            var vm = new RegisterViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RegisterView>(vm);
        }

        // Metod som körs när användaren försöker logga in.
        // Kollar med UserManager ifall inloggningen lyckas.
        // Om ja: navigera till RecipeListView.
        private void Login()
        {
            // Kolla användarnamn/lösenord
            if (!_users.Login(Username, Password))
            {
                MessageBox.Show("Wrong Username/Password.", "Login",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kräver att användaren tryckt "Get F2A code" och fyllt i koden
            if (string.IsNullOrWhiteSpace(F2ACode))
            {
                MessageBox.Show("Press 'Get F2A code', fill in the code and press 'Log in' again.",
                    "F2A krävs", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 3) Jämför mot din AuthCode
            if (F2ACode != AuthCode.ToString())
            {
                MessageBox.Show("Wrong F2A-Code. Try again!.", "F2A",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4) Allt OK → in

            MessageBox.Show("Login Successful!", "CookMaster",
                MessageBoxButton.OK, MessageBoxImage.Information);

            var recipeVm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(recipeVm);
        }
        // Den satta koden för Authenticator
        int AuthCode = 133700;

        // Visar F2A koden för användaren
        private void Show2FAMessage()
        {
           MessageBox.Show($"Your code: {AuthCode}", "F2A Code", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void OpenForgotPassword()
        {
            var vm = new ForgotPasswordViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<ForgotPasswordView>(vm);
        }


    }
}
