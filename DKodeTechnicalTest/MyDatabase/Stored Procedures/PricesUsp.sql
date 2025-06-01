CREATE PROCEDURE [dbo].[usp_UpsertPrices]
	@ProductID VARCHAR(50),
	@SKU VARCHAR(50),
	@DiscountNetPrice DECIMAL(10,2),
	@LogisticDiscountNetPrice DECIMAL(10,2)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM Prices WHERE ProductID = @ProductID)
		BEGIN
			UPDATE Prices
			SET
				[SKU] = @SKU,
				[DiscountNetPrice] = @DiscountNetPrice,
				[LogisticDiscountNetPrice] = @LogisticDiscountNetPrice
			WHERE ProductID = @ProductID;
		END
	ELSE
		BEGIN
			INSERT INTO Prices (
				[ProductID],
				[SKU],
				[DiscountNetPrice],
				[LogisticDiscountNetPrice]
			) VALUES (
				@ProductID,
				@SKU,
				@DiscountNetPrice,
				@LogisticDiscountNetPrice
			);
		END
END

			
