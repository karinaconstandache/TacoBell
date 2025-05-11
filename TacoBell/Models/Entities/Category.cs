using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TacoBell.Models.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public ICollection<Dish> Dishes { get; set; }
        public ICollection<Menu> Menus { get; set; }
    }
}
