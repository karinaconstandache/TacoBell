using System;
using System.Collections.Generic;
using System.Linq;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class CategoryBLL
    {
        private readonly TacoBellDbContext _db = new();
        private readonly DishBLL _dishBLL = new();
        private readonly MenuBLL _menuBLL = new();

        public List<Category> GetAllCategories()
        {
            return _db.Categories.ToList();
        }

        public void AddCategory(Category category)
        {
            _db.Categories.Add(category);
            _db.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            var existing = _db.Categories.Find(category.CategoryId);
            if (existing != null)
            {
                existing.Name = category.Name;
                _db.SaveChanges();
            }
        }

        public void DeleteCategory(int categoryId)
        {
            var category = _db.Categories.Find(categoryId);
            if (category != null)
            {
                bool hasDishes = _db.Dishes.Any(d => d.CategoryId == categoryId);
                bool hasMenus = _db.Menus.Any(m => m.CategoryId == categoryId);

                if (hasDishes || hasMenus)
                    throw new InvalidOperationException("Această categorie conține preparate sau meniuri și nu poate fi ștearsă.");

                _db.Categories.Remove(category);
                _db.SaveChanges();
            }
        }


    }
}
