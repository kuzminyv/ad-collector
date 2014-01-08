CREATE TABLE [dbo].[AdsRealty]
(
	[AdId] INT NOT NULL, 
    [Address] NVARCHAR(500) NULL, 
    [RoomsCount] INT NOT NULL, 
    [Floor] INT NOT NULL, 
    [FloorsCount] INT NOT NULL, 
    [LivingSpace] FLOAT NOT NULL, 
    [IsNewBuilding] BIT NOT NULL DEFAULT 0, 
    [PricePerMeter] FLOAT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_AdsRealty_ToAds] FOREIGN KEY ([AdId]) REFERENCES dbo.[Ads]([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_AdsRealty] PRIMARY KEY ([AdId])
)

GO



CREATE INDEX [IX_AdsRealty_PricePerMeter] ON [dbo].[AdsRealty] ([PricePerMeter])
