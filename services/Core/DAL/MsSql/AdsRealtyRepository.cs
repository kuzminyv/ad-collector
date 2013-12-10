using System;
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

namespace Core.DAL.MsSql
{
    public class AdsRealtyRepository : MsSqlRepository<DbAd, AdRealty, int>
    {
        private class DbAdsRealtyContainer
        {
            public DbAdsRealty Ad { get; set; }
            public int? HistoryLength { get; set; }
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
                IsNewBuilding = entity.IsNewBuilding
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
                Metadata = ((MetadataRepository)Repositories.MetadataRepository).ConvertToEntity(dbEntity.Metadatas.FirstOrDefault())
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

        protected IQueryable<DbAdsRealty> ApplyFilter(AdCollectorDBEntities context, IQueryable<DbAdsRealty> entities, List<Filter> filters)
        {
            var result = entities;
            foreach (var filter in filters)
            {
                switch (filter.Name)
                {
                    case "Suspect":
                        var suspect = (bool)filter.Value;
                        result = result.Where(t => t.IsSuspicious == suspect);
                        break;
                    case "ConnectorId":
                        var connectorId = (string)filter.Value;
                        result = result.Where(t => t.ConnectorId == connectorId);
                        break;
                    case "TextFilter":
                        var textFilter = (string)filter.Value;
                        if (string.IsNullOrEmpty(textFilter))
                        {
                            continue;
                        }
                        result = result.Where(t =>
                            t.Title.Contains(textFilter) ||
                            t.Description.Contains(textFilter) ||
                            t.Url.Contains(textFilter));
                        break;
                    case "PriceMin":
                        double priceMin = (int)filter.Value;
                        result = result.Where(t => t.Price >= priceMin);
                        break;
                    case "PriceMax":
                        double priceMax = (int)filter.Value;
                        result = result.Where(t => t.Price <= priceMax || t.Price == 0);
                        break;
                    case "DetailsDownloadStatus":
                        break;
                    case "IsNew":
                        break;
                    default:
                        throw new Exception(string.Format("Unknown filter '{0}'!", filter.Name));
                }
            }
            return result;
        }

        protected IQueryable<DbAdsRealty> ApplyOrder(AdCollectorDBEntities context, IQueryable<DbAdsRealty> entities, List<Sort> list)
        {
            var result = entities;
            Sort sort = list.First();
            switch (sort.Name)
            { 
                case "CollectDate":
                    if (sort.SortOrder == SortOrder.Ascending)
                        result = result.OrderBy(a => a.CollectDate);
                    else
                        result = result.OrderByDescending(a => a.CollectDate);
                    break;
                case "PublishDate":
                    if (sort.SortOrder == SortOrder.Ascending)
                        result = result.OrderBy(a => a.PublishDate);
                    else
                        result = result.OrderByDescending(a => a.PublishDate);
                    break;
                case "Price":
                    if (sort.SortOrder == SortOrder.Ascending)
                        result = result.OrderBy(a => a.Price);
                    else
                        result = result.OrderByDescending(a => a.Price);
                    break;
                default:
                    throw new Exception(string.Format("Not supported sort {0}!", sort.Name));
            }
            return result;
        }

        private DbQuery<DbAd> FillAdditionalProperties(DbSet<DbAd> entities, List<string> optionalFields)
        {
            DbQuery<DbAd> result = entities;
            foreach (var fieldName in optionalFields)
            {
                switch (fieldName)
                {
                    case "Metadata":
                        result = entities.Include("Metadatas");
                        break;
                }
            }
            return result;
        }

        public override QueryResult<AdRealty> GetList(Query query)
        {
            QueryResult<AdRealty> result = null;
            int? totalCount = null;

            ExecuteDbOperation(context =>
            {
                IQueryable<DbAdsRealty> dbEntities;

                if (query != null)
                {
                    if (query.HasOptionalFields)
                    {
                        dbEntities = FillAdditionalProperties(GetDbEntities(context), query.OptionalFields).OfType<DbAdsRealty>();
                    }
                    else
                    {
                        dbEntities = GetDbEntities(context).OfType<DbAdsRealty>();
                    }
                    if (query.HasFilters)
                    {
                        dbEntities = ApplyFilter(context, dbEntities, query.Filters);
                    }
                    if (query.HasSorts)
                    {
                        dbEntities = ApplyOrder(context, dbEntities, query.Sorts);
                    }
                    if (query.Start.HasValue && query.Limit.HasValue)
                    {
                        totalCount = dbEntities.Count();
                        if (query.Start > 0)
                        {
                            dbEntities = dbEntities.Skip(query.Start.Value);
                        }
                        dbEntities = dbEntities.Take(query.Limit.Value);
                    }

                    if (query.HasOptionalFields && query.OptionalFields.Contains("HistoryLength"))
                    {
                        result = new QueryResult<AdRealty>(ConvertAllToEntity(dbEntities.Select(a => new DbAdsRealtyContainer()
                        {
                            Ad = a,
                            HistoryLength = a.AdHistoryItems.Count()
                        })).ToList(), totalCount);
                    }
                }
                else
                {
                    dbEntities = GetDbEntities(context).OfType<DbAdsRealty>();
                }

                result = result ?? new QueryResult<AdRealty>(
                            ConvertAllToEntity(dbEntities).ToList(), totalCount);
            });

            return result;
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
