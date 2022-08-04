using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class DietMeal
    {
        public int Id { get; set; }
        public int IdMeal { get; set; }
        public int IdDiet { get; set; }

        public virtual DietTable IdDietNavigation { get; set; }
        public virtual MainTable IdMealNavigation { get; set; }
    }
}
