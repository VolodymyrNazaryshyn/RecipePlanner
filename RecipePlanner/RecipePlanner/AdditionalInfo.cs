using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class AdditionalInfo
    {
        public int Id { get; set; }
        public int IdMeal { get; set; }
        public int IdKindOfMeal { get; set; }
        public string CookingTime { get; set; }
        public string Image { get; set; }
        public int? IdCuisine { get; set; }

        public virtual CuisineType IdCuisineNavigation { get; set; }
        public virtual KindOfMeal IdKindOfMealNavigation { get; set; }
        public virtual MainTable IdMealNavigation { get; set; }
    }
}
