using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class IngredientsTable
    {
        public IngredientsTable()
        {
            IngredientAlergens = new HashSet<IngredientAlergen>();
            MealIngredients = new HashSet<MealIngredient>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<IngredientAlergen> IngredientAlergens { get; set; }
        public virtual ICollection<MealIngredient> MealIngredients { get; set; }
    }
}
