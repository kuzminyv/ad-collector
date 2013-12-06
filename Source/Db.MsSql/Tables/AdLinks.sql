CREATE TABLE [dbo].[AdLinks]
(
	[Id] INT NOT NULL IDENTITY , 
    [AdId] INT NOT NULL, 
    [LinkedAdId] INT NOT NULL, 
    [LinkType] INT NOT NULL, 
    CONSTRAINT [PK_AdLinks] PRIMARY KEY ([AdId], [LinkedAdId]), 
    CONSTRAINT [FK_AdLinks_ToAds] FOREIGN KEY ([AdId]) REFERENCES dbo.[Ads]([Id]), 
    CONSTRAINT [FK_AdLinks_ToTable] FOREIGN KEY ([LinkedAdId]) REFERENCES [Ads]([Id])
)
