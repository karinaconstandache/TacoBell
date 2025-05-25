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

            foreach (var item in items)
            {
                if (item.DishId.HasValue)
                {
                    db.OrderDishes.Add(new OrderDish
                    {
                        OrderId = orderId,
                        DishId = item.DishId.Value,
                        Quantity = item.Quantity
                    });
                }
                else if (item.MenuId.HasValue)
                {
                    db.OrderMenus.Add(new OrderMenu
                    {
                        OrderId = orderId,
                        MenuId = item.MenuId.Value,
                        Quantity = item.Quantity
                    });
                }
            }

            await db.SaveChangesAsync();

            // Start background timer to auto-update status
            _ = StartStatusTimerAsync(orderId);
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
