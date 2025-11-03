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
    public class RecipeDetailsViewModel : ObservableObject
    {
        // UserManager för att kunna kolla inloggad användare och dess rättigheter
        private readonly UserManager _users;
        
        // NavigationService för att kunna navigera till andra vyer
        private readonly NavigationService _nav;

        // RecipeManager för att kunna hämta och hantera recept
        public RecipeManager _recipes;
        public UserManager Users => _users;
        public Recipe SelectedRecipe { get; }

        private Recipe? _selectedIngredient;
        // Bunden till vald rad i t.ex. en ListBox eller DataGrid
        public Recipe? SelectedIngredient
        {
            get => _selectedIngredient;
            set { _selectedIngredient = value; OnPropertyChanged(); }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        public string EditTitle { get; set; } = "";
        public RecipeCategory EditCategory { get; set; }
        public string EditInstructions { get; set; } = "";
        public string EditIngredientsText { get; set; } = "";

        public RecipeDetailsViewModel(UserManager users, NavigationService nav, RecipeManager recipes, Recipe recipe)
        {
            _users = users;

            _nav = nav;

            _recipes = recipes;

            SelectedRecipe = recipe ?? throw new ArgumentNullException(nameof(recipe));
        }

        public IReadOnlyList<RecipeCategory> Categories { get; } =
        Enum.GetValues(typeof(RecipeCategory)).Cast<RecipeCategory>().ToList();

        
    }
}
