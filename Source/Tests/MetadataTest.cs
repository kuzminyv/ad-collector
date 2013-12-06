using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Entities;
using Core.BLL;
using Core.DAL.Common;

namespace Tests
{
    [TestClass]
    public class MetadataTest
    {
        [TestMethod]
        public void SaveTest()
        {
            Query query = new Query(0, 1);
            query.AddFields("Metadata");

            Ad ad = Managers.AdManager.GetAds(query).Items[0];
            Managers.MetadataManager.SaveItem(new Metadata()
            {
                AdId = ad.Id,
                UserId = 1,
                Note = "Test",
                IsFavorite = true
            });

            ad = Managers.AdManager.GetAds(query).Items[0];
            Assert.AreNotEqual(ad.Metadata, null);
            Assert.AreEqual(ad.Metadata.Note, "Test");
            Assert.AreEqual(ad.Metadata.IsFavorite, true);

            Managers.MetadataManager.SaveItem(new Metadata()
            {
                AdId = ad.Id,
                UserId = 1
            });
            ad = Managers.AdManager.GetAds(query).Items[0];
            Assert.AreEqual(ad.Metadata, null);



        }
    }
}
