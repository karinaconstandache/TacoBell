using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TacoBell.Models.Entities
{
    public class Allergen
    {
        [Key]
        public int AllergenId { get; set; }
        public string Name { get; set; }

        public ICollection<DishAllergen> DishAllergens { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }  // <- adăugat pentru UI
    }
}
