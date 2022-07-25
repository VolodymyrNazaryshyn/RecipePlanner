using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class MealIngredient
    {
        public int Id { get; set; }
        public int IdMeal { get; set; }
        public int IdIngredient { get; set; }
        public string Quantity { get; set; }

        public virtual IngredientsTable IdIngredientNavigation { get; set; }
        public virtual MainTable IdMealNavigation { get; set; }
    }
}
