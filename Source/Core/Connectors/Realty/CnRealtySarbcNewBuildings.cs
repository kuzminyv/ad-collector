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
    public class CnRealtySarbcNewBuildings : CnRealtySarbc
    {
        public override string Id
        {
            get { return "http://www.realty.sarbc.ru/catalog/buy/newbuilding"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://realty.sarbc.ru/catalog/buy/newbuilding?act=s&what=buy&want=newbuilding&page={0}"; }
        }

        public override Selector CreateSelector()
        {
            return new BoundExpressionSelector("Ad", new BoundExpression(
                    new BoundExpressionGroup("DetailUrl", new BoundSelector("<td class=\"board-title\">", "<a href=\"", "\"")),
                    new BoundExpressionGroup("Address", new BoundSelector("\">", "</a>")),
                    new BoundExpressionGroup("CommissioningDate", new BoundSelector("срок сдачи:", "<br>")),
                    new BoundExpressionGroup("Date", new BoundSelector("<span class", ">", "</span>")),
                    new BoundExpressionGroup("Rooms", new BoundSelector("продаётся", "<b>", "</b>")),
                    new BoundExpressionGroup("Floor", true, new BoundSelector("этаж:", "<br/>")), //этаж: 10 из 10 
                    new BoundExpressionGroup("Floors", true, new BoundSelector("этажность:", "<br/>")), //
                    new BoundExpressionGroup("Size", new BoundSelector("площадь (общ/жил/кух):", "<br/>")), //70/50/16
                    new BoundExpressionGroup("Price", new BoundSelector("цена:", "</b>")) // 3,250.00 тыс. руб
                ));
        }

        public override Ad CreateAd(Match match)
        {
            return new AdRealty()
            {
                Title = string.Format("{0}, {1}, {2} ", match["Price"], match["Address"], match["Size"]),
                Description = string.Format("Price: {0}\nAddress: {1}\nFloor: {2}\nSize: {3}", match["Price"], match["Address"], match["Floor"], match["Size"]),
                Url = "http://www.realty.sarbc.ru" + match["DetailUrl"],
                PublishDate = ParseDate(match["Date"]),
                ConnectorId = this.Id,
                Address = match["Address"],
                RoomsCount = ParseRooms(match["Rooms"]),
                Floor = ParseFloor(match["Floor"]),
                FloorsCount = ParseFloors(match["Floor"]),
                LivingSpace = ParseSize(match["Size"]),
                Price = ParsePrice(match["Price"]),
                IsNewBuilding = true,
                CommissioningDate = ParseCommissioningDate(match["CommissioningDate"])
            };
        }

        private DateTime? ParseCommissioningDate(string commissioningDate)
        {
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear; year < (currentYear + 20); year++)
            {
                if (commissioningDate.IndexOf(year.ToString()) >= 0)
                {
                    return new DateTime(year, 6, 1);
                }
            }

            return null;
        }
    }
}
