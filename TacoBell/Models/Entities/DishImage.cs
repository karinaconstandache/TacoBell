using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TacoBell.Models.Entities
{
    public class DishImage
    {
        [Key]
        public int ImageId { get; set; }

        [ForeignKey("Dish")]
        public int DishId { get; set; }
        public Dish Dish { get; set; }

        public string RelativePath { get; set; }
    }
}
