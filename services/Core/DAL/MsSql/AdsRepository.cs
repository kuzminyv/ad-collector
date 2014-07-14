using Core.DAL.API;
using Core.DAL.Common;
using Core.DAL.MsSql.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAL.MsSql
{
    public class AdsRepository : IAdsRepository, IRepository<Ad>
    {
        private AdsRealtyRepository _realtyRepository;

        public List<Ad> GetLastAds(string sourceUrl, int limit)
        {
            return _realtyRepository.GetLastAds(sourceUrl, limit).ToList<Ad>();
        }

        public List<Ad> GetLinkedAds(int adId)
        {
            return new List<Ad>();
        }

        public void AddList(List<Ad> list)
        {
            _realtyRepository.AddList(list.ConvertAll(a => (AdRealty)a).ToList());
        }

        public QueryResult<Ad> GetList(Query query)
        {
            var result = _realtyRepository.GetList(query);
            return new QueryResult<Ad>(result.Items.ToList<Ad>(), result.TotalCount);
        }

        public List<Ad> GetAdsForTheSameObject(Ad ad, bool isSupportedIdOnWebSite)
        {
            if (ad.GetType() == typeof(AdRealty))
            {
                return _realtyRepository.GetAdsForTheSameObject((AdRealty)ad, isSupportedIdOnWebSite).ToList<Ad>();
            }
            return null;
        }

        public List<TAd> GetAdsObjects<TAd>()
        {
            if (typeof(TAd) == typeof(AdRealty))
            {
                return _realtyRepository.GetAdsObjects() as List<TAd>;
            }
            return null;
        }

        public void DeleteItems(List<int> ids)
        {
            _realtyRepository.DeleteItems(ids);
        }

        public AdsRepository()
        {
            _realtyRepository = new AdsRealtyRepository();
        }


        public void UpdateItem(Ad item)
        {
            _realtyRepository.UpdateItem((AdRealty)item);
        }


        public void AddItem(Ad item)
        {
            _realtyRepository.AddItem((AdRealty)item);
        }


        public Ad GetItem(int id)
        {
            return _realtyRepository.GetItem(id);
        }
    }
}
