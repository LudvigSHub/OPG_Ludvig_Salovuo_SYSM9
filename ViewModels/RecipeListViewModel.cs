using CookMaster.Infrastructure;
using CookMaster.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.ViewModels
{
    internal class RecipeListViewModel : ObservableObject
    {
        private readonly UserManager _users;

        private readonly NavigationService _nav;

        public RecipeListViewModel(UserManager users, NavigationService nav)
        {
            _users = users;
            
            _nav = nav;
        }

        
    }
}
