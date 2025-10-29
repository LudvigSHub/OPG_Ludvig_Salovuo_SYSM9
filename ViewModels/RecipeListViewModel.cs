using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using CookMaster.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class RecipeListViewModel : ObservableObject
    {
        private readonly UserManager _users;

        public UserManager Users => _users;

        private readonly NavigationService _nav;

        private ObservableCollection<Recipe>? _myRecipes;
        public ObservableCollection<Recipe>? MyRecipes
        {
            get => _myRecipes;
            private set { _myRecipes = value; OnPropertyChanged(); }
        }

        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set { _selectedRecipe = value; OnPropertyChanged(); }
        }

        public RecipeListViewModel(UserManager users, NavigationService nav)
        {
            _users = users;
            
            _nav = nav;

            MyRecipes = _users.CurrentUser?.RecipeList;

            _users.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(UserManager.CurrentUser))
                    MyRecipes = _users.CurrentUser?.RecipeList;
            };

            AddRecipeCommand = new RelayCommand(_ => AddRecipe());

            SignOutCommand = new RelayCommand(_ => SignOut());

            RemoveCommand = new RelayCommand (_ => removeSelectedRecipe());
        }

        public ICommand AddRecipeCommand { get; }
        public ICommand RemoveCommand { get; }

        public ICommand SignOutCommand { get; }



        public void AddRecipe()
        {
            var recipeVm = new AddRecipeViewModel(_users, _nav);
            _nav.NavigateTo<AddRecipeView>(recipeVm);
        }

        public void SignOut()
        {
            _users.Logout();
            var loginVm = new MainViewModel(_users, _nav, new RecipeManager(_users));
            _nav.NavigateTo<MainWindow>(loginVm);
        }
        
        public void removeSelectedRecipe()
        {
            if (SelectedRecipe is not null)
            {
                MyRecipes?.Remove(SelectedRecipe);
            }
        }

    }
}
