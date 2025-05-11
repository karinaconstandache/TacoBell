using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TacoBell.Models.Entities
{
    public class Allergen
    {
        [Key]
        public int AllergenId { get; set; }
        public string Name { get; set; }

        public ICollection<DishAllergen> DishAllergens { get; set; }
    }
}
