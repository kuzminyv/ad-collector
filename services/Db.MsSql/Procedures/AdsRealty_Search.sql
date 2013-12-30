CREATE PROCEDURE [dbo].[AdsRealty_Search] 
	@offset int,
	@limit int,
	@sortOrder int,-- 0 asc
	@sortBy nvarchar(20),
	@connectorId nvarchar(1000) = NULL,
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
	[_order] int identity(1,1),
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
	[ConnectorId] nvarchar(1000),
	[HistoryLength] int,
	[TotalCount] int
) 

SET @sql = '
WITH TempResult AS (
SELECT 
	rv.[Id], 
	rv.[Title], 
	rv.[Description], 
	rv.[PublishDate], 
	rv.[CollectDate], 
	rv.[Url], 
	rv.[Price], 
	rv.[CreationDate],
	rv.[Address], 
	rv.[RoomsCount], 
	rv.[Floor], 
	rv.[FloorsCount], 
	rv.[LivingSpace], 
	rv.[IsNewBuilding],
	rv.[ConnectorId],
	(SELECT COUNT(h.[AdId]) FROM dbo.AdHistoryItems h WHERE h.[AdId] = rv.[Id]) as [HistoryLength]
FROM dbo.AdsRealtyView rv ' +
'WHERE 0 = 0 ' +
IIF(@connectorId is null, '', ' and rv.[ConnectorId] = @connectorId') +
IIF(@priceMin is null, '',    ' and rv.[Price] >= @priceMin') +
IIF(@priceMax is null, '',    ' and rv.[Price] <= @priceMax') +
IIF(@searchCondition is null, '', ' and CONTAINS(([Title], [Description], [Url], [SystemTags], [Address]),  @searchCondition)') +
IIF(@detailsDownloadStatus is null, '', ' and rv.[DetailsDownloadStatus] = @detailsDownloadStatus') + 	
'), TempCount AS (SELECT COUNT(*) AS TotalCount FROM TempResult)' +
' SELECT * FROM TempResult, TempCount ' +
' ORDER BY ' + @sortBy + ' ' + @order +
' OFFSET @offset ROWS' +
' FETCH NEXT @limit ROWS ONLY'

INSERT INTO @ads 
EXECUTE sp_executesql @sql, N'@offset int, @limit int, @connectorId nvarchar(1000), @priceMin float, @priceMax float, @detailsDownloadStatus int, @searchCondition nvarchar(200)', 
						      @offset,     @limit,     @connectorId,                @priceMin,       @priceMax,       @detailsDownloadStatus,     @searchCondition

SELECT * FROM @ads ORDER BY [_order]
SELECT [Id], [AdId], [Url], [PreviewUrl] FROM dbo.AdImages i WHERE i.[AdId] in (SELECT [Id] FROM @ads)

END
GO