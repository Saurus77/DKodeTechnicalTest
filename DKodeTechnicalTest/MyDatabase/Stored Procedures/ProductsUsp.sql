CREATE PROCEDURE [dbo].[usp_UpsertProducts]
	@ID INT,
	@SKU VARCHAR(50),
	@Name NVARCHAR(255),
	@EAN VARCHAR(50),
	@ProducerName NVARCHAR(255),
	@MainCategory NVARCHAR(100),
	@SubCategory NVARCHAR(100),
	@ChildCategory NVARCHAR(100),
	@IsWire BIT,
	@Shipping VARCHAR(20),
	@Available BIT,
	@IsVendor BIT,
	@DefaultImage NVARCHAR(2048)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM Products WHERE ID = @ID)
		BEGIN
			UPDATE Products
			SET
				[SKU] = @SKU,
				[Name] = @Name,
				[EAN] = @EAN,
				[ProducerName] = @ProducerName,
				[MainCategory] = @MainCategory,
				[SubCategory] = @SubCategory,
				[ChildCategory] = @ChildCategory,
				[IsWire] = @IsWire,
				[Shipping] = @Shipping,
				[Available] = @Available,
				[IsVendor] = @IsVendor,
				[DefaultImage] = @DefaultImage
			WHERE ID = @ID;
		END
	ELSE
		BEGIN
			INSERT INTO Products (
				[ID],
				[SKU],
				[Name],
				[EAN],
				[ProducerName],
				[MainCategory],
				[SubCategory],
				[ChildCategory],
				[IsWire],
				[Shipping],
				[Available],
				[IsVendor],
				[DefaultImage]
			) VALUES (
				@ID,
				@SKU,
				@Name,
				@EAN,
				@ProducerName,
				@MainCategory,
				@SubCategory,
				@ChildCategory,
				@IsWire,
				@Shipping,
				@Available,
				@IsVendor,
				@DefaultImage
			);
		END
END