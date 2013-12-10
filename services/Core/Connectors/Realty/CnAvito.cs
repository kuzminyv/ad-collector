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
    public class CnAvito : BasicConnector
    {
        public override string Id
        {
            get { return "http://www.avito.ru/saratov/kvartiry/prodam/vtorichka"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://www.avito.ru/saratov/kvartiry/prodam/vtorichka?p={0}"; }
        }

        public override Selector CreateSelector()
        {
            return new BoundExpressionSelector("Ad", new BoundExpression(
                    new BoundExpressionGroup("Date", new BoundSelector("t_i_i t_i_odd t_i_e_r", "t_i_date\">", "<")),
                    new BoundExpressionGroup("Time", new BoundSelector("t_i_time\">", "<")),
                    new BoundExpressionGroup("Url", new BoundSelector("href=\"", "\"")),
                    new BoundExpressionGroup("Title", new BoundSelector("title=\"", "\">")),
                    new BoundExpressionGroup("Description", new BoundSelector("t_i_description", ">", "</div>"))
                ));
        }

        public override Ad CreateAd(Match match)
        {
            return new AdRealty()
            {
                Url = "http://www.avito.ru" + match["Url"]
            };
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
            string[] parts = floors.Split(new string[] { "из" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
            {
                return int.Parse(parts[0].Trim());
            }
            return int.Parse(parts[1].Trim());
        }

        protected int ParseFloor(string floor)
        {
            return int.Parse(floor.Split(new string[] { "из" }, StringSplitOptions.RemoveEmptyEntries)[0].Trim());
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
