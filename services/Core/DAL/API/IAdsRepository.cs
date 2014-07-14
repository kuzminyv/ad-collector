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

namespace Core.DAL
{
	public interface IAdsRepository
	{

        List<Ad> GetLastAds(string sourceUrl, int limit);
        List<Ad> GetLinkedAds(int adId);

        void AddList(List<Ad> list);
        void AddItem(Ad ad);

        QueryResult<Ad> GetList(Query query);
        List<TAd> GetAdsObjects<TAd>();
        List<Ad> GetAdsForTheSameObject(Ad ad, bool isSupportedIdOnWebSite);
        void DeleteItems(List<int> ids);
        void UpdateItem(Ad ad);

        Ad GetItem(int id);
    }
}
