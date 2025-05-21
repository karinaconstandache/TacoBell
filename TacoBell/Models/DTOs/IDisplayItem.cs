using System.Collections.Generic;

namespace TacoBell.Models.DTOs
{
    public interface IDisplayItem
    {
        string Name { get; }
        string PortionInfo { get; }
        string PriceInfo { get; }
        string ImagePath { get; }
        bool IsAvailable { get; }
        string Availability { get; }
        List<string> Allergens { get; }
    }
}
