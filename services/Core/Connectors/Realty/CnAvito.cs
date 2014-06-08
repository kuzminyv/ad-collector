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
    public class CnAvito : BasicConnector
    {
        public override string Id
        {
            get { return "http://www.avito.ru/saratov/kvartiry/prodam"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://www.avito.ru/saratov/kvartiry/prodam?p={0}"; }
        }


        public override Selector CreateSelector()
        {
            return new HtmlPathSelector("Ad", "//div[@class=\"b-catalog-table\"]//div[starts-with(@id, \"i\")]", false,
                new HtmlPathSelector("Date", "//div[@class=\"date\"]/text()", true),
                new HtmlPathSelector("Time", "//span[@class=\"time\"]/text()", true),
                new HtmlPathSelector("Url", "//h3[@class=\"title\"]/a/@href", true, false, "href"),
                new HtmlPathSelector("Title", "//h3[@class=\"title\"]/a/text()", true,
                    new RegexSelector("Rooms", "(?<Rooms>\\d)-к"),
                    new RegexSelector("LivingSpace", @"(?<LivingSpace>[0-9\.]{1,6})\sм"),
                    new RegexSelector("Floor", @"(?<Floor>\d{1,2})/\d{1,2}\sэт"),
                    new RegexSelector("Floors", @"\d{1,2}/(?<Floors>\d{1,2})\sэт")
                ),
                new HtmlPathSelector("About", "//div[@class=\"about\"]", true, 
                    FilterSelector.HtmlToText(new RegexSelector("Price", @"(?<Price>[0-9\s]{1,12})\sруб"))),
                new HtmlPathSelector("Address", "//p[@class=\"address fader\"]/text()", true)
            );
        }

        public override Selector CreateDetailsSelector()
        {
            return new HtmlPathSelector("Item", "//div[@class=\"item\"]", false,
                new HtmlPathSelector("Image", "//div[@id=\"photo\"]//div[@class=\"items\"]/div", true,
                    new HtmlPathSelector("Big", "//a/@href", true, false, "href"),
                    new HtmlPathSelector("Small", "//img/@src", true, false, "src")
                ),
                new HtmlPathSelector("Description", "//div[@id=\"desc_text\"]/p/text()", true)
            );
        }

        public override Ad CreateAd(Match match)
        {
            Thread.Sleep(300);
            AdRealty ad = new AdRealty()
            {
                ConnectorId = this.Id,
                CreationDate = DateTime.Now,
                PublishDate = ParsersHelper.ParseDate(match["Date"].Trim() + " " + match["Time"].Trim(), new string[] { "d MMM.", "d MMMM" }, "HH:mm", CultureInfo.CreateSpecificCulture("ru-Ru"), "сегодня", "вчера"),
                Url = "http://www.avito.ru" + match["Url"],
                RoomsCount = ParsersHelper.ParseInt(match["Title\\Rooms"]),
                LivingSpace = ParsersHelper.ParseFloat(match["Title\\LivingSpace"], "."),
                Floor = ParsersHelper.ParseInt(match["Title\\Floor"]),
                FloorsCount = ParsersHelper.ParseInt(match["Title\\Floors"]),
                Price = ParsersHelper.ParseDouble(match["About\\Price"]),
                Address = match["Address"],
                IdOnWebSite = match["Url"].Substring(match["Url"].LastIndexOf('/'))
            };

            if ((ad.PublishDate - DateTime.Now).TotalDays > 30)
            {
                ad.PublishDate = ad.PublishDate.AddYears(-1);
            }
            return string.IsNullOrEmpty(ad.Address) ? null : ad;
        }

        public override void FillAdDetails(Ad ad, Match match)
        {
            Thread.Sleep(2000);

            ad.Description = match["Description"];

            var bigImages = match.GetByPath("Item\\Image\\Big", false).ToList();
            var smallImages = match.GetByPath("Item\\Image\\Small", false).ToList();

            if (smallImages.Count > 0)
            {
                ad.Images = new List<AdImage>();
                for (int i = 0; i < bigImages.Count; i++)
                {
                    ad.Images.Add(new AdImage()
                    {
                        AdId = ad.Id,
                        Url = "http:" + bigImages[i].Value,
                        PreviewUrl = "http:" + smallImages[i].Value
                    });
                }
            }
            
        }
    }
}
