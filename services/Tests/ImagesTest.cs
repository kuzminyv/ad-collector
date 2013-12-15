using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Entities;
using Core.BLL;
using Core.DAL.Common;
using Core.DAL;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class ImagesTest
    {
        [TestMethod]
        public void ImagesTest_SaveTest()
        {
            Query query = new Query(0, 1);
            query.AddFields("Images");

            Ad ad = Managers.AdManager.GetAds(query).Items[0];
            Repositories.AdImagesRepository.SetList(ad.Id,
            new List<AdImage>()
            {
                new AdImage()
                {
                    PreviewUrl = "previewUrl",
                    Url = "url"
                }
            });

            ad = Managers.AdManager.GetAds(query).Items[0];
            Assert.AreNotEqual(ad.Images, null);
            Assert.IsTrue(ad.Images.Count == 1);

            Assert.AreEqual(ad.Images[0].PreviewUrl, "previewUrl");
            Assert.AreEqual(ad.Images[0].Url, "url");
        }
    }
}
