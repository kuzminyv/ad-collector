CREATE VIEW [dbo].[AdsRealtyView]
	WITH SCHEMABINDING
	AS 
	SELECT
		a.[Id], 
		a.[Title], 
		a.[Description], 
		a.[PublishDate], 
		a.[CollectDate], 
		a.[Url], 
		a.[Price], 
		a.[IdOnWebSite], 
		a.[IsSuspicious], 
		a.[ConnectorId], 
		a.[CreationDate], 
		a.[DetailsDownloadStatus], 
		a.[SystemTags],
		r.[Address], 
		r.[RoomsCount], 
		r.[Floor], 
		r.[FloorsCount], 
		r.[LivingSpace], 
		r.[IsNewBuilding] 
	FROM dbo.Ads a INNER JOIN dbo.AdsRealty r ON a.Id = r.AdId
GO

CREATE UNIQUE CLUSTERED INDEX IDX_AdsRealtyView_Id 
    ON [dbo].[AdsRealtyView] ([Id]);
GO

CREATE FULLTEXT INDEX ON [dbo].[AdsRealtyView] ([Title], [Description], [Url], [IdOnWebSite], [Address], [SystemTags]) KEY INDEX [IDX_AdsRealtyView_Id] ON [AdsFulltextCatalog] WITH CHANGE_TRACKING AUTO
GO