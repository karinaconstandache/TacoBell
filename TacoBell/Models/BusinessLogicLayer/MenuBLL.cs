using System.Collections.Generic;
using System.Linq;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class MenuBLL
    {
        private readonly TacoBellDbContext _db = new();

        public List<Menu> GetAllMenus()
        {
            return _db.Menus
                      .Select(m => new Menu
                      {
                          MenuId = m.MenuId,
                          Name = m.Name,
                          CategoryId = m.CategoryId,
                          CategoryName = m.Category.Name
                      }).ToList();
        }

        public void AddMenu(Menu menu)
        {
            _db.Menus.Add(new Menu
            {
                Name = menu.Name,
                CategoryId = menu.CategoryId
            });
            _db.SaveChanges();
        }

        public void UpdateMenu(Menu menu)
        {
            var existing = _db.Menus.Find(menu.MenuId);
            if (existing != null)
            {
                existing.Name = menu.Name;
                existing.CategoryId = menu.CategoryId;
                _db.SaveChanges();
            }
        }

        public void DeleteMenu(int id)
        {
            var menu = _db.Menus.Find(id);
            if (menu != null)
            {
                _db.Menus.Remove(menu);
                _db.SaveChanges();
            }
        }
    }
}
