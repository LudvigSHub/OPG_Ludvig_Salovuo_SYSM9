using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.ViewModels
{
    public class AddRecipeViewModel
    {
        private readonly UserManager _users;

        private readonly NavigationService _nav;

        public AddRecipeViewModel(UserManager users, NavigationService nav)
        {
            _users = users;

            _nav = nav;

            
        }
    }
}
