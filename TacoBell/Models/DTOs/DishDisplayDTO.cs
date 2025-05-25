using System.Collections.Generic;

namespace TacoBell.Models.DTOs
{
    public class DishDisplayDTO : IDisplayItem
    {
        public string Name { get; set; }
        public string PortionSize { get; set; }  // string, conform entității tale
        public decimal Price { get; set; }
        public decimal TotalQuantity { get; set; }
        public string ImagePath { get; set; }    // folosește RelativePath de fapt
        public List<string> Allergens { get; set; } = new();

        public string PortionInfo => $"Portie: {PortionSize}";
        public string PriceInfo => $"Pret: {Price} lei";
        public bool IsAvailable => TotalQuantity > 0;
        public string Availability => IsAvailable ? "" : "Indisponibil";
    }
}
