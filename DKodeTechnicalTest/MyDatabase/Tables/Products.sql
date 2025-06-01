CREATE TABLE [dbo].[Products]
(
	[ID] VARCHAR(50) UNIQUE PRIMARY KEY NOT NULL, /* Possible long strings of numbers for ID -> int */
	[SKU] VARCHAR(50) UNIQUE NOT NULL,
	[Name] NVARCHAR(255) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[EAN] VARCHAR(50) NULL,
	[ProducerName] NVARCHAR(255) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[MainCategory] NVARCHAR(100) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[SubCategory] NVARCHAR(100) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[ChildCategory] NVARCHAR(100) NULL, /* May contain lanugage specific special characters -> nvarchar */
	[IsWire] BIT NULL,
	[Shipping] VARCHAR(20) NULL,
/* May contain lanugage specific special characters -> nvarchar */
	[Available] BIT NULL,
/* Accuracy up to 3 digits (centimeters), save space -> smallint */
/* Accuracy up to 3 digits (centimeters), save space -> smallint */
/* Accuracy up to 3 digits (centimeters), save space -> smallint */
/* Relatively small numbers, precision: 1 digit, 5 decimals -> decimal(6,5) */
	[IsVendor] BIT NULL,
	[DefaultImage] NVARCHAR(2048) NULL /* Possible special characters in URL, URL maximum character lenght for browsers -> nvarchar(2048) */
);



