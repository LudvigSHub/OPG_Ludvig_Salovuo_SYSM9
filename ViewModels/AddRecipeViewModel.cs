using CookMaster.Infrastructure;
using CookMaster.Models;
using CookMaster.Services;
using CookMaster.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CookMaster.ViewModels
{
    public class AddRecipeViewModel : ObservableObject
    {
        private readonly UserManager _users;

        private readonly NavigationService _nav;

        private readonly RecipeManager _recipes;

        public AddRecipeViewModel(UserManager users, NavigationService nav, RecipeManager recipe)
        {
            _users = users;

            _nav = nav;

            _recipes = recipe;

            AddCommand = new RelayCommand(_ => AddRecipe(), _ => CanAddRecipe);

            CancelCommand = new RelayCommand(_ => Cancel());

        }

        public ICommand AddCommand { get; }

        public ICommand CancelCommand { get; }

        private string _newTitle = "";
        public string NewTitle
        {
            get => _newTitle;
            set
            {
                if (_newTitle == value) return;
                _newTitle = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAddRecipe));
                CommandManager.InvalidateRequerySuggested();
            }
        }
        
        private string _newInstructions = "";
        public string NewInstructions
        {
            get => _newInstructions;
            set
            {
                if (_newInstructions == value) return;
                _newInstructions = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAddRecipe));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _newIngredientsText = "";
        public string NewIngredientsText
        {
            get => _newIngredientsText;
            set
            {
                if (_newIngredientsText == value) return;
                _newIngredientsText = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAddRecipe));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private RecipeCategory? _selectedCategory = null;
        public RecipeCategory? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory == value) return;
                _selectedCategory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanAddRecipe));
                
            }
        }

        // Källan för Categories i combobox vid skapande av recept
        public IReadOnlyList<RecipeCategory> Categories { get; } =
        Enum.GetValues(typeof(RecipeCategory)).Cast<RecipeCategory>().ToList();

        private static List<string> ParseIngredients(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new();

            // Normalisera radslut och dela på rad, komma eller semikolon
            var tokens = input.Replace("\r\n", "\n")
                              .Split(new[] { '\n', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                              .Select(s => s.Trim())
                              .Where(s => s.Length > 0)
                              // Ta bort ev. bullets/numrering i början: "1. ", "2) ", "- ", "• "
                              .Select(s => Regex.Replace(s, @"^\s*(?:\d+[.)]\s*|[-•]\s*)", ""))
                              .ToList();

            return tokens;
        }

        // Normalisera instruktionstext
        // Tar bort extra tomrader och normaliserar radslut
        private static string Normalize(string? s)
        {
            // s står för instruktionstexten
            // Om s är null → ersätt med tom sträng.
            //Trim() tar bort tomrum i början / slutet(mellanslag, tabbar, radbrytningar).
            var t = (s ?? string.Empty).Trim();

            // normalisera radslut
            t = t.Replace("\r\n", "\n").Replace("\r", "\n");

            //Delar texten till en lista av rader.
            // ta bort extra tomrader (valfritt)
            var lines = t.Split('\n')
                         .Select(x => x.TrimEnd())
                         .ToList();
            for (int i = lines.Count - 2; i >= 0; i--)
                if (string.IsNullOrWhiteSpace(lines[i]) && string.IsNullOrWhiteSpace(lines[i + 1]))
                    lines.RemoveAt(i + 1);
            //Slår ihop raderna med \r\n(Windows - standard).
            return string.Join("\r\n", lines);
        }

        private void AddRecipe()
        {
            var r1 = new Recipe
            {
                Title = NewTitle,
                Instructions = Normalize(NewInstructions),
                Ingredients = ParseIngredients(NewIngredientsText),
                Category = SelectedCategory!.Value,
                CreatedUtc = DateTime.UtcNow

            };

            _recipes.AddForCurrentUser(r1);
            var vm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(vm);
        }

        private void Cancel()
        {
            var vm = new RecipeListViewModel(_users, _nav, _recipes);
            _nav.NavigateTo<RecipeListView>(vm);
        }

        public bool CanAddRecipe =>
        
          !string.IsNullOrWhiteSpace(NewTitle) &&
          !string.IsNullOrWhiteSpace(NewInstructions) &&
          !string.IsNullOrWhiteSpace(NewIngredientsText) &&
          SelectedCategory is not null;

       

    }
}
