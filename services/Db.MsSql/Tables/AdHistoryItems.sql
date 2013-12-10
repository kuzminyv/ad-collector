CREATE TABLE [dbo].[AdHistoryItems]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AdId] INT NOT NULL, 
    [AdPublishDate] DATETIME2 NOT NULL, 
    [Price] FLOAT NOT NULL, 
    [AdCollectDate] DATETIME2 NOT NULL, 
    CONSTRAINT [FK_AdHistoryItems_ToAds] FOREIGN KEY ([AdId]) REFERENCES [Ads]([Id])
)
