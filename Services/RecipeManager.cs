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
        private readonly UserManager _users;


        // Receptsamlingen 
        // Den privata, modiferbara samlingen
        private readonly ObservableCollection<Recipe> _recipes = new();
        public ReadOnlyObservableCollection<Recipe> Recipes { get; }

        public RecipeManager(UserManager users)
        {
            _users = users;
            Recipes = new ReadOnlyObservableCollection<Recipe>(_recipes);
        }


        // Skapar globala startrecept som alla kan se
        // Körs endast en gång
        // Körs i app.cs vid uppstart
        public void SeedDefaultsOnce()
        {
            var r3 = new Recipe
            {
                Title = "Pancakes",
                Ingredients = new List<string>
                {
                    "200g all-purpose flour",
                    "2 eggs",
                    "300ml milk",
                    "1 tbsp sugar",
                    "1 tsp baking powder",
                    "Pinch of salt",
                    "Butter for cooking"
                },
                Category = RecipeCategory.Dessert,
                CreatedUtc = DateTime.UtcNow,
                OwnerUsername = null // Globalt recept, alla skall kunna se det, så OwnerUsername är null
            };

            var r4 = new Recipe
            {
                Title = "Caesar Salad",
                Ingredients = new List<string>
                {
                    "1 head romaine lettuce",
                    "50g Parmesan cheese",
                    "100g croutons",
                    "2 chicken breasts",
                    "Caesar dressing",
                    "Salt",
                    "Pepper"
                },
                Category = RecipeCategory.Salad,
                CreatedUtc = DateTime.UtcNow,
                OwnerUsername = null // Globalt recept, alla skall kunna se det, så OwnerUsername är null
            };

            _recipes.Add(r3);

            _recipes.Add(r4);

        }

        // skapar startrecept för den redan skapade användaren "user"
        public void SeedForUser(User user)
        {
            // Om metoden kallas med null, gör inget
            if (user is null)
                return;

            // Kolla om det redan finns recept i _recipes som ägs av just denna användare.
            // Om det redan finns då har vi redan seedat för denna user och ska inte göra det igen.
            bool hasAnyForUser = _recipes.Any(r =>
                r.OwnerUsername != null &&
                r.OwnerUsername.Equals(user.Username, StringComparison.OrdinalIgnoreCase));


            if (hasAnyForUser) return; // redan seedad, avsluta här om så är fallet

            // Startreceot för denna user
            var r1 = new Recipe
            {
                Title = "Spaghetti Bolognese",
                Ingredients = new List<string>
                {
                    "200g spaghetti",
                    "100g minced beef",
                    "1 can tomato sauce",
                    "White wine",
                    "1 onion",
                    "2 cloves garlic",
                    "Salt",
                    "Pepper",
                    "Olive oil"
                },
                Instructions = "1. Cook spaghetti according to package instructions.\n" +
                               "2. In a pan, heat olive oil and sauté chopped onions and garlic until translucent.\n" +
                               
                               "3. Add minced beef and cook until browned.\n" +
                               "4. Add tomato paste and let it cook for a few minutes. \n" +
                               "5. Add white wine and let it reduce slightly.\n" +
                               "6. Pour in tomato sauce, season with salt and pepper, and let it simmer for 15 minutes.\n" +
                               "7. Taste the sauce and if needed add either: salt, sugar or vinegar. \n" +
                               "8. Serve the sauce over the cooked spaghetti.\n",
                               

                Category = RecipeCategory.MainCourse,
                CreatedUtc = DateTime.UtcNow,
                OwnerUsername = user.Username
            };

            // Andra receptet för denna user
            var r2 = new Recipe
            {
                Title = "Risotto",
                Ingredients = new List<string>
                {
                    "200g Arborio rice",
                    "1 liter chicken broth",
                    "1 onion",
                    "100ml white wine",
                    "50g Parmesan cheese",
                    "2 tbsp butter",
                    "Salt",
                    "Pepper"
                },
                Category = RecipeCategory.MainCourse,
                CreatedUtc = DateTime.UtcNow,
                OwnerUsername = user.Username
            };

            _recipes.Add(r1);
            _recipes.Add(r2);
        }

        

        //Lägg för current user 
        public void AddForCurrentUser(Recipe recipe)
        {
            var u = _users.CurrentUser ?? throw new InvalidOperationException("No user");
            recipe.OwnerUsername = u.Username;
            _recipes.Add(recipe);
            
        }

        //Ta bort 
        public bool Remove(Recipe recipe)
        {
            if (recipe is null) return false;
            
            return _recipes.Remove(recipe);
        }
    }
}
