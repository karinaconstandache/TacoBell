using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TacoBell.Models.DTOs;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class OrderBLL
    {
        public async Task PlaceOrderAsync(int userId, decimal shippingFee, decimal discount, List<CartItem> items)
        {
            using var db = new TacoBellDbContext();
            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                // First, check if all items have sufficient quantity
                await ValidateQuantitiesAsync(db, items);

                var orderIdParam = new SqlParameter("@OrderId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };

                await db.Database.ExecuteSqlRawAsync(
                    "EXEC sp_CreateOrderWithItems @UserId, @ShippingFee, @DiscountApplied, @OrderId OUTPUT",
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ShippingFee", shippingFee),
                    new SqlParameter("@DiscountApplied", discount),
                    orderIdParam
                );

                int orderId = (int)orderIdParam.Value;

                // Add order items and update quantities
                foreach (var item in items)
                {
                    if (item.DishId.HasValue)
                    {
                        // Add to order
                        db.OrderDishes.Add(new OrderDish
                        {
                            OrderId = orderId,
                            DishId = item.DishId.Value,
                            Quantity = item.Quantity
                        });

                        // Update dish quantity
                        await UpdateDishQuantityAsync(db, item.DishId.Value, item.Quantity);
                    }
                    else if (item.MenuId.HasValue)
                    {
                        // Add to order
                        db.OrderMenus.Add(new OrderMenu
                        {
                            OrderId = orderId,
                            MenuId = item.MenuId.Value,
                            Quantity = item.Quantity
                        });

                        // Update quantities for all dishes in the menu
                        await UpdateMenuDishQuantitiesAsync(db, item.MenuId.Value, item.Quantity);
                    }
                }

                await db.SaveChangesAsync();
                await transaction.CommitAsync();

                // Start background timer to auto-update status
                _ = StartStatusTimerAsync(orderId);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ValidateQuantitiesAsync(TacoBellDbContext db, List<CartItem> items)
        {
            foreach (var item in items)
            {
                if (item.DishId.HasValue)
                {
                    var dish = await db.Dishes.FindAsync(item.DishId.Value);
                    if (dish == null)
                        throw new InvalidOperationException($"Preparat cu ID {item.DishId.Value} nu a fost găsit.");

                    if (dish.TotalQuantity < item.Quantity)
                        throw new InvalidOperationException($"Cantitate insuficientă pentru {dish.Name}. Disponibil: {dish.TotalQuantity}, Solicitat: {item.Quantity}");
                }
                else if (item.MenuId.HasValue)
                {
                    var menuDishes = await db.MenuDishes
                        .Include(md => md.Dish)
                        .Where(md => md.MenuId == item.MenuId.Value)
                        .ToListAsync();

                    foreach (var menuDish in menuDishes)
                    {
                        var requiredQuantity = menuDish.DishQuantityInMenu * item.Quantity;
                        if (menuDish.Dish.TotalQuantity < requiredQuantity)
                            throw new InvalidOperationException($"Cantitate insuficientă pentru {menuDish.Dish.Name} din meniu. Disponibil: {menuDish.Dish.TotalQuantity}, Necesar: {requiredQuantity}");
                    }
                }
            }
        }

        private async Task UpdateDishQuantityAsync(TacoBellDbContext db, int dishId, int orderedQuantity)
        {
            var dish = await db.Dishes.FindAsync(dishId);
            if (dish != null)
            {
                dish.TotalQuantity -= orderedQuantity;
                if (dish.TotalQuantity < 0)
                    dish.TotalQuantity = 0;
            }
        }

        private async Task UpdateMenuDishQuantitiesAsync(TacoBellDbContext db, int menuId, int menuQuantity)
        {
            var menuDishes = await db.MenuDishes
                .Include(md => md.Dish)
                .Where(md => md.MenuId == menuId)
                .ToListAsync();

            foreach (var menuDish in menuDishes)
            {
                var requiredQuantity = menuDish.DishQuantityInMenu * menuQuantity;
                menuDish.Dish.TotalQuantity -= requiredQuantity;
                if (menuDish.Dish.TotalQuantity < 0)
                    menuDish.Dish.TotalQuantity = 0;
            }
        }

        private async Task StartStatusTimerAsync(int orderId)
        {
            string[] statuses = { "se pregateste", "a plecat la client", "livrata" };
            int delaySeconds = 90;

            foreach (var status in statuses)
            {
                await Task.Delay(delaySeconds * 1000);

                using var db = new TacoBellDbContext();
                var order = await db.Orders.FindAsync(orderId);
                if (order != null && order.Status != "anulata" && order.Status != "livrata")
                {
                    order.Status = status;
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}