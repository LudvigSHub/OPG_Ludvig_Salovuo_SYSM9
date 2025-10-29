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

        private readonly ObservableCollection<Recipe> _recipes = new();
        public ReadOnlyObservableCollection<Recipe> Recipes { get; }

        public RecipeManager(UserManager users)
        {
            _users = users;
            Recipes = new ReadOnlyObservableCollection<Recipe>(_recipes);
        }

        // 1) Seeda efter login (endast om user saknar recept)
        public void SeedForCurrentUser()
        {
            var u = _users.CurrentUser;
            if (u is null || u.RecipeList.Count > 0) return;

            var r1 = new Recipe
            {
                Title = "Spaghetti Bolognese",
                Category = RecipeCategory.MainCourse,
                CreatedUtc = DateTime.UtcNow,
                OwnerUsername = u.Username
            };
            var r2 = new Recipe
            {
                Title = "Risotto",
                Category = RecipeCategory.MainCourse,
                CreatedUtc = DateTime.UtcNow,
                OwnerUsername = u.Username
            };

            _recipes.Add(r1); _recipes.Add(r2);    // central källa
            u.RecipeList.Add(r1); u.RecipeList.Add(r2); // min-vy för current user
        }

        // 2) Synka user-listan från centrala källan (kan kallas vid login/byte user)
        public void SyncUserList()
        {
            var u = _users.CurrentUser;
            if (u is null) return;

            var mine = _recipes
                .Where(r => r.OwnerUsername.Equals(u.Username, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (mine.Count == 0) return; // <-- behåll befintlig lista om centrala källan saknar poster

            u.RecipeList.Clear();
            foreach (var r in mine)
                u.RecipeList.Add(r);
        }

        // 3) Lägg för current user (uppdatera båda samlingar)
        public void AddForCurrentUser(Recipe recipe)
        {
            var u = _users.CurrentUser ?? throw new InvalidOperationException("No user");
            recipe.OwnerUsername = u.Username;
            _recipes.Add(recipe);
            u.RecipeList.Add(recipe);
        }

        // 4) Ta bort (uppdatera båda)
        public bool Remove(Recipe recipe)
        {
            if (recipe is null) return false;
            _users.CurrentUser?.RecipeList.Remove(recipe);
            return _recipes.Remove(recipe);
        }
    }
}
