using Core.Entities;
using Core.Expressions;
using Core.Expressions.AdParsers;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Connectors
{
    public class CnRealtfin : BasicConnector
    {
        public override string Id
        {
            get { return "http://realtfin.com"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://realtfin.com/saratov/obj/search/flat/?paged={0}"; }
        }

        public override ConnectorOptions GetOptions()
        {
            var options = base.GetOptions();
            options.MinAds = 100;
            options.FrameSize = 50;
            options.MinNewAdsInFrame = 0.1;
            options.IsSupportIdOnWebSite = true;
            return options;
        }

        public override Selector CreateSelector()
        {

            return new HtmlPathSelector("Ad", "//ul[@class='lenta']/li", false,
                new HtmlPathSelector("IsAdv", "div[@class='advInsert']", true),
                new HtmlPathSelector("DateText", "div[@class='td1']/text()", true),
                new HtmlPathSelector("Price", "//span[@class='value']/text()", true),
                new HtmlPathSelector("LivingSpaceText", "//div[@class='area']/text()", true, new RegexSelector("LivingSpace", "(?<LivingSpace>\\d{1,3}\\.?\\d?)[/\\s]")),
                new HtmlPathSelector("Rooms", "//span[@class='name']/span/text()", true),
                new HtmlPathSelector("Address", "//span[@class='address']/text()", true),
                new HtmlPathSelector("Details", "//div[@class='param-list']/text()", true,
                    new RegexSelector("Floor", "этаж:(?<Floor>\\d{1,2})/(?<Floors>\\d{1,2})[пк+]"),
                    new RegexSelector("Floors", "этаж:(?<Floor>\\d{1,2})/(?<Floors>\\d{1,2})[пк+]")),
                new HtmlPathSelector("Url", "//div[@class='td4']/a", true, false, "href",
                    new RegexSelector("Id", "id/(?<Id>\\d{4,8})")));
                
        }

        public override Selector CreateDetailsSelector()
        {
            return new HtmlPathSelector("Details", "//div[@class=\"content\"]", false,
                new HtmlPathSelector("Address", "//span[@class='address']/text()", true),
                new HtmlPathSelector("ImagePreviewUrl", "//ul[@class='list-foto']/li/img", true, false, "src"));
        }

        public override Ad CreateAd(Match match)
        {
            Thread.Sleep(300);
            if (!string.IsNullOrEmpty(match["IsAdv"]))
            {
                return null;
            }

            return new AdRealty()
            {
                Description = string.Format(match["Details"]),
                Url = "http://realtfin.com/" + match["Url"],
                PublishDate = ParsersHelper.ParseDate(match["DateText"]
                    .Replace("авг", "августа")
                    .Replace("сент", "сентября")
                    .Trim() + " " + DateTime.Now.Year, "d MMMM yyyy", null, CultureInfo.CreateSpecificCulture("ru-Ru")),
                ConnectorId = this.Id,
                Address = match["Address"],
                RoomsCount = ParsersHelper.ParseInt(match["Rooms"]),
                Floor = ParsersHelper.ParseInt(match["Details\\Floor"]),
                FloorsCount = ParsersHelper.ParseInt(match["Details\\Floors"]),
                LivingSpace = ParsersHelper.ParseFloat(match["LivingSpaceText\\LivingSpace"], "."),
                Price = ParsersHelper.ParseDouble(match["Price"], ".") * 1000000d,
                IdOnWebSite = match["Url\\Id"]
            };
        }

        public override void FillAdDetails(Ad ad, Match match)
        {
            Thread.Sleep(300);
            var previews = match.GetByPath("ImagePreviewUrl", true).Select(m => m.Value).ToList();

            List<AdImage> adImages = new List<AdImage>(previews.Count);
            for (int i = 0; i < previews.Count; i++)
            {
                adImages.Add(new AdImage()
                {
                    AdId = ad.Id,
                    PreviewUrl = "http://realtfin.com/" + previews[i],
                    Url = "http://realtfin.com/" + previews[i].Replace("m", "")
                });
            }
            ad.Images = adImages;

            AdRealty adRealty = (AdRealty)ad;
            adRealty.Address = match["Address"] ?? adRealty.Address;
        }
    }
}
