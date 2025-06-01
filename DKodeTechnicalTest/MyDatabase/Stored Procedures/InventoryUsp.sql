CREATE PROCEDURE [dbo].[usp_UpsertInventory]
	@ProductID VARCHAR(50),
	@SKU VARCHAR(50),
	@StockQuantity DECIMAL(10,0),
	@Shipping NVARCHAR(255)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM Inventory WHERE ProductID = @ProductID)
		BEGIN
			UPDATE Inventory
			SET
				[SKU] = @SKU,
				[StockQuantity] = @StockQuantity,
				[Shipping] = @Shipping
			WHERE ProductID = @ProductID;
		END
	ELSE
		BEGIN
			INSERT INTO Inventory  (
				[ProductID],
				[SKU],
				[StockQuantity],
				[Shipping]
			) VALUES (
				@ProductID,
				@SKU,
				@StockQuantity,
				@Shipping
			);
		END
END
