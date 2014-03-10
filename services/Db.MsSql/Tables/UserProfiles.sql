CREATE TABLE [dbo].[UserProfiles]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [AdsQuery] XML NULL
)
