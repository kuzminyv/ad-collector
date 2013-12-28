CREATE PROCEDURE [dbo].[AdsRealty_Search] 
	@offset int,
	@limit int,
	@sortOrder int,-- 0 asc
	@sortBy nvarchar(20),
	@connectorId int = NULL,
	@priceMin float = NULL,
	@priceMax float = NULL,
	@detailsDownloadStatus int = NULL,
	@searchCondition nvarchar(200) = NULL
AS
BEGIN

DECLARE @sql nvarchar(2000)
DECLARE @order nvarchar(4)
SELECT @order = IIF(@sortOrder = 0, 'ASC', 'DESC')

DECLARE @ads TABLE (
	[Id] int NOT NULL PRIMARY KEY, 
	[Title] nvarchar(250) NULL, 
	[Description] nvarchar(4000) NULL, 
	[PublishDate] datetime2(7), 
	[CollectDate] datetime2(7), 
	[Url] nvarchar(4000) NULL, 
	[Price] float, 
	[CreationDate] datetime2(7) NULL,
	[Address] nvarchar(500) NULL, 
	[RoomsCount] int, 
	[Floor] int, 
	[FloorsCount] int, 
	[LivingSpace] float, 
	[IsNewBuilding] bit,
	[HistoryLength] int,
	[TotalCount] int
) 

SET @sql = '
WITH TempResult AS (
SELECT 
	a.[Id], 
	a.[Title], 
	a.[Description], 
	a.[PublishDate], 
	a.[CollectDate], 
	a.[Url], 
	a.[Price], 
	a.[CreationDate],
	r.[Address], 
	r.[RoomsCount], 
	r.[Floor], 
	r.[FloorsCount], 
	r.[LivingSpace], 
	r.[IsNewBuilding],
	(SELECT COUNT(h.[AdId]) FROM dbo.AdHistoryItems h WHERE h.[AdId] = a.[Id]) as [HistoryLength]
FROM dbo.Ads a left join 
	 dbo.AdsRealty r on a.Id = r.AdId	 
WHERE 
	(@connectorId is null or a.[ConnectorId] = @connectorId) and
	(@priceMin is null or a.[Price] >= @priceMin) and
	(@priceMax is null or a.[Price] <= @priceMax) and
	(@detailsDownloadStatus is null or a.[DetailsDownloadStatus] = @detailsDownloadStatus)' + 
	IIF(@searchCondition is NULL, '',  
		' and (CONTAINS((a.[Title], a.[Description], a.[Url], a.[SystemTags]),  @searchCondition) or CONTAINS(r.[Address], @searchCondition))') +
'), TempCount AS (SELECT COUNT(*) AS TotalCount FROM TempResult)' +
' SELECT * FROM TempResult, TempCount ' +
' ORDER BY ' + @sortBy + ' ' + @order +
' OFFSET @offset ROWS' +
' FETCH NEXT @limit ROWS ONLY'

INSERT INTO @ads 
EXECUTE sp_executesql @sql, N'@offset int, @limit int, @connectorId int, @priceMin float, @priceMax float, @detailsDownloadStatus int, @searchCondition nvarchar(200)', 
						      @offset,     @limit,     @connectorId,     @priceMin,       @priceMax,       @detailsDownloadStatus,     @searchCondition

SELECT * FROM @ads
SELECT [Id], [AdId], [Url], [PreviewUrl] FROM dbo.AdImages i WHERE i.[AdId] in (SELECT [Id] FROM @ads)

END
GO