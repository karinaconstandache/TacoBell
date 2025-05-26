using System;
using System.Collections.Generic;

namespace TacoBell.Models.DTOs
{
    public class OrderDisplayDTO
    {
        public int OrderId { get; set; }
        public Guid OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan EstimatedDelivery { get; set; }
        public string Status { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal DiscountApplied { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalCost { get; set; }

        // Formatted properties for display
        public string FormattedOrderDate => OrderDate.ToString("dd/MM/yyyy HH:mm");
        public string FormattedEstimatedDelivery => EstimatedDelivery.ToString(@"hh\:mm");
        public string FormattedSubtotal => $"{Subtotal:0.00} lei";
        public string FormattedShippingFee => $"{ShippingFee:0.00} lei";
        public string FormattedDiscount => $"{DiscountApplied:0.00} lei";
        public string FormattedTotalCost => $"{TotalCost:0.00} lei";
        public string StatusDisplay => Status.ToUpper();

        // For expandable details
        public List<OrderItemDTO> OrderItems { get; set; } = new();

        public bool CanBeCancelled => Status != "livrata" && Status != "anulata";
    }

    public class OrderItemDTO
    {
        public string ItemType { get; set; } // "Dish" or "Menu"
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public string FormattedQuantity => $"{Quantity}x";
        public string FormattedUnitPrice => $"{UnitPrice:0.00} lei";
        public string FormattedTotalPrice => $"{TotalPrice:0.00} lei";
    }
}