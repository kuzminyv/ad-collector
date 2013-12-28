CREATE TABLE [dbo].[Ads]
(
	[Id] INT NOT NULL IDENTITY, 
    [Title] NVARCHAR(250) NULL, 
    [Description] NVARCHAR(4000) NULL, 
    [PublishDate] DATETIME2 NOT NULL, 
    [CollectDate] DATETIME2 NOT NULL, 
    [Url] NVARCHAR(4000) NULL, 
    [Price] FLOAT NOT NULL, 
    [IdOnWebSite] NVARCHAR(250) NULL, 
    [IsSuspicious] BIT NOT NULL, 
    [ConnectorId] NVARCHAR(1000) NULL, 
    [CreationDate] DATETIME2 NULL, 
    [DetailsDownloadStatus] INT NOT NULL DEFAULT 0,
	[SystemTags] NVARCHAR(500) NULL, 
    CONSTRAINT [PK_Ads] PRIMARY KEY ([Id])
)

GO

CREATE INDEX [IX_Ads_PublishDate] ON [dbo].[Ads] ([PublishDate])

GO

CREATE INDEX [IX_Ads_CollectDate] ON [dbo].[Ads] ([CollectDate])

GO

CREATE INDEX [IX_Ads_Price] ON [dbo].[Ads] ([Price])

GO

CREATE FULLTEXT INDEX ON [dbo].[Ads] ([Title], [Description], [Url], [IdOnWebSite], [SystemTags]) KEY INDEX [PK_Ads] ON [AdsFulltextCatalog] WITH CHANGE_TRACKING AUTO
