using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookMaster.Models
{
    public class Recipe
    {
        public string Title { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new List<string>();
        public string Instructions { get; set; } = string.Empty;
        public RecipeCategory Category { get; set; } = RecipeCategory.Other;
        public DateTime Date { get; init; } = DateTime.UtcNow;

        public string OwnerUsername { get; set; } = string.Empty;

    }

    public Recipe(string title, List<string> ingredients, string instructions, RecipeCategory category, string ownerUsername)
        {
            Title = title;
            Ingredients = ingredients;
            Instructions = instructions;
            Category = category;
            OwnerUsername = ownerUsername;
        }
    }
}
