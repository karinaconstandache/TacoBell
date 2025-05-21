using System;
using System.Collections.Generic;
using System.Linq;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class DishBLL
    {
        private readonly TacoBellDbContext _db = new();

        public List<Dish> GetAllDishes()
        {
            return _db.Dishes.ToList();
        }

        public void AddDish(Dish dish)
        {
            _db.Dishes.Add(dish);
            _db.SaveChanges();
        }

        public void AddDishImage(int dishId, string relativeImagePath)
        {
            if (!string.IsNullOrWhiteSpace(relativeImagePath))
            {
                _db.DishImages.Add(new DishImage
                {
                    DishId = dishId,
                    RelativePath = relativeImagePath
                });
                _db.SaveChanges();
            }
        }

        public void AddDishAllergens(int dishId, List<int> allergenIds)
        {
            if (allergenIds != null && allergenIds.Count > 0)
            {
                foreach (int allergenId in allergenIds)
                {
                    _db.DishAllergens.Add(new DishAllergen
                    {
                        DishId = dishId,
                        AllergenId = allergenId
                    });
                }
                _db.SaveChanges();
            }
        }

        public void UpdateDish(Dish dish)
        {
            var existing = _db.Dishes.Find(dish.DishId);
            if (existing != null)
            {
                existing.Name = dish.Name;
                existing.Price = dish.Price;
                existing.PortionSize = dish.PortionSize;
                existing.TotalQuantity = dish.TotalQuantity;
                _db.SaveChanges();
            }
        }

        public void DeleteDish(int dishId)
        {
            var dish = _db.Dishes.Find(dishId);
            if (dish != null)
            {
                bool inMenus = _db.MenuDishes.Any(md => md.DishId == dishId);
                bool inOrders = _db.OrderDishes.Any(od => od.DishId == dishId);

                if (inMenus)
                    throw new InvalidOperationException("Acest preparat este inclus într-un meniu și nu poate fi șters.");
                if (inOrders)
                    throw new InvalidOperationException("Acest preparat este inclus într-o comandă și nu poate fi șters.");

                // Legături care nu blochează integritatea
                var allergens = _db.DishAllergens.Where(da => da.DishId == dishId).ToList();
                if (allergens.Any()) _db.DishAllergens.RemoveRange(allergens);

                var images = _db.DishImages.Where(img => img.DishId == dishId).ToList();
                if (images.Any()) _db.DishImages.RemoveRange(images);

                _db.Dishes.Remove(dish);
                _db.SaveChanges();
            }
        }




    }
}