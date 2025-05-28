CREATE TABLE [dbo].[Inventory]
(
	[ProductID] INT UNIQUE PRIMARY KEY NOT NULL,
	[SKU] VARCHAR(50) UNIQUE NOT NULL,
	[Unit] NVARCHAR(10) NOT NULL,
	[StockQuantity] INT NOT NULL,
	[ManufacturerName] NVARCHAR(255) NULL,
	[ManufacturerRefNum] NVARCHAR(255) NULL,
	[Shipping] NVARCHAR(255) NOT NULL,
	[ShippingCost] DECIMAL(10,2) NULL
)
