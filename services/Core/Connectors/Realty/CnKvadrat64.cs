using Core.Entities;
using Core.Expressions;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Core.BLL;

namespace Core.Connectors
{
    public class CnKvadrat64 : BasicConnector
    {
        public override string Id
        {
            get { return "http://www.kvadrat64.ru"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://www.kvadrat64.ru/sellflatbank-50-{0}.html"; }
        }

        public override Selector CreateSelector()
        {
            var adTerminationSelector = new BoundSelector("</tab", "le>");

            var date = new BoundExpressionGroup("Date", new BoundSelector("strup> &#8593; </a>", "</div></td>"));
            var price = new BoundExpressionGroup("Price", new BoundSelector("class=zhyofoto>", "р<br>"),
                                                     new BoundSelector("class=tprice>", "р<br>"));

            var detailUrl = new BoundExpressionGroup("DetailUrl", new BoundSelector("<A href='sellflat-", "'") { StringComparison = StringComparison.OrdinalIgnoreCase });
            var rooms = new BoundExpressionGroup("Rooms", new BoundSelector("class=site3", ">", "(") { Superposition = true });

            var size = new BoundExpressionGroup("Size", new BoundSelector("(", ")"));
            var address = new BoundExpressionGroup("Address", new BoundSelector(",", ","));
            var floor = new BoundExpressionGroup("Floor", new BoundSelector("этаж", ","), adTerminationSelector);

            var withoutDate = new BoundExpressionGroup("C1", new BoundSelector("class=strup>", "</a> </td>"));
            var withDate = new BoundExpressionGroup("C2", new BoundSelector("class=strup>", "</div></td>"));

            return new BoundExpressionSelector("Ad", new BoundExpression(
                    new BoundExpressionCondition(
                        withoutDate, new BoundExpressionToken[] 
                        {
                            price,
                            detailUrl,
                            rooms,
                            size,
                            address,
                            floor
                        },

                        withDate, new BoundExpressionToken[]
                        {
                            date,
                            price,
                            detailUrl,
                            rooms,
                            size,
                            address,
                            floor
                        }
                    )
                ));
        }

        public override Selector CreateDetailsSelector()
        {
            return new HtmlPathSelector("Details", "/html/body/table[3]", false,
                new HtmlPathSelector("Description", "//p[@class=\"dinfo\"]", true, true, null),
                new HtmlPathSelector("ImagePreviewUrl", "//td[@class=\"tdimg\"]/a/img/@src", true, false, "src"));
        }

        public override void FillAdDetails(Ad ad, Match match)
        {
            ad.Images = match.GetByPath(@"ImagePreviewUrl", true).Select(previewUrl => new AdImage()
            {
                AdId = ad.Id,
                PreviewUrl = Id + "/" + previewUrl.Value,
                Url = Id + "/" + previewUrl.Value.Replace("s", "b")
            }).ToList();
            ad.Description = match["Description"];
        }

        public override Ad CreateAd(Match match)
        {
            return new AdRealty()
            {
                Title = string.Format("{0}, {1}, {2} ", match["Price"], match["Address"], match["Size"]),
                Description = string.Format("Price: {0}\nAddress: {1}\nFloor: {2}\nSize: {3}", match["Price"], match["Address"], match["Floor"], match["Size"]),
                Url = Id + string.Format("/sellflat-{0}", match["DetailUrl"]),
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

        protected virtual float ParseSize(string size)
        {
            return float.Parse(size.Split('/')[0], CultureInfo.InvariantCulture);
        }

        protected double ParsePrice(string price)
        {
            //1 600 000 р
            return int.Parse(price.Replace(" ", ""));
        }

        protected int ParseRooms(string rooms)
        {
            return int.Parse(rooms.Substring(0, 1));
        }

        protected virtual int ParseFloors(string floors)
        {
            //этаж 6/10
            return int.Parse(floors.Replace("этаж", "").Trim().Split('/')[1]);
        }

        protected virtual int ParseFloor(string floor)
        {
            return int.Parse(floor.Replace("этаж", "").Trim().Split('/')[0]);
        }

        protected DateTime ParseDate(string date)
        {
            if (date == null)
            {
                return DateTime.Today;
            }
            else
            {
                return DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
            }
        }
    }
}
