using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class MainTable
    {
        public MainTable()
        {
            AdditionalInfos = new HashSet<AdditionalInfo>();
            DietMeals = new HashSet<DietMeal>();
            MealIngredients = new HashSet<MealIngredient>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Calories { get; set; }

        public virtual ICollection<AdditionalInfo> AdditionalInfos { get; set; }
        public virtual ICollection<DietMeal> DietMeals { get; set; }
        public virtual ICollection<MealIngredient> MealIngredients { get; set; }
    }
}
