﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Entities;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using Core.DAL.Common;
using System.Globalization;
using Core.DAL.MsSql.Common;
using AutoMapper;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Core.Entities.Enums;
using SQL = System.Data.SqlClient;

namespace Core.DAL.MsSql
{
    public class AdsRealtyRepository : MsSqlRepository<DbAd, AdRealty, int>
    {
        private class DbAdsRealtyContainer
        {
            public DbAdsRealty Ad { get; set; }
            public int? HistoryLength { get; set; }
            public DbMetadata Metadata { get; set; }
            public ICollection<DbAdImage> Images { get; set; }
        }

        private class Sorts
        {
            public static string GetSortNameOrDefault(Sort sort)
            {
                if (sort != null)
                {
                    switch (sort.Name)
                    {
                        case "CollectDate": return "CollectDate";
                        case "PublishDate": return "PublishDate";
                        case "Price": return "Price";
                        case "PricePerMeter": return "PricePerMeter";
                    }
                }
                return "PublishDate";
            }
        }

        #region Abstract methods implementation
        protected override System.Data.Entity.DbSet<DbAd> GetDbEntities(AdCollectorDBEntities context)
        {
            return context.Ads;
        }

        protected override IQueryable<DbAd> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbAd> entities, List<Filter> filters)
        {
            return entities;
        }

