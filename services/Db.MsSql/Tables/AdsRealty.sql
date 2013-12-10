CREATE TABLE [dbo].[AdsRealty]
(
	[AdId] INT NOT NULL PRIMARY KEY, 
    [Address] NVARCHAR(500) NULL, 
    [RoomsCount] INT NOT NULL, 
    [Floor] INT NOT NULL, 
    [FloorsCount] INT NOT NULL, 
    [LivingSpace] FLOAT NOT NULL, 
    [IsNewBuilding] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_AdsRealty_ToAds] FOREIGN KEY ([AdId]) REFERENCES dbo.[Ads]([Id])
)
