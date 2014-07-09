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
    public class CnRealtySarbc : BasicConnector
    {
        public override string Id
        {
            get { return "http://www.realty.sarbc.ru"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://realty.sarbc.ru/catalog/buy/flat/?page={0}"; }
        }

        public override WebClientOptions GetWebClientOptions()
        {
            return new WebClientOptions() { IgnoreError404 = true };
        }

        public override Selector CreateSelector()
        {

            return new HtmlPathSelector("Ad", "//div[@class='item_container']", false,
                new RegexSelector("Url", ".*href=\\\"(?<Url>.*)\".*"),
                new RegexSelector("Rooms", ".*(?<Rooms>\\d)-х\\sком.*"),
                new RegexSelector("Address", "address.*>.*>(?<Address>.*)</a>"),
                new RegexSelector("Price", "price.*>.*>(?<Price>.*)</span>\\sруб"),
                new RegexSelector("Floor", "Этаж:\\s</span>(?<Floor>\\d{1,2});"),
                new RegexSelector("Floors", "(?:Этажность\\sдома|Этажей\\sв\\sдоме):\\s</span>(?<Floors>\\d{1,2});"),
                new RegexSelector("LivingSpace", "лощадь:\\s</span>(?<LivingSpace>\\d{1,3}\\.?\\d{0,2})(?:;|\\s)"),
                new RegexSelector("Id", "other-board-id\\\">ID\\s(?<Id>\\d{2,10})</span>"));
        }

        public override Selector CreateDetailsSelector()
        {
            return new HtmlPathSelector("Details", "//*[@id=\"content\"]", false,
                new HtmlPathSelector("ImageUrl", "//*[@class=\"fancybox item-photo-link\"]", true, false, "href"),
                new HtmlPathSelector("ImagePreviewUrl", "//*[@class=\"item-photo\"]", true, false, "src"),
                new HtmlPathSelector("Description", "//*[@class=\"item-text\"]/text()", true));
        }

        public override Ad CreateAd(Match match)
        {
            return new AdRealty()
            {
                Title = string.Format("{0}, {1}, {2} ", match["Price"], match["Address"], match["LivingSpace"]),
                Description = string.Format("Price: {0}\nAddress: {1}\nFloor: {2}\nSize: {3}", match["Price"], match["Address"], match["Floor"], match["LivingSpace"]),
                Url = "http://www.realty.sarbc.ru" + match["Url"],
                PublishDate = DateTime.Now,
                ConnectorId = this.Id,
                Address = match["Address"],
                RoomsCount = ParsersHelper.ParseInt(match["Rooms"]),
                Floor = ParsersHelper.ParseInt(match["Floor"]),
                FloorsCount = ParsersHelper.ParseInt(match["Floors"]),
                LivingSpace = ParsersHelper.ParseFloat(match["LivingSpace"], "."),
                Price = ParsersHelper.ParseDouble(match["Price"], "."),
                IdOnWebSite = match["Id"]
            };
        }

        public override void FillAdDetails(Ad ad, Match match)
        {
            var images = match.GetByPath(@"ImageUrl", true).Select(m => m.Value).ToList();
            var previews = match.GetByPath(@"ImagePreviewUrl", true).Select(m => m.Value).ToList();

            List<AdImage> adImages = new List<AdImage>(images.Count);
            for (int i = 0; i < images.Count; i++)
            {
                adImages.Add(new AdImage()
                {
                    AdId = ad.Id,
                    PreviewUrl = "http://realty.sarbc.ru" + previews[i],
                    Url = "http://realty.sarbc.ru" + images[i]
                });
            }
            ad.Images = adImages;

            var description = match.GetByPath(@"Description", true).LastOrDefault();
            ad.Description = description == null ? ad.Description : description.Value;
        }
    }
}
