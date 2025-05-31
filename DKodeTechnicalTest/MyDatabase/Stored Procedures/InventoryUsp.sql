CREATE PROCEDURE [dbo].[usp_UpsertInventory]
	@ProductID INT,
	@SKU VARCHAR(50),
	@Unit NVARCHAR(10),
	@StockQuantity INT,
	@ManufacturerName NVARCHAR(255),
	@ManufacturerRefNum NVARCHAR(255),
	@Shipping NVARCHAR(255),
	@ShippingCost DECIMAL(10,2)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM Inventory WHERE ProductID = @ProductID)
		BEGIN
			UPDATE Inventory
			SET
				[SKU] = @SKU,
				[Unit] = @Unit,
				[StockQuantity] = @StockQuantity,
				[ManufacturerName] = @ManufacturerName,
				[ManufacturerRefNum] = @ManufacturerRefNum,
				[Shipping] = @Shipping,
				[ShippingCost] = @ShippingCost
			WHERE ProductID = @ProductID;
		END
	ELSE
		BEGIN
			INSERT INTO Inventory  (
				[ProductID],
				[SKU],
				[Unit],
				[StockQuantity],
				[ManufacturerName],
				[ManufacturerRefNum],
				[Shipping],
				[ShippingCost]
			) VALUES (
				@ProductID,
				@SKU,
				@Unit,
				@StockQuantity,
				@ManufacturerName,
				@ManufacturerRefNum,
				@Shipping,
				@ShippingCost
			);
		END
END