        protected override IQueryable<DbAd> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbAd> entities, List<Sort> sorts)
        {
            return entities;
        }

        protected override DbAd CreateDbEntity(int id)
        {
            return new DbAdsRealty() {Id = id };
        }

        protected override void UpdateEntityId(AdRealty entity, DbAd dbEntity)
        {
            entity.Id = dbEntity.Id;
        }
        #endregion 

        protected override void ConfigureEntityMapping()
        {
            //custom mapping
        }

        #region Entity Conversion

        public override DbAd ConvertToDbEntity(AdRealty entity)
        {
            return new DbAdsRealty()
            {
                Address = entity.Address,
                CollectDate = entity.CollectDate,
                PublishDate = entity.PublishDate,
                Description = entity.Description,
                Floor = entity.Floor,
                FloorsCount = entity.FloorsCount,
                Id = entity.Id,
                IdOnWebSite = entity.IdOnWebSite,
                IsSuspicious = entity.IsSuspicious,
                LivingSpace = entity.LivingSpace,
                Price = entity.Price,
                RoomsCount = entity.RoomsCount,
                ConnectorId = entity.ConnectorId,
                Title = entity.Title,
                Url = entity.Url,
                IsNewBuilding = entity.IsNewBuilding,
                DetailsDownloadStatus = (int)entity.DetailsDownloadStatus,
                PricePerMeter = entity.PricePerMeter
            };
        }

        private AdRealty ConvertToEntity(DbAdsRealtyContainer container)
        {
            AdRealty result = ConvertToEntity(container.Ad);
            result.HistoryLength = container.HistoryLength;
            return result;
        }

        public override AdRealty ConvertToEntity(DbAd dbEntity)
        {
            DbAdsRealty source = (DbAdsRealty)dbEntity;
            return new AdRealty()
            {
                Address = source.Address,
                CollectDate = source.CollectDate,
                PublishDate = source.PublishDate,
                Url = source.Url,
                Floor = source.Floor,
                FloorsCount = source.FloorsCount,
                Title = source.Title,
                Id = source.Id,
                IdOnWebSite = source.IdOnWebSite,
                LivingSpace = (float)source.LivingSpace,
                Price = source.Price,
                RoomsCount = source.RoomsCount,
                ConnectorId = source.ConnectorId,
                IsSuspicious = source.IsSuspicious,
                Description = source.Description,
                IsNewBuilding = source.IsNewBuilding,
                DetailsDownloadStatus = (DetailsDownloadStatus)source.DetailsDownloadStatus,
                Metadata = ((MetadataRepository)Repositories.MetadataRepository).ConvertToEntity(dbEntity.Metadatas.FirstOrDefault()),
                Images = ((AdImagesRepository)Repositories.AdImagesRepository).ConvertAllToEntity(dbEntity.AdImages).ToList()
            };
        }

        private IEnumerable<AdRealty> ConvertAllToEntity(IEnumerable<DbAdsRealtyContainer> containers)
        {
            foreach (var container in containers)
            {
                yield return ConvertToEntity(container);
            }
        }
        #endregion

        protected AdRealty ReadAdRealty(SQL.SqlDataReader reader)
        {
            AdRealty ad = new AdRealty()
            {
	            Id = reader.GetInt32(1),
                Title = reader.GetNullableString(2),
                Description = reader.GetNullableString(3),
                PublishDate = reader.GetDateTime(4),
	            CollectDate = reader.GetDateTime(5), 
	            Url = reader.GetString(6), 
	            Price = reader.GetDouble(7), 
	            CreationDate = reader.GetNullableDateTime(8),
	            Address = reader.GetNullableString(9), 
	            RoomsCount = reader.GetInt32(10), 
	            Floor = reader.GetInt32(11), 
	            FloorsCount = reader.GetInt32(12), 
	            LivingSpace = (float)reader.GetDouble(13), 
	            IsNewBuilding = reader.GetBoolean(14),
                ConnectorId = reader.GetString(15),
	            HistoryLength = reader.GetInt32(16)
            };
            return ad;
        }

        protected AdImage ReadAdImage(SQL.SqlDataReader reader)
        {
            AdImage image = new AdImage()
            {
                Id = reader.GetInt32(0),
                AdId = reader.GetInt32(1),
                Url = reader.GetNullableString(2),
                PreviewUrl = reader.GetNullableString(3)
            };
            return image;
        }

        protected Metadata ReadMetadata(SQL.SqlDataReader reader)
        {
            Metadata metadata = new Metadata()
            {
                Id = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                AdId = reader.GetInt32(2),
                IsFavorite = reader.GetBoolean(3),
                Note = reader.GetNullableString(4)                
            };
            return metadata;
        }

        public override QueryResult<AdRealty> GetList(Query query)
        {
            query = query == null ? new Query() : query;
            Dictionary<int, AdRealty> ads = new Dictionary<int, AdRealty>();
            List<AdImage> images = new List<AdImage>();
            int? totalCount = 0;
            ExecuteSqlOperation(context =>
            {
                SQL.SqlCommand cmd = new SQL.SqlCommand("dbo.AdsRealty_Search", context);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.Add("@offset", System.Data.SqlDbType.Int).Value = query.Start.HasValue ? query.Start.Value : 0;
                cmd.Parameters.Add("@limit", System.Data.SqlDbType.Int).Value = query.Limit.HasValue ? query.Limit.Value : int.MaxValue;

                Sort sort = query.Sorts.FirstOrDefault();
                cmd.Parameters.Add("@sortOrder", System.Data.SqlDbType.Int).Value = sort == null ? (int)SortOrder.Ascending : (int)sort.SortOrder;
                cmd.Parameters.Add("@sortBy", System.Data.SqlDbType.NVarChar).Value = Sorts.GetSortNameOrDefault(sort);

                var connectorFilter = query.Filters.FirstOrDefault(f => f.Name == "ConnectorId");
                cmd.Parameters.Add("@connectorId", System.Data.SqlDbType.NVarChar).Value = connectorFilter == null ? null : connectorFilter.Value;

                var priceMinFilter = query.Filters.FirstOrDefault(f => f.Name == "PriceMin");
                cmd.Parameters.Add("@priceMin", System.Data.SqlDbType.Float).Value = priceMinFilter == null ? null : priceMinFilter.Value;

                var priceMaxFilter = query.Filters.FirstOrDefault(f => f.Name == "PriceMax");
                cmd.Parameters.Add("@priceMax", System.Data.SqlDbType.Float).Value = priceMaxFilter == null ? null : priceMaxFilter.Value;

                var detailsDownloadStatusFilter = query.Filters.FirstOrDefault(f => f.Name == "DetailsDownloadStatus");
                cmd.Parameters.Add("@detailsDownloadStatus", System.Data.SqlDbType.Int).Value = detailsDownloadStatusFilter == null ? null : detailsDownloadStatusFilter.Value;

                //WHERE CONTAINS(item.keywords, '("wi*") AND ("#4*")')
                var textFilter = query.Filters.FirstOrDefault(f => f.Name == "TextFilter");
                var searchText = textFilter == null ? null : (string)textFilter.Value;
                var searchCondition = (string.IsNullOrWhiteSpace(searchText)) ? null : 
                    string.Join(" AND ",
                        searchText.Split(new char[]{' ', ',', '.', '?', ';', '!', ':', '\'', '"', '*', '+', '-'}, StringSplitOptions.RemoveEmptyEntries)
                                  .Where(s => s.Length >= 2 && Char.IsLetterOrDigit(s[0]))
                                  .Select(s => string.Format("(\"{0}*\")", s)));
                cmd.Parameters.Add("@searchCondition", System.Data.SqlDbType.NVarChar).Value = string.IsNullOrWhiteSpace(searchCondition) ? null : searchCondition;

                var isFavoriteFilter = query.Filters.FirstOrDefault(f => f.Name == "IsFavorite");
                cmd.Parameters.Add("@isFavorite", System.Data.SqlDbType.Bit).Value = isFavoriteFilter == null ? null : isFavoriteFilter.Value;

                cmd.Parameters.Add("@userId", System.Data.SqlDbType.Int).Value = 1;

                var floorMinFilter = query.Filters.FirstOrDefault(f => f.Name == "FloorMin");
                cmd.Parameters.Add("@floorMin", System.Data.SqlDbType.Int).Value = floorMinFilter == null ? null : floorMinFilter.Value;

                var floorMaxFilter = query.Filters.FirstOrDefault(f => f.Name == "FloorMax");
                cmd.Parameters.Add("@floorMax", System.Data.SqlDbType.Int).Value = floorMaxFilter == null ? null : floorMaxFilter.Value;

                var floorsMinFilter = query.Filters.FirstOrDefault(f => f.Name == "FloorsMin");
                cmd.Parameters.Add("@floorsMin", System.Data.SqlDbType.Int).Value = floorsMinFilter == null ? null : floorsMinFilter.Value;

                var floorsMaxFilter = query.Filters.FirstOrDefault(f => f.Name == "FloorsMax");
                cmd.Parameters.Add("@floorsMax", System.Data.SqlDbType.Int).Value = floorsMaxFilter == null ? null : floorsMaxFilter.Value;


                var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    var ad = ReadAdRealty(reader); 
                    ads.Add(ad.Id, ad); 
                    totalCount = reader.GetInt32("TotalCount");
                }
                while (reader.Read()) 
                { 
                    var ad = ReadAdRealty(reader); 
                    ads.Add(ad.Id, ad); 
                }

                reader.NextResult();
                while (reader.Read())
                {
                    var image = ReadAdImage(reader);
                    var ad = ads[image.AdId];
                    if (ad.Images == null)
                    {
                        ad.Images = new List<AdImage>();
                    }
                    ad.Images.Add(image);
                }

                reader.NextResult();
                while (reader.Read())
                {
                    var metadata = ReadMetadata(reader);
                    ads[metadata.AdId].Metadata = metadata;
                }
            });

            return new QueryResult<AdRealty>(ads.Values.ToList(), totalCount);
        }

        public List<AdRealty> GetLastAds(string connectorId, int limit)
        {
            Query lastAdQuery = new Query(0, limit);
            lastAdQuery.AddSort("CollectDate", SortOrder.Descending);
            lastAdQuery.AddFilter("ConnectorId", connectorId);

            return GetList(lastAdQuery).Items;
        }

        public List<AdRealty> GetAdsForTheSameObject(AdRealty adRealty)
        {
            List<AdRealty> result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertAllToEntity(context.Ads
                        .OfType<DbAdsRealty>()
                        .Where(a =>
                            a.Address == adRealty.Address &&
                            a.Floor == adRealty.Floor &&
                            a.FloorsCount == adRealty.FloorsCount &&
                            a.IdOnWebSite == adRealty.IdOnWebSite &&
                            a.LivingSpace == adRealty.LivingSpace &&
                            a.ConnectorId == adRealty.ConnectorId &&
                            a.RoomsCount == adRealty.RoomsCount)).ToList();
                        
            });
            return result;
        }

        public List<AdRealty> GetAdsObjects()
        {
            List<AdRealty> result = null;
            ExecuteDbOperation(context =>
            {
                result = context.Ads
                    .OfType<DbAdsRealty>()
                    .GroupBy(a => new 
                    { 
                        a.Address, 
                        a.Floor,
                        a.FloorsCount, 
                        a.IdOnWebSite, 
                        a.LivingSpace, 
                        a.RoomsCount 
                    })
                    .Select(g => new AdRealty()
                    {
                        Address = g.Key.Address,
                        Floor = g.Key.Floor,
                        FloorsCount = g.Key.FloorsCount,
                        IdOnWebSite = g.Key.IdOnWebSite,
                        LivingSpace = (float)g.Key.LivingSpace,
                        RoomsCount = g.Key.RoomsCount
                    })
                    .ToList();
            });
            return result;
        }

        public AdRealty GetItem(int id)
        {
            AdRealty result = null;
            ExecuteDbOperation(context =>
            {
                result = ConvertToEntity(context.Ads.OfType<DbAdsRealty>().Where(a => a.Id == id).FirstOrDefault());
            });
            return result;
        }

        public void AutoHistory()
        { 

        }
    }
}
