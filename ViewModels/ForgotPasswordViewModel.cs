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
    public class ForgotPasswordViewModel : ObservableObject
    {
        private readonly UserManager _users;
        private readonly NavigationService _nav;
        private readonly RecipeManager _recipes;

        // RelayCommands för att hämta security question från user, lägga till svar, sedan ändra lösenord
        public ICommand GetQuestionCommand { get; }
        public ICommand SubmitAnswerCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        private string _username = "";
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _securityAnswer = "";
        public string SecurityAnswer
        {
            get => _securityAnswer;
            set
            {
                _securityAnswer = value;
                OnPropertyChanged();
                // Talar om för WPF att köra om alla CanExecute-metoder.
                // Används när värden ändras som påverkar om en knapp ska vara aktiv (IsEnabled).
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Nya fält för nytt lösen
        private string _newPassword = "";
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanChangePassword));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _confirmNewPassword = "";
        public string ConfirmNewPassword
        {
            get => _confirmNewPassword;
            set
            {
                _confirmNewPassword = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanChangePassword));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Sätts till true när rätt säkerhetssvar lämnats
        private bool _answerVerified;
        public bool AnswerVerified
        {
            get => _answerVerified;
            set
            {
                _answerVerified = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanChangePassword));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // Bestämmer om Change password knappen ska vara aktiv.
        // Krav: säkerhetssvaret är godkänt + nytt lösenord är ifyllt och matchar bekräftelsen.
        public bool CanChangePassword =>
            AnswerVerified &&
            !string.IsNullOrWhiteSpace(NewPassword) &&
            NewPassword == ConfirmNewPassword;

        // (intern) förväntat svar
        private string? _expectedAnswer;

        public ForgotPasswordViewModel(UserManager users, NavigationService nav, RecipeManager recipes)
        {
            _users = users;
            _nav = nav;
            _recipes = recipes;
            GetQuestionCommand = new RelayCommand(_ => GetQuestion(), _ => !string.IsNullOrWhiteSpace(Username));
            SubmitAnswerCommand = new RelayCommand(_ => SubmitAnswer(), _ => !string.IsNullOrWhiteSpace(SecurityAnswer));
            ChangePasswordCommand = new RelayCommand(_ => ChangePassword(), _ => CanChangePassword);
        }

        private void GetQuestion()
        {
            var user = _users.Users.FirstOrDefault(u => u.Username == Username);
            if (user is null)
            {
                MessageBox.Show("User not found.", "Forgot password", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // nollställ state inför nytt försök
            AnswerVerified = false;
            _expectedAnswer = user.SecurityAnswer;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;

            MessageBox.Show(user.SecurityQuestion ?? "No security question set.",
                            "Security question", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Kollar om Security answer == expectedAnswer
        // eller att det expected anwser inte är null
        // Används av ForgotPasswordViewModel
        private void SubmitAnswer()
        {
            if (_expectedAnswer is null)
            {
                MessageBox.Show("Click 'Get Question' first.", "Forgot password", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.Equals(SecurityAnswer, _expectedAnswer, StringComparison.Ordinal))
            {
                AnswerVerified = false;
                MessageBox.Show("Wrong security answer.", "Forgot password", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AnswerVerified = true;
            MessageBox.Show("Answer accepted. You can now set a new password.", "Forgot password",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }
        // Bytar lösenord i ForgotPasswordViewModel
        private void ChangePassword()
        {
            // skydd: ska normalt vara sant via CanExecute
            if (!CanChangePassword) return;

            var user = _users.Users.FirstOrDefault(u => u.Username == Username);
            if (user is null)
            {
                MessageBox.Show("User not found.", "Forgot password", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _users.UpdatePassword(Username, NewPassword);
            MessageBox.Show("Password changed.", "Forgot password", MessageBoxButton.OK, MessageBoxImage.Information);


            var MainVm = new MainViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<MainWindow>(MainVm);


        }
    }
}
