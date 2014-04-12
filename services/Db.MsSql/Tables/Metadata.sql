CREATE TABLE [dbo].[Metadata]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UserId] INT NOT NULL, 
    [AdId] INT NOT NULL, 
    [IsFavorite] BIT NOT NULL DEFAULT 0, 
    [Note] NVARCHAR(2000) NULL, 
    CONSTRAINT [FK_Metadata_ToAds] FOREIGN KEY ([AdId]) REFERENCES [Ads]([Id]) ON DELETE CASCADE
)

GO

CREATE INDEX [IX_Metadata_AdId] ON [dbo].[Metadata] ([AdId])

GO

CREATE INDEX [IX_Metadata_IsFavorite] ON [dbo].[Metadata] ([IsFavorite])

GO

CREATE INDEX [IX_Metadata_UserId] ON [dbo].[Metadata] ([UserId])
