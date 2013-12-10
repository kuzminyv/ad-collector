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
using Core.DAL.Binary.Common;

namespace Core.DAL.Binary
{
    public class AdsRepository : BinaryRepository<Ad, int>, IAdsRepository
	{
        protected override IEnumerable<Ad> ApplyFilter(IEnumerable<Ad> entities, List<Filter> filters)
        {
            var result = entities;
            foreach(var filter in filters)
            {
                switch (filter.Name)
                {
                    case "Suspect":
                        var suspect = (bool)filter.Value;
                        result = result.Where(t => t.IsSuspicious == suspect);
                        break;
                    case "SourceUrl":
                        var sourceUrl = (string)filter.Value;
                        result = result.Where(t => t.ConnectorId == sourceUrl);
                        break;
                    case "IsNew":
                        var isNew = (bool)filter.Value;
                        result = result.Where(t => t.IsNew == isNew);
                        break;
                    case "TextFilter":
                        var textFilter = (string)filter.Value;
                        if (string.IsNullOrEmpty(textFilter))
                        {
                            continue;
                        }

                        var tokens = textFilter.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var words = tokens.Where(t => t[0] != '-').ToArray();
                        foreach (var token in tokens)
                        {
                            if (token[0] == '-' && token.Length > 1)
                            {
                                string word = token.Substring(1);
                                result = result.Where(t =>
                                    !(
                                        (t.Description == null || t.Description.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                        (t.Title == null || t.Title.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                        (t.Url == null || t.Url.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                        (t.ConnectorId == null || t.ConnectorId.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                        (t.Price.ToString(CultureInfo.InvariantCulture).IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                        (t.PublishDate.ToString(CultureInfo.InvariantCulture).IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0)
                                    )
                                );
                            }
                        }
                        if (words.Length > 0)
                        {
                            result = result.Where(t =>
                                (t.Description == null || ContainsAny(t.Description, words)) ||
                                (t.Title == null || ContainsAny(t.Title, words)) ||
                                (t.Url == null || ContainsAny(t.Url, words)) ||
                                (t.ConnectorId == null || ContainsAny(t.ConnectorId, words)) ||
                                ContainsAny(t.Price.ToString(CultureInfo.InvariantCulture), words) ||
                                ContainsAny(t.PublishDate.ToString(CultureInfo.InvariantCulture), words));
                        }

                        break;
                    case "PriceMin":
                        double priceMin = (int)filter.Value;
                        result = result.Where(t => t.Price >= priceMin);
                        break;
                    case "PriceMax":
                        double priceMax = (int)filter.Value;
                        result = result.Where(t => t.Price <= priceMax || t.Price == 0);
                        break;
                    default:
                        throw new Exception(string.Format("Unknown filter '{0}'!", filter.Name));
                }
            }
            return result;
        }

        private bool ContainsAny(string str, string[] words)
        {
            for (int i = 0; i < words.Length; i++)
            {
                if (str.IndexOf(words[i], StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override IEnumerable<Ad> FillOptionalFields(IEnumerable<Ad> entities, List<string> fields)
        {
            var result = entities;
            foreach (var field in fields)
            {
                switch (field)
                {
                    case "LinkedAdsCount":
                        result = result.GroupJoin(Repositories.AdLinksRepository.GetList(null).Items, a => a.Id, l => l.AdId,
                            (ad, links) => { ad.LinkedAdsCount = links.Count(); return ad; });
                        break;
                    case "HistoryLength":
                        result = result.GroupJoin(Repositories.AdHistoryItemsRepository.GetList(null).Items, a => a.Id, h => h.AdId,
                            (ad, historyItems) => { ad.HistoryLength = historyItems.Count(); return ad; });
                        break;
                    default:
                        throw new Exception(string.Format("Unknown field '{0}'!", field));
                }
            }
            return result;
        }

        public List<Ad> GetLastAds(string sourceUrl, int limit)
        {
            Query lastAdQuery = new Query(0, limit);
            lastAdQuery.AddSort("CollectDate", SortOrder.Descending);
            lastAdQuery.AddFilter("SourceUrl", sourceUrl);

            return base.GetList(lastAdQuery).Items;
        }

        public List<Ad> GetLinkedAds(int adId)
        {
            lock (_lockObject)
            {
                return Repositories.AdLinksRepository.GetList(adId).Select(l => Entities[l.LinkedAdId]).OrderByDescending(kvp => kvp.CollectDate).ToList();
            }
        }


        public List<TAd> GetAdsObjects<TAd>()
        {
            lock (_lockObject)
            {
                return Entities.Select(kvp => kvp.Value).OfType<AdRealty>()
                    .GroupBy(a => new 
                    { 
                        a.Address, 
                        a.Floor, 
                        Floors = a.FloorsCount, 
                        a.IdOnWebSite, 
                        a.LivingSpace, 
                        Rooms = a.RoomsCount 
                    })
                    .Select(g => new AdRealty()
                    {
                        Address = g.Key.Address,
                        Floor = g.Key.Floor,
                        FloorsCount = g.Key.Floors,
                        IdOnWebSite = g.Key.IdOnWebSite,
                        LivingSpace = g.Key.LivingSpace,
                        RoomsCount = g.Key.Rooms
                    })
                    .ToList() as List<TAd>;
            }
        }

        public List<Ad> GetAdsForTheSameObject(Ad ad)
        {
            lock (_lockObject)
            {
                AdRealty adRealty = ad as AdRealty;
                return Entities.Select(kvp => kvp.Value).OfType<AdRealty>()
                                .Where(a =>
                                    a.Address == adRealty.Address &&
                                    a.Floor == adRealty.Floor &&
                                    a.FloorsCount == adRealty.FloorsCount &&
                                    a.IdOnWebSite == adRealty.IdOnWebSite &&
                                    a.LivingSpace == adRealty.LivingSpace &&
                                    a.RoomsCount == adRealty.RoomsCount).ToList<Ad>();
            }
        }
    }
}
