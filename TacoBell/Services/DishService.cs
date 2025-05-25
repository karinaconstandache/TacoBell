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
                .Include(p => p.DishAllergens)
                    .ThenInclude(da => da.Allergen)
                .Include(p => p.Images)  // <-- corect, nu DishImages
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new DishDisplayDTO
                {
                    Name = p.Name,
                    PortionSize = p.PortionSize,
                    Price = p.Price,
                    TotalQuantity = p.TotalQuantity,
                    ImagePath = p.Images.Select(img => img.RelativePath).FirstOrDefault()
                                ?? "/Assets/Images/menuimage.jpg",
                    Allergens = p.DishAllergens.Select(da => da.Allergen.Name).ToList()
                })
                .ToListAsync();

            return dishes;
        }
    }
}
