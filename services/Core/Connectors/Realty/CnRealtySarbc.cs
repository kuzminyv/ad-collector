using Core.Entities;
using Core.Expressions;
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
            get { return @"http://realty.sarbc.ru/catalog/buy/flat?act=s&what=buy&want=flat&page={0}"; }
        }

        public override Selector CreateSelector()
        {
            return new BoundExpressionSelector("Ad", new BoundExpression(
                    new BoundExpressionGroup("DetailUrl", new BoundSelector("<td class=\"board-title\">", "<a href=\"", "\" target=\"_blank\"")),
                    new BoundExpressionGroup("Address", new BoundSelector("\">", "</a>")),
                    new BoundExpressionGroup("Date", new BoundSelector("<span class", ">", "</span>")),
                    new BoundExpressionGroup("Rooms", new BoundSelector("продаю", "<b>", "</b>")),
                    new BoundExpressionGroup("Floor", new BoundSelector("этаж:", "<br/>")), //этаж: 10 из 10
                    new BoundExpressionGroup("Size", new BoundSelector("площадь (общ/жил/кух):", "<br/>")), //70/50/16
                    new BoundExpressionGroup("Price", new BoundSelector("цена:", "</b>")) // 3,250.00 тыс. руб
                ));
        }

        public override Selector CreateDetailsSelector()
        {
            return new HtmlPathSelector("Details", "//*[@id=\"content\"]/div", false,
                new HtmlPathSelector("FirstImageUrl", "//*[@id=\"tur\"]/a/@href", true, false, "href"),
                new HtmlPathSelector("FirsrImagePreviewUrl", "//*[@id=\"tur\"]/a/img/@src", true, false, "src"),
                new HtmlPathSelector("Description", "/div/dl/dd/p", true, true, null),
                new HtmlPathSelector("Images", "/div/table[2]//tr//td", true, true, null,
                	 new HtmlPathSelector("Url", "/a/@href", true, true, "href"),
                	 new HtmlPathSelector("PreviewUrl", "/a/img/@src", true, true, "src")));
        }

        public override Ad CreateAd(Match match)
        {
            return new AdRealty()
            {
                Title = string.Format("{0}, {1}, {2} ", match["Price"], match["Address"], match["Size"]),
                Description = string.Format("Price: {0}\nAddress: {1}\nFloor: {2}\nSize: {3}", match["Price"], match["Address"], match["Floor"], match["Size"]),
                Url = Id + match["DetailUrl"],
                PublishDate = ParseDate(match["Date"]),
                ConnectorId = this.Id,
                Address = match["Address"],
                RoomsCount = ParseRooms(match["Rooms"]),
                Floor = ParseFloor(match["Floor"]),
                FloorsCount = ParseFloors(match["Floor"]),
                LivingSpace = ParseSize(match["Size"]),
                Price = ParsePrice(match["Price"])
            };
        }

        protected override void FillAdDetails(Ad ad, Match match)
        {
            ad.Images = match.GetByPath(@"Details\Images", true).Select(img => new AdImage()
            {
                AdId = ad.Id,
                PreviewUrl = img["PreviewUrl"],
                Url = img["Url"]
            }).ToList();
            ad.Description = match["Description"];

            if (!string.IsNullOrEmpty(match["FirstImageUrl"]))
            {
                ad.Images.Add(new AdImage()
                {
                    AdId = ad.Id,
                    PreviewUrl = match["FirsrImagePreviewUrl"],
                    Url = match["FirstImageUrl"]
                });
            }
        }

        protected int ParseRooms(string rooms)
        {
            return int.Parse(rooms);
        }

        protected double ParsePrice(string price)
        {
            if (price.Contains("договор") || price.Contains("м") || price.Contains("$")) 
            {
                return 0;
            }
            return double.Parse(price.Replace("тыс. руб", "").Replace(",", "").Trim(), CultureInfo.InvariantCulture) * 1000;
        }

        protected float ParseSize(string size)
        {
            return float.Parse(size.Trim().Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[0], CultureInfo.InvariantCulture);
        }

        protected int ParseFloors(string floors)
        {
            if (!string.IsNullOrEmpty(floors))
            {
                string[] parts = floors.Split(new string[] { "из" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    return int.Parse(parts[0].Trim());
                }
                return int.Parse(parts[1].Trim());
            }
            return 0;
        }

        protected int ParseFloor(string floor)
        {
            if (!string.IsNullOrEmpty(floor))
            {
                return int.Parse(floor.Split(new string[] { "из" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim());
            }
            return 0;
        }

        protected DateTime ParseDate(string date)
        {
            if (date.Contains("сегодня"))
            {
                DateTime time = DateTime.ParseExact(date.Replace("сегодня", "").Trim(), "H:mm", CultureInfo.InvariantCulture);
                return DateTime.Today.AddHours(time.Hour).AddMinutes(time.Minute);
            }
            else
            {
                return DateTime.ParseExact(date.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
        }
    }
}
