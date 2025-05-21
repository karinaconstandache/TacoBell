using System;
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

        public Menu AddMenu(Menu menu)
        {
            var created = _db.Menus.Add(new Menu
            {
                Name = menu.Name,
                CategoryId = menu.CategoryId
            }).Entity;

            _db.SaveChanges();
            return created;
        }


        public void AddDishToMenu(int menuId, int dishId, decimal quantity)
        {
            _db.MenuDishes.Add(new MenuDish
            {
                MenuId = menuId,
                DishId = dishId,
                DishQuantityInMenu = quantity
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
                bool inOrders = _db.OrderMenus.Any(om => om.MenuId == id);
                if (inOrders)
                    throw new InvalidOperationException("Acest meniu este inclus într-o comandă și nu poate fi șters.");

                var relatedDishes = _db.MenuDishes.Where(md => md.MenuId == id).ToList();
                _db.MenuDishes.RemoveRange(relatedDishes);

                _db.Menus.Remove(menu);
                _db.SaveChanges();
            }
        }


    }
}
