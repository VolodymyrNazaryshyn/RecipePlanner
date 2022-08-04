using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class IngredientAlergen
    {
        public int Id { get; set; }
        public int IdIngredient { get; set; }
        public int IdAlergens { get; set; }

        public virtual Alergen IdAlergensNavigation { get; set; }
        public virtual IngredientsTable IdIngredientNavigation { get; set; }
    }
}
