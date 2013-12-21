using Core.BLL;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class AdsService : IAdsService
    {
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "data/{id}")]
        public string GetData(string id)
        {
            return string.Format("You entered: {0}", id);
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, 
            UriTemplate = "ads?query={query}&priceMin={priceMin}&priceMax={priceMax}&sortBy={sortBy}&sortDirection={sortDirection}&offset={offset}&limit={limit}")]
        public QueryResult<AdRealty> GetAds(string query, string priceMin, string priceMax, string sortBy, string sortDirection, string offset, string limit)
        {
            Query q = new Query();

            if (!string.IsNullOrEmpty(query))
            {
                q.AddFilter("TextFilter", query);
            }

            double? _priceMin = UriParamsHelper.ParseDouble(priceMin);
            if (_priceMin.HasValue)
            {
                q.AddFilter("PriceMin", _priceMin);
            }

            double? _priceMax = UriParamsHelper.ParseDouble(priceMax);
            if (_priceMax.HasValue)
            {
                q.AddFilter("PriceMax", _priceMax);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                SortOrder? _sortOrder = UriParamsHelper.ParseEnum<SortOrder>(sortDirection, SortOrder.Ascending);
                q.AddSort(sortBy, _sortOrder.Value);
            }
            else
            {
                q.AddSort("CollectDate", SortOrder.Descending);
            }

            q.Start = UriParamsHelper.ParseInt(offset, 0);
            q.Limit = UriParamsHelper.ParseInt(limit, 100);
            q.AddFields("Metadata", "HistoryLength");

            var result = Managers.AdManager.GetAds(q);
            return new QueryResult<AdRealty>(result.Items.OfType<AdRealty>().ToList(), result.TotalCount);
        }

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ads/{adId}/history")]
        public QueryResult<AdHistoryItem> GetAdHistory(string adId)
        {
            int? _adId = UriParamsHelper.ParseInt(adId);
            if (_adId.HasValue)
            {
                return new QueryResult<AdHistoryItem>(Managers.AdHistoryManager.GetAdHistory(_adId.Value));
            }
            return null;
        }

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "ads/{adId}/metadata")]
        public void SaveAdMetadata(string adId, Metadata metadata)
        {
            int? _adId = UriParamsHelper.ParseInt(adId);
            if (metadata != null && _adId.HasValue)
            {
                metadata.UserId = 1;
                metadata.AdId = _adId.Value;
                Managers.MetadataManager.SaveItem(metadata);
            }
        }
    }
}
