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

        // UserManager för att kunna kolla inloggad användare och dess rättigheter
        private readonly UserManager _users;

        public UserManager Users => _users;

        // NavigationService för att kunna navigera till andra vyer
        private readonly NavigationService _nav;

        // RecipeManager för att kunna hämta och hantera recept
        public RecipeManager _recipes;

        // View för att binda till i vyn (ListBox, DataGrid, etc)
        // Används för att kunna filtrera recept baserat på inloggad användare
        public ICollectionView RecipesView { get; }

        // Property för att hålla reda på det valda receptet i UI
        private Recipe? _selectedRecipe;
        // Bunden till vald rad i t.ex. en ListBox eller DataGrid
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

            // Skapa en CollectionView för receptsamlingen
            // 
            RecipesView = CollectionViewSource.GetDefaultView(_recipes.Recipes);
            RecipesView.Filter = RecipeFilterPredicate;

            // Lyssna på byten av CurrentUser → uppdatera filter
            // 
            _users.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserManager.CurrentUser))
                    RecipesView.Refresh();
            };


            DetailsCommand = new RelayCommand(_ => OpenDetails(), _ => SelectedRecipe != null);

            AddRecipeCommand = new RelayCommand(_ => AddRecipe());

            SignOutCommand = new RelayCommand(_ => SignOut());

            RemoveCommand = new RelayCommand (_ => removeSelectedRecipe());
        }

        public ICommand DetailsCommand { get; }
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

        private void OpenDetails()
        {
            var vm = new RecipeDetailsViewModel(_users, _nav, _recipes, SelectedRecipe!);
            _nav.NavigateTo<RecipeDetailsView>(vm);
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
