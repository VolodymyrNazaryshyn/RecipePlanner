using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class Alergen
    {
        public Alergen()
        {
            IngredientAlergens = new HashSet<IngredientAlergen>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<IngredientAlergen> IngredientAlergens { get; set; }
    }
}
