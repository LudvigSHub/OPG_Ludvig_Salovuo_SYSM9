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
        public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;



        public string? OwnerUsername { get; set; } = null;

        public bool IsGlobal => OwnerUsername is null;





    }

}



