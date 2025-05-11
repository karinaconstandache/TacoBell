using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TacoBell.Models.Entities
{
    public class Dish
    {
        [Key]
        public int DishId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PortionSize { get; set; }
        public decimal TotalQuantity { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<DishAllergen> DishAllergens { get; set; }
        public ICollection<DishImage> Images { get; set; }
        public ICollection<MenuDish> MenuDishes { get; set; }
        public ICollection<OrderDish> OrderDishes { get; set; }
    }
}
