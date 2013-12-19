CREATE TABLE [dbo].[Ads]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
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
    [DetailsDownloadStatus] INT NOT NULL DEFAULT 0
)

GO

CREATE INDEX [IX_Ads_PublishDate] ON [dbo].[Ads] ([PublishDate])

GO

CREATE INDEX [IX_Ads_CollectDate] ON [dbo].[Ads] ([CollectDate])

GO

CREATE INDEX [IX_Ads_Price] ON [dbo].[Ads] ([Price])
