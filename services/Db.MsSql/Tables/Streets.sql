CREATE TABLE [dbo].[Streets]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [LocationId] INT NOT NULL, 
    [Name] NVARCHAR(100) NOT NULL
)
