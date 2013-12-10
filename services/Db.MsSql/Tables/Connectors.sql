CREATE TABLE [dbo].[Connectors]
(
	[Id] NVARCHAR(1000) NOT NULL PRIMARY KEY, 
    [Disabled] BIT NOT NULL DEFAULT 0, 
    [LastCheckDate] DATETIME2 NULL
)
