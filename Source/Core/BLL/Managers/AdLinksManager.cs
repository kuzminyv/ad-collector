using Core.DAL;
using Core.DAL.Common;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.BLL
{
    public class AdLinksManager
    {
        public List<Ad> GetLinkedAds(int adId)
        {
            return Repositories.AdsRepository.GetLinkedAds(adId);
        }

        public void CreateLinks(int adId, List<int> linkedAdsIds)
        {
            Repositories.AdLinksRepository.AddList(linkedAdsIds.Select(linkedId => new AdLink() { AdId = adId, LinkedAdId = linkedId }).ToList());
        }

        public void CreateAutoLinks(List<Ad> ads)
        {
            foreach (var ad in ads)
            {
                Repositories.AdLinksRepository.CreateAutoLinks(ad);
            }
        }

        public void RecreateAutoLinks()
        {
            Repositories.AdLinksRepository.DropAllAutoLinks();
            CreateAutoLinks(Repositories.AdsRepository.GetList(null).Items);
        }
    }
}
