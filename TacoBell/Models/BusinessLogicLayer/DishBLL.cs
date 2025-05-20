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

        public void DeleteDish(int id)
        {
            var dish = _db.Dishes.Find(id);
            if (dish != null)
            {
                _db.Dishes.Remove(dish);
                _db.SaveChanges();
            }
        }
    }
}