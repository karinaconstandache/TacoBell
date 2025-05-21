CREATE PROCEDURE sp_CreateOrderWithItems
    @UserId INT,
    @ShippingFee DECIMAL(10,2),
    @DiscountApplied BIT,
    @OrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Orders (UserId, OrderDate, Status, ShippingFee, DiscountApplied)
    VALUES (@UserId, GETDATE(), 0, @ShippingFee, @DiscountApplied);

    SET @OrderId = SCOPE_IDENTITY();
END
