using CookMaster.Infrastructure;
using CookMaster.Services;
using CookMaster.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class InfoViewModel : ObservableObject
    {
        private readonly UserManager _users;
        private readonly RecipeManager _recipes;
        private readonly NavigationService _nav;

        public InfoViewModel(UserManager users, RecipeManager recipes, NavigationService nav)
        {
            _users = users;
            _recipes = recipes;
            _nav = nav;


            CloseCommand = new RelayCommand(_ => Close());
        }

        public ICommand CloseCommand { get; }

        private void Close()
        {
            var vm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(vm);
        }
    }
}
