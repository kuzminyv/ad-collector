using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Entities;
using Core.BLL;
using Core.DAL.Common;
using Core.Entities.Enums;
using Core.DAL;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class AdsTest
    {
        [TestMethod]
        public void AdsTest_SortTest()
        {
            Query query = new Query();
            query.AddFilter("DetailsDownloadStatus", DetailsDownloadStatus.NotDownloaded);
            query.AddSort("CollectDate", SortOrder.Descending);

            var ads = Repositories.AdsRepository.GetList(query).Items;

            var sortedAds = ads.OrderByDescending(a => a.CollectDate).ToList();

            for (int i = 0; i < ads.Count; i++)
            {
                Assert.AreEqual(ads[i], sortedAds[i], "Sort by collectDate failed");
            }

        }
    }
}
