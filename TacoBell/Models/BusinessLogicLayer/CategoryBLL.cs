using System.Collections.Generic;
using System.Linq;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class CategoryBLL
    {
        private readonly TacoBellDbContext _db = new();

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

        public void DeleteCategory(int id)
        {
            var category = _db.Categories.Find(id);
            if (category != null)
            {
                _db.Categories.Remove(category);
                _db.SaveChanges();
            }
        }
    }
}
