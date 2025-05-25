CREATE PROCEDURE sp_CreateOrderWithItems
    @UserId INT,
    @ShippingFee DECIMAL(10,2),
    @DiscountApplied DECIMAL(10,2),
    @OrderId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Orders (UserId, OrderDate, Status, ShippingFee, DiscountApplied, OrderCode, EstimatedDelivery)
    VALUES (
        @UserId,
        GETDATE(),
        'inregistrata',
        @ShippingFee,
        @DiscountApplied,
        NEWID(),
        CAST(DATEADD(MINUTE, 45, GETDATE()) AS TIME)
    );

    SET @OrderId = SCOPE_IDENTITY();
END
