using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TacoBell.Models.Entities
{
    public class Menu
    {
        [Key]
        public int MenuId { get; set; }
        public string Name { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }

        public ICollection<MenuDish> MenuDishes { get; set; }
        public ICollection<OrderMenu> OrderMenus { get; set; }
    }
}
