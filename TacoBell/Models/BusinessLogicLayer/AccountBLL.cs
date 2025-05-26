using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TacoBell.Models;
using TacoBell.Models.DTOs;
using TacoBell.Models.Entities;

namespace TacoBell.Models.BusinessLogicLayer
{
    public class AccountBLL
    {
        public async Task<List<OrderDisplayDTO>> GetUserActiveOrdersAsync(int userId)
        {
            using var db = new TacoBellDbContext();

            var orders = new List<OrderDisplayDTO>();

            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "sp_GetUserActiveOrders";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@UserId", userId));

            await db.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // Handle EstimatedDelivery which might be stored as TIME or as string
                TimeSpan estimatedDelivery;
                var deliveryValue = reader["EstimatedDelivery"];
                if (deliveryValue is TimeSpan timeSpan)
                {
                    estimatedDelivery = timeSpan;
                }
                else if (deliveryValue is string timeString)
                {
                    TimeSpan.TryParse(timeString, out estimatedDelivery);
                }
                else
                {
                    estimatedDelivery = TimeSpan.Zero;
                }

                var order = new OrderDisplayDTO
                {
                    OrderId = reader.GetInt32("OrderId"),
                    OrderCode = reader.GetGuid("OrderCode"),
                    OrderDate = reader.GetDateTime("OrderDate"),
                    EstimatedDelivery = estimatedDelivery,
                    Status = reader.GetString("Status"),
                    ShippingFee = reader.GetDecimal("ShippingFee"),
                    DiscountApplied = reader.GetDecimal("DiscountApplied"),
                    Subtotal = reader.GetDecimal("Subtotal"),
                    TotalCost = reader.GetDecimal("TotalCost")
                };

                orders.Add(order);
            }

            return orders;
        }

        public async Task<List<OrderDisplayDTO>> GetUserOrderHistoryAsync(int userId)
        {
            using var db = new TacoBellDbContext();

            var orders = new List<OrderDisplayDTO>();

            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "sp_GetUserOrderHistory";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@UserId", userId));

            await db.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                // Handle EstimatedDelivery which might be stored as TIME or as string
                TimeSpan estimatedDelivery;
                var deliveryValue = reader["EstimatedDelivery"];
                if (deliveryValue is TimeSpan timeSpan)
                {
                    estimatedDelivery = timeSpan;
                }
                else if (deliveryValue is string timeString)
                {
                    TimeSpan.TryParse(timeString, out estimatedDelivery);
                }
                else
                {
                    estimatedDelivery = TimeSpan.Zero;
                }

                var order = new OrderDisplayDTO
                {
                    OrderId = reader.GetInt32("OrderId"),
                    OrderCode = reader.GetGuid("OrderCode"),
                    OrderDate = reader.GetDateTime("OrderDate"),
                    EstimatedDelivery = estimatedDelivery,
                    Status = reader.GetString("Status"),
                    ShippingFee = reader.GetDecimal("ShippingFee"),
                    DiscountApplied = reader.GetDecimal("DiscountApplied"),
                    Subtotal = reader.GetDecimal("Subtotal"),
                    TotalCost = reader.GetDecimal("TotalCost")
                };

                orders.Add(order);
            }

            return orders;
        }

        public async Task<List<OrderItemDTO>> GetOrderDetailsAsync(int orderId)
        {
            using var db = new TacoBellDbContext();

            var orderItems = new List<OrderItemDTO>();

            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "sp_GetOrderDetails";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OrderId", orderId));

            await db.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var item = new OrderItemDTO
                {
                    ItemType = reader.GetString("ItemType"),
                    ItemName = reader.GetString("ItemName"),
                    Quantity = reader.GetInt32("Quantity"),
                    UnitPrice = reader.GetDecimal("UnitPrice"),
                    TotalPrice = reader.GetDecimal("TotalPrice")
                };

                orderItems.Add(item);
            }

            return orderItems;
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            using var db = new TacoBellDbContext();

            await using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = "sp_CancelOrder";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("@OrderId", orderId));
            command.Parameters.Add(new SqlParameter("@UserId", userId));

            await db.Database.OpenConnectionAsync();

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetInt32("RowsAffected") > 0;
            }

            return false;
        }

        public User GetUserDetails(int userId)
        {
            using var db = new TacoBellDbContext();
            return db.Users.Find(userId);
        }

        public void UpdateUserDetails(User user)
        {
            using var db = new TacoBellDbContext();
            var existing = db.Users.Find(user.UserId);
            if (existing != null)
            {
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                existing.PhoneNumber = user.PhoneNumber;
                existing.DeliveryAddress = user.DeliveryAddress;
                // Don't update email and password here for security
                db.SaveChanges();
            }
        }
    }
}