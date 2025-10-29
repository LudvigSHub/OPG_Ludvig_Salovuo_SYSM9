using CookMaster.Infrastructure;
using CookMaster.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Services
{
    public class RecipeManager : ObservableObject
    {
        private readonly ObservableCollection<Recipe> _recipes = new();
        public ReadOnlyObservableCollection<Recipe> Recipes { get; }

        public RecipeManager()
        {
            Recipes = new ReadOnlyObservableCollection<Recipe>(_recipes);

            _recipes.Add(new Recipe
            {
                
                Title = "Spaghetti Bolognese",
                Ingredients = new List<string> { "Spaghetti", "Ground beef", "Tomato sauce", "Onion", "Garlic", "Olive oil", "Salt", "Pepper" },
                Instructions = "1. Cook spaghetti according to package instructions.\n2. In a pan, heat olive oil and sauté onion and garlic until translucent.\n3. Add ground beef and cook until browned.\n4. Pour in tomato sauce and simmer for 15 minutes.\n5. Season with salt and pepper to taste.\n6. Serve sauce over spaghetti.",
                Category = RecipeCategory.MainCourse,
                OwnerUsername = "user"

            });
            // UserManager.CurrentUser.RecipeList = Recipes
        }

        public void AddRecipe(Recipe recipe)
        {
            if (recipe == null) return;
            _recipes.Add(recipe);
        }

        public bool RemoveRecipe(Recipe recipe)
        {
            if (recipe == null) return false;
            return _recipes.Remove(recipe);
        }
    }
}
