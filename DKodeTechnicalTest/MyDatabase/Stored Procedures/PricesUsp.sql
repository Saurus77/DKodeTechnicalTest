CREATE PROCEDURE [dbo].[usp_UpsertPrices]
	@ProductID INT,
	@SKU VARCHAR(50),
	@NetPrice DECIMAL(10,2),
	@DiscountNetPrice DECIMAL(10,2),
	@VAT TINYINT,
	@LogisticDiscountNetPrice DECIMAL(10,2)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM Prices WHERE ProductID = @ProductID)
		BEGIN
			UPDATE Prices
			SET
				[SKU] = @SKU,
				[NetPrice] = @NetPrice,
				[DiscountNetPrice] = @DiscountNetPrice,
				[VAT] = @VAT,
				[LogisticDiscountNetPrice] = @LogisticDiscountNetPrice
			WHERE ProductID = @ProductID;
		END
	ELSE
		BEGIN
			INSERT INTO Prices (
				[ProductID],
				[SKU],
				[NetPrice],
				[DiscountNetPrice],
				[VAT],
				[LogisticDiscountNetPrice]
			) VALUES (
				@ProductID,
				@SKU,
				@NetPrice,
				@DiscountNetPrice,
				@VAT,
				@LogisticDiscountNetPrice
			);
		END
END

			
