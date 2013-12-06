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
    public class CnKvadrat64NewBuildings : CnKvadrat64
    {
        public override string Id
        {
            get { return "http://www.kvadrat64.ru/newflatbank"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://www.kvadrat64.ru/newflatbank-1000-{0}.html"; }
        }

        public override Selector CreateSelector()
        {
            var adTerminationSelector = new BoundSelector("</tab", "le>");

            var date = new BoundExpressionGroup("Date", new BoundSelector("strup> &#8593; </a>", "</div></td>"));
            var price = new BoundExpressionGroup("Price", new BoundSelector("class=zhyofoto>", "р<br>"),
                                                     new BoundSelector("class=tprice>", "р<br>"));

            var detailUrl = new BoundExpressionGroup("DetailUrl", new BoundSelector("<A href='newflat-", "'") { StringComparison = StringComparison.OrdinalIgnoreCase });
            var rooms = new BoundExpressionGroup("Rooms", new BoundSelector("class=site3", ">", "(") { Superposition = true });

            var size = new BoundExpressionGroup("Size", new BoundSelector("(", ")"));
            var address = new BoundExpressionGroup("Address", new BoundSelector(",", ","));
            var floor = new BoundExpressionGroup("Floor", new BoundSelector("этаж", " ,"), adTerminationSelector);

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

        public override Ad CreateAd(Match match)
        {
            return new AdRealty()
            {
                Title = string.Format("{0}, {1}, {2} ", match["Price"], match["Address"], match["Size"]),
                Description = string.Format("Price: {0}\nAddress: {1}\nFloor: {2}\nSize: {3}", match["Price"], match["Address"], match["Floor"], match["Size"]),
                Url = "http://www.kvadrat64.ru" + string.Format("/newflat-{0}", match["DetailUrl"]),
                PublishDate = ParseDate(match["Date"]),
                ConnectorId = this.Id,
                Address = match["Address"],
                RoomsCount = ParseRooms(match["Rooms"]),
                Floor = ParseFloor(match["Floor"]),
                FloorsCount = ParseFloors(match["Floor"]),
                LivingSpace = ParseSize(match["Size"]),
                Price = ParsePrice(match["Price"]),
                IsNewBuilding = true
            };
        }

        protected override int ParseFloors(string floors)
        {
            //этаж 6/10
            return int.Parse(floors.Replace("этаж", "").Trim().Split('/')[1]);
        }

        protected override int ParseFloor(string floor)
        {
            if (floor.IndexOf("средние", StringComparison.OrdinalIgnoreCase) >= 0 ||
                floor.IndexOf("средний", StringComparison.OrdinalIgnoreCase) >= 0 ||
                floor.IndexOf("ср", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ParseFloors(floor) / 2;
            }
            if (floor.IndexOf("все", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return ParseFloors(floor);
            }

            return int.Parse(floor.Replace("этаж", "").Trim().Split('/')[0].Split(new char[]{',', '-'}, StringSplitOptions.RemoveEmptyEntries).Last().Trim());
        }

        protected override float ParseSize(string size)
        {
            return float.Parse(size.Split('/')[0].Split('-')[0].Trim(), CultureInfo.InvariantCulture);
        }
    }
}
