CREATE TABLE [dbo].[AdsRealty]
(
	[AdId] INT NOT NULL, 
    [Address] NVARCHAR(500) NULL, 
    [RoomsCount] INT NOT NULL, 
    [Floor] INT NOT NULL, 
    [FloorsCount] INT NOT NULL, 
    [LivingSpace] FLOAT NOT NULL, 
    [IsNewBuilding] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_AdsRealty_ToAds] FOREIGN KEY ([AdId]) REFERENCES dbo.[Ads]([Id]) ON DELETE CASCADE,
	CONSTRAINT [PK_AdsRealty] PRIMARY KEY ([AdId])
)

GO

CREATE FULLTEXT INDEX ON [dbo].[AdsRealty] ([Address]) KEY INDEX [PK_AdsRealty] ON [AdsFulltextCatalog] WITH CHANGE_TRACKING AUTO
