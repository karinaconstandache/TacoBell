using System.Collections.Generic;

namespace TacoBell.Models.DTOs
{
    public class MenuDisplayDTO : IDisplayItem
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public List<string> ItemPortions { get; set; } = new();
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string ImagePath { get; set; } = "/Assets/Images/menuimage.jpg";
        public List<string> Allergens { get; set; } = new();

        public string PortionInfo => string.Join(", ", ItemPortions);
        public string PriceInfo => $"Pret: {Price} lei";
        public string Availability => IsAvailable ? "" : "Indisponibil";
    }

}
