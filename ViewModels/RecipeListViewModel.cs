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
            RecipesView = CollectionViewSource.GetDefaultView(_recipes.Recipes);
            RecipesView.Filter = CombinedFilter;

            // Lyssna på byten av CurrentUser → uppdatera filter
            // 
            _users.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserManager.CurrentUser))
                    RecipesView.Refresh();
            };


            RecipeDetailsCommand = new RelayCommand(_ => RecipeDetails(), _ => SelectedRecipe != null);

            AddRecipeCommand = new RelayCommand(_ => AddRecipe());

            SignOutCommand = new RelayCommand(_ => SignOut());

            RemoveCommand = new RelayCommand(_ => removeSelectedRecipe(), _ => SelectedRecipe != null);

            UserDetailsCommand = new RelayCommand(_ => UserDetails());

            InfoCommand = new RelayCommand(_ => ShowInfo());

            ClearFiltersCommand = new RelayCommand(_ => ClearFilters());



        }

        // Källan för Categories i combobox vid search categories
        public IReadOnlyList<RecipeCategory> Categories { get; } =
        Enum.GetValues(typeof(RecipeCategory)).Cast<RecipeCategory>().ToList();

        private string? _searchText;
        public string? SearchText
        {
            get => _searchText;
            set { if (_searchText == value) return; _searchText = value; OnPropertyChanged(); RecipesView.Refresh(); }
        }

        private RecipeCategory? _selectedCategory;
        public RecipeCategory? SelectedCategory
        {
            get => _selectedCategory;
            set { if (_selectedCategory == value) return; _selectedCategory = value; OnPropertyChanged(); RecipesView.Refresh(); }
        }

        private DateTime? _exactDate;
        public DateTime? ExactDate
        {
            get => _exactDate;
            set { if (_exactDate == value) return; _exactDate = value; OnPropertyChanged(); RecipesView.Refresh(); }
        }




        public ICommand RecipeDetailsCommand { get; }
        public ICommand AddRecipeCommand { get; }
        public ICommand RemoveCommand { get; }

        public ICommand SignOutCommand { get; }

        public ICommand UserDetailsCommand { get; }

        public ICommand InfoCommand { get; }

        public ICommand ClearFiltersCommand { get; }



        public void AddRecipe()
        {
            var recipeVm = new AddRecipeViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<AddRecipeView>(recipeVm);
        }

        public void SignOut()
        {
            _users.Logout();
            var loginVm = new MainViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<MainWindow>(loginVm);
        }

        public void removeSelectedRecipe()
        {
            if (SelectedRecipe is not null)
            {
                _recipes.Remove(SelectedRecipe);
            }
        }

        private void RecipeDetails()
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

        private void UserDetails()
        {
            var vm = new UserDetailsViewModel(_users, _recipes, _nav);
            _nav.NavigateTo<UserDetailsView>(vm);
        }

        private void ShowInfo()
        {
            var vm = new InfoViewModel(_users, _recipes, _nav);
            _nav.NavigateTo<InfoView>(vm);

        }

        private bool CombinedFilter(object obj)
        {
            //Basfilter först
            // Kolla om receptet är synligt för den inloggade användaren
            if (!RecipeFilterPredicate(obj)) return false;

            var r = (Recipe)obj;

            // en filtrering i taget: Title > Category > ExactDate
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var q = SearchText!.Trim();
                return (r.Title?.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                     || (r.Instructions?.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (SelectedCategory is RecipeCategory cat)
            {
                return r.Category == cat;
            }
            else if (ExactDate is DateTime d)
            {
                return r.CreatedUtc.Date == d.Date;
            }

            // 3) Inget extra filter valt → visa allt som passerade basfiltret
            return true;
        }

        private void ClearFilters()
        {
            SearchText = null;
            SelectedCategory = null;
            ExactDate = null;



        }

    }
}
