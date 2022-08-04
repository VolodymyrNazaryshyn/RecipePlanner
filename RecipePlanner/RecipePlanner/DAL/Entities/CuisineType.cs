using System;
using System.Collections.Generic;

#nullable disable

namespace RecipePlanner
{
    public partial class CuisineType
    {
        public CuisineType()
        {
            AdditionalInfos = new HashSet<AdditionalInfo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<AdditionalInfo> AdditionalInfos { get; set; }
    }
}
