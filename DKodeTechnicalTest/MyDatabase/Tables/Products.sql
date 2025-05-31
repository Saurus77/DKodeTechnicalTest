CREATE TABLE [dbo].[Products]
(
	[ID] INT UNIQUE PRIMARY KEY NOT NULL, /* Possible long strings of numbers for ID -> int */
	[SKU] VARCHAR(50) UNIQUE NOT NULL,
	[Name] NVARCHAR(255) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[ReferenceNumber] VARCHAR(100) NULL,
	[EAN] VARCHAR(50) NULL,
	[CanBeReturned] BIT NULL,
	[ProducerName] NVARCHAR(255) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[MainCategory] NVARCHAR(100) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[SubCategory] NVARCHAR(100) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[ChildCategory] NVARCHAR(100) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[IsWire] BIT NULL,
	[Shipping] VARCHAR(20) NULL,
	[PackageSize] NVARCHAR(50) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[Available] BIT NULL,
	[LogisticHeight] SMALLINT NULL, /* Accuracy up to 3 digits (centimeters), save space -> smallint */
	[LogisticWidth] SMALLINT NULL, /* Accuracy up to 3 digits (centimeters), save space -> smallint */
	[LogisticLength] SMALLINT NULL, /* Accuracy up to 3 digits (centimeters), save space -> smallint */
	[LogisticWeight] DECIMAL(6,5) NULL, /* Relatively small numbers, precision: 1 digit, 5 decimals -> decimal(6,5) */
	[IsVendor] BIT NULL,
	[AvailableInParcelLocker] BIT NULL,
	[DefaultImage] NVARCHAR(2048) NULL /* Possible special characters in URL, URL maximum character lenght for browsers -> nvarchar(2048) */
);



