using System.Collections.Generic;

namespace TacoBell.Models.DTOs
{
    public class DishDisplayDTO : IDisplayItem
    {
        public int DishId { get; set; }
        public string Name { get; set; }
        public string PortionSize { get; set; }
        public decimal Price { get; set; }
        public decimal TotalQuantity { get; set; }
        public string ImagePath { get; set; }
        public List<string> Allergens { get; set; } = new();

        public string PortionInfo => $"Portie: {PortionSize} g";
        public string PriceInfo => $"Pret: {Price:0.00} lei";
        public bool IsAvailable => TotalQuantity > 0;
        public string Availability => IsAvailable ? "" : "Indisponibil";
    }
}