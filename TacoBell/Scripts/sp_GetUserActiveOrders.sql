CREATE PROCEDURE sp_GetUserActiveOrders
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.OrderId,
        o.OrderCode,
        o.OrderDate,
        o.EstimatedDelivery,
        o.Status,
        o.ShippingFee,
        o.DiscountApplied,
        -- Calculate subtotal from dishes and menus
        ISNULL(DishTotal.Total, 0) + ISNULL(MenuTotal.Total, 0) AS Subtotal,
        (ISNULL(DishTotal.Total, 0) + ISNULL(MenuTotal.Total, 0) + o.ShippingFee - o.DiscountApplied) AS TotalCost
    FROM Orders o
    LEFT JOIN (
        SELECT 
            od.OrderId,
            SUM(od.Quantity * d.Price) AS Total
        FROM OrderDishes od
        INNER JOIN Dishes d ON od.DishId = d.DishId
        GROUP BY od.OrderId
    ) DishTotal ON o.OrderId = DishTotal.OrderId
    LEFT JOIN (
        SELECT 
            om.OrderId,
            SUM(om.Quantity * MenuPrices.MenuPrice) AS Total
        FROM OrderMenus om
        INNER JOIN (
            SELECT 
                m.MenuId,
                SUM(md.DishQuantityInMenu * d.Price) * 0.9 AS MenuPrice -- 10% discount for menus
            FROM Menus m
            INNER JOIN MenuDishes md ON m.MenuId = md.MenuId
            INNER JOIN Dishes d ON md.DishId = d.DishId
            GROUP BY m.MenuId
        ) MenuPrices ON om.MenuId = MenuPrices.MenuId
        GROUP BY om.OrderId
    ) MenuTotal ON o.OrderId = MenuTotal.OrderId
    WHERE o.UserId = @UserId 
        AND o.Status NOT IN ('livrata', 'anulata')
    ORDER BY o.OrderDate DESC;
END