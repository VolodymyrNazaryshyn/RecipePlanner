using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class DietTable
    {
        public DietTable()
        {
            DietMeals = new HashSet<DietMeal>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DietMeal> DietMeals { get; set; }
    }
}
