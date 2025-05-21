using System;
using System.Data;
using System.Data.SqlClient;
using TacoBell.Models;
using TacoBell.Helpers;

namespace TacoBell.BusinessLogicLayer
{
    public class OrderBLL
    {
        private readonly string _connectionString;

        public OrderBLL(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int CreateOrder(int userId, decimal shippingFee, bool discountApplied)
        {
            int newOrderId = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_CreateOrderWithItems", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@ShippingFee", shippingFee);
                cmd.Parameters.AddWithValue("@DiscountApplied", discountApplied);

                var outputId = new SqlParameter("@OrderId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputId);

                conn.Open();
                cmd.ExecuteNonQuery();

                newOrderId = (int)outputId.Value;
            }

            return newOrderId;
        }
    }
}
