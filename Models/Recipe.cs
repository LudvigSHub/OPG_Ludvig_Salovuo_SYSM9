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
        public DateTime CreatedAt { get; private set; }

        public string OwnerUsername { get; set; } = string.Empty;


        
    }

}

    

