﻿CREATE TABLE [dbo].[AdImages]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AdId] INT NOT NULL, 
    [Url] NVARCHAR(1000) NULL, 
    [PreviewUrl] NVARCHAR(1000) NULL, 
    CONSTRAINT [FK_AdImages_ToAds] FOREIGN KEY ([AdId]) REFERENCES [Ads]([Id]) 
)
