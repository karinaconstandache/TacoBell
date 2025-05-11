using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TacoBell.Models.Entities
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public Guid OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan EstimatedDelivery { get; set; }
        public string Status { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountApplied { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<OrderDish> OrderDishes { get; set; }
        public ICollection<OrderMenu> OrderMenus { get; set; }
    }
}
