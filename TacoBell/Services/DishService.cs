using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TacoBell.Models;
using TacoBell.Models.DTOs;

namespace TacoBell.Services
{
    public class DishService
    {
        public async Task<List<DishDisplayDTO>> GetByCategoryIdAsync(int categoryId)
        {
            using var db = new TacoBellDbContext();

            var dishes = await db.Dishes
                .Include(d => d.DishAllergens)
                    .ThenInclude(da => da.Allergen)
                .Include(d => d.Images)  // <-- corect, nu DishImages
                .Where(d => d.CategoryId == categoryId)
                .Select(d => new DishDisplayDTO
                {
                    DishId = d.DishId,
                    Name = d.Name,
                    PortionSize = d.PortionSize,
                    Price = d.Price,
                    TotalQuantity = d.TotalQuantity,
                    ImagePath = d.Images.Select(img => img.RelativePath).FirstOrDefault()
                                ?? "/Assets/Images/menuimage.jpg",
                    Allergens = d.DishAllergens.Select(da => da.Allergen.Name).ToList()
                })
                .ToListAsync();

            return dishes;
        }
    }
}
