using Core.DAL.Binary.Common;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.DAL.Binary
{
    public class AdLinksRepository : BinaryRepository<AdLink, int>, IAdLinksRepository
    {
        protected override IEnumerable<AdLink> ApplyFilter(IEnumerable<AdLink> entities, List<Filter> filters) 
        {
            var result = entities;
            foreach (var filter in filters)
            {
                switch (filter.Name)
                {
                    case "AdId":
                        var adId = (int)filter.Value;
                        result = result.Where(t => t.AdId == adId);
                        break;
                    default:
                        throw new Exception(string.Format("Unknown filter '{0}'!", filter.Name));
                }
            }
            return result;
        }

        public List<AdLink> GetList(int adId)
        {
            Query query = new Query().AddFilter("AdId", adId);
            return GetList(query).Items;
        }

        public void CreateAutoLinks(Ad ad)
        {

            ExecuteDbOperation(() =>
                {
                    var similarAds = Repositories.AdsRepository.GetList(null).Items.Where(a => a.IsSimilarObject(ad) && a.Id != ad.Id).ToList();

                    //find exist link and drop them
                    var existsLinks = Entities.Where(ln => ln.Value.LinkType == LinkType.Automatic && similarAds.Any(sa => sa.Id == ln.Value.AdId || sa.Id == ln.Value.LinkedAdId))
                                              .Select(ln => ln.Key)
                                              .ToList();

                    foreach (var link in existsLinks)
                    {
                        DeleteItemById(link);
                    }

                    //find last Ad and create links to it
                    similarAds.Add(ad);
                    var lastAd = similarAds.OrderByDescending(a => a.CollectDate).First();
                    similarAds.Remove(lastAd);

                    var newLinks = new List<AdLink>();
                    foreach (var similarAd in similarAds)
                    {
                        newLinks.Add(new AdLink() { AdId = lastAd.Id, LinkedAdId = similarAd.Id, LinkType = LinkType.Automatic });
                    }
                    AddList(newLinks);
                });
        }

        public void DropAllAutoLinks()
        {
            var autoLinks = Entities.Where(kvp => kvp.Value.LinkType == LinkType.Automatic).ToList();
            ExecuteDbOperation(() =>
                {
                    foreach (var autoLink in autoLinks)
                    {
                        Entities.Remove(autoLink.Key);
                    }
                });
        }
    }
}
