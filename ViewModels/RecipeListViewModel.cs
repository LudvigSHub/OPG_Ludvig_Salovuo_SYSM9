using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using CookMaster.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class RecipeListViewModel : ObservableObject
    {
        private readonly UserManager _users;

        public UserManager Users => _users;

        private readonly NavigationService _nav;

        public RecipeManager _recipes;

        public ICollectionView RecipesView { get; }

        private Recipe? _selectedRecipe;
        public Recipe? SelectedRecipe
        {
            get => _selectedRecipe;
            set { _selectedRecipe = value; OnPropertyChanged(); }
        }

        public RecipeListViewModel(UserManager users, NavigationService nav, RecipeManager recipes)
        {
            _users = users;
            
            _nav = nav;

            _recipes = recipes;

            RecipesView = CollectionViewSource.GetDefaultView(_recipes.Recipes);
            RecipesView.Filter = RecipeFilterPredicate;

            // Lyssna på byten av CurrentUser → uppdatera filter
            _users.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserManager.CurrentUser))
                    RecipesView.Refresh();
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
                _recipes.Remove  (SelectedRecipe);
            }
        }

        private bool RecipeFilterPredicate(object obj)
        {
            if (obj is not Recipe r) return false;

            var u = _users.CurrentUser;
            if (u is null) return false;                // ingen inloggad → visa inget (eller return true om du vill)

            if (u.IsAdmin) return true;                 // admin ser allt

            return r.IsGlobal ||
             string.Equals(r.OwnerUsername, u.Username, StringComparison.OrdinalIgnoreCase);
        }

    }
}
