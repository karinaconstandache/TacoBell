using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TacoBell.Models;
using TacoBell.Models.DTOs;

namespace TacoBell.Services
{
    public class MenuService
    {
        public async Task<List<MenuDisplayDTO>> GetByCategoryIdAsync(int categoryId)
        {
            using var db = new TacoBellDbContext();

            var menus = await db.Menus
                .Include(m => m.MenuDishes)
                    .ThenInclude(md => md.Dish)
                        .ThenInclude(d => d.DishAllergens)
                            .ThenInclude(da => da.Allergen)
                .Where(m => m.CategoryId == categoryId)
                .ToListAsync();

            var dtoList = menus.Select(menu =>
            {
                var allDishes = menu.MenuDishes.Select(md => md.Dish).ToList();

                var portions = menu.MenuDishes
                    .Select(md => $"{md.Dish.Name}: {md.DishQuantityInMenu:0.##} buc").ToList();

                var allergens = allDishes
                    .SelectMany(d => d.DishAllergens)
                    .Select(da => da.Allergen.Name)
                    .Distinct()
                    .ToList();

                // Check if menu is available based on dish quantities and required amounts
                var allAvailable = menu.MenuDishes.All(md =>
                    md.Dish.TotalQuantity >= md.DishQuantityInMenu);

                return new MenuDisplayDTO
                {
                    MenuId = menu.MenuId,
                    Name = menu.Name,
                    ItemPortions = portions,
                    Price = allDishes.Sum(d => d.Price) * 0.9m, // reducere 10%
                    IsAvailable = allAvailable,
                    ImagePath = "/Assets/Images/menuimages.jpg",
                    Allergens = allergens
                };
            }).ToList();

            return dtoList;
        }
    }
}