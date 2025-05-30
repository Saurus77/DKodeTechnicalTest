CREATE PROCEDURE [dbo].[usp_UpsertProducts]
	@ID INT,
	@SKU VARCHAR(50),
	@Name NVARCHAR(255),
	@ReferenceNumber VARCHAR(100),
	@EAN VARCHAR(50),
	@CanBeReturned BIT,
	@ProducerName NVARCHAR(255),
	@MainCategory NVARCHAR(100),
	@SubCategory NVARCHAR(100),
	@ChildCategory NVARCHAR(100),
	@IsWire BIT,
	@Shipping VARCHAR(20),
	@PackageSize NVARCHAR(50),
	@Available BIT,
	@LogisticHeight SMALLINT,
	@LogisticWidth SMALLINT,
	@LogisticLength SMALLINT,
	@LogisticWeight DECIMAL(6,5),
	@IsVendor BIT,
	@AvailableInParcelLocker BIT,
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
				[ReferenceNumber] = @ReferenceNumber,
				[EAN] = @EAN,
				[CanBeReturned] = @CanBeReturned,
				[ProducerName] = @ProducerName,
				[MainCategory] = @MainCategory,
				[SubCategory] = @SubCategory,
				[ChildCategory] = @ChildCategory,
				[IsWire] = @IsWire,
				[Shipping] = @Shipping,
				[PackageSize] = @PackageSize,
				[Available] = @Available,
				[LogisticHeight] = @LogisticHeight,
				[LogisticWidth] = @LogisticWidth,
				[LogisticLength] = @LogisticLength,
				[LogisticWeight] = @LogisticWeight,
				[IsVendor] = @IsVendor,
				[AvailableInParcelLocker] = @AvailableInParcelLocker,
				[DefaultImage] = @DefaultImage
			WHERE ID = @ID;
		END
	ELSE
		BEGIN
			INSERT INTO Products (
				[ID],
				[SKU],
				[Name],
				[ReferenceNumber],
				[EAN],
				[CanBeReturned],
				[ProducerName],
				[MainCategory],
				[SubCategory],
				[ChildCategory],
				[IsWire],
				[Shipping],
				[PackageSize],
				[Available],
				[LogisticHeight],
				[LogisticWidth],
				[LogisticLength],
				[LogisticWeight],
				[IsVendor],
				[AvailableInParcelLocker],
				[DefaultImage]
			) VALUES (
				@ID,
				@SKU,
				@Name,
				@ReferenceNumber,
				@EAN,
				@CanBeReturned,
				@ProducerName,
				@MainCategory,
				@SubCategory,
				@ChildCategory,
				@IsWire,
				@Shipping,
				@PackageSize,
				@Available,
				@LogisticHeight,
				@LogisticWidth,
				@LogisticLength,
				@LogisticWeight,
				@IsVendor,
				@AvailableInParcelLocker,
				@DefaultImage
			);
		END
END