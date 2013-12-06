CREATE TABLE [dbo].[LogEntries]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Severity] INT NOT NULL, 
    [Time] DATETIME2 NOT NULL, 
    [Message] NVARCHAR(1000) NOT NULL, 
    [Details] NVARCHAR(MAX) NULL
)
