using CookMaster.Infrastructure;
using CookMaster.Models;
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


        // bool property för att växla UI mellan visningsläge och redigeringsläge
        // I XAML visar/döljer man sektioner via Visibility + BooleanToVisibilityConverter eller sätter IsReadOnly
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set { _isEditing = value; OnPropertyChanged(); }
        }

        // Properties för redigering av recept
        // temporära lagringsplatser för redigeringsfält
        // dessa används när användaren klickar på "Edit"
        public string EditTitle { get; set; } = "";
        public RecipeCategory EditCategory { get; set; }
        public string EditInstructions { get; set; } = "";

        // en multirad-sträng som mappar listan
        // Ingredients i Recipe till en sträng med radbrytningar
        // Getter sätts i StartEdit(): string.Join(Environment.NewLine, SelectedRecipe.Ingredients)
        public string EditIngredientsText { get; set; } = "";

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }


        public RecipeDetailsViewModel(UserManager users, NavigationService nav, RecipeManager recipes, Recipe recipe)
        {
            _users = users;

            _nav = nav;

            _recipes = recipes;

            SelectedRecipe = recipe ?? throw new ArgumentNullException(nameof(recipe));

            EditCommand = new RelayCommand(_ => StartEdit(), _ => CanEdit());
            SaveCommand = new RelayCommand(_ => Save(), _ => IsEditing);
            CancelCommand = new RelayCommand(_ => Cancel(), _ => IsEditing);
        }

        // källan för kategorier i combobox vid redigering/skapande av recept
        public IReadOnlyList<RecipeCategory> Categories { get; } =
        Enum.GetValues(typeof(RecipeCategory)).Cast<RecipeCategory>().ToList();

        private bool CanEdit()
        {
            var u = _users.CurrentUser;
            if (u is null) return false;
            if (u.IsAdmin) return true;
            return string.Equals(SelectedRecipe.OwnerUsername, u.Username, StringComparison.OrdinalIgnoreCase);
        }


        // Fyller temporära redigeringsfält med data från SelectedRecipe
        // UI kan visa dessa direkt när IsEditing sätts till true
        private void StartEdit()
        {
            EditTitle = SelectedRecipe.Title;
            EditCategory = SelectedRecipe.Category;
            EditInstructions = SelectedRecipe.Instructions ?? "";
            EditIngredientsText = string.Join(Environment.NewLine, SelectedRecipe.Ingredients ?? new());

            // signalera UI om du visar dessa direkt
            OnPropertyChanged(nameof(EditTitle));
            OnPropertyChanged(nameof(EditCategory));
            OnPropertyChanged(nameof(EditInstructions));
            OnPropertyChanged(nameof(EditIngredientsText));

            IsEditing = true;
        }

        private void Save()
        {
            //  Validering
            if (string.IsNullOrWhiteSpace(EditTitle))
                return; 

            // Skriv tillbaka till modellen
            SelectedRecipe.Title = EditTitle.Trim();
            SelectedRecipe.Category = EditCategory;
            SelectedRecipe.Instructions = EditInstructions;

            SelectedRecipe.Ingredients = (EditIngredientsText ?? "")
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();

            // Uppdatera UI om Recipe inte notifierar själv
            OnPropertyChanged(nameof(SelectedRecipe));

            var vm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(vm);
        }

        private void Cancel()
        {
            IsEditing = false; 
        }

        


    }
}
