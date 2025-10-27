using CookMaster.Infrastructure;
using CookMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.ViewModels
{
    public class RegisterViewModel : ObservableObject
    {
        private readonly UserManager _users;

        public RelayCommand RegisterCommand { get; }

        private readonly NavigationService _nav;

        public RegisterViewModel(UserManager users, NavigationService nav)
        {
            _users = users;
            RegisterCommand = new RelayCommand(_ => Register());
            _nav = nav;
        }

        private void Register()
        {
            throw new NotImplementedException();
        }
    }
}
