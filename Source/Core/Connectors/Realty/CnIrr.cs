using Core.Entities;
using Core.Expressions;
using Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using RGX = System.Text.RegularExpressions;
using System.Threading;
using Core.BLL;

namespace Core.Connectors
{
    public class CnIrr : BasicConnector
    {
        public override string Id
        {
            get { return "http://saratov.irr.ru/"; }
        }

        public override string PageUrlFormat
        {
            get { return @"http://saratov.irr.ru/real-estate/apartments-sale/page{0}/"; }
        }

        public override Selector CreateSelector()
        {
            return new BoundExpressionSelector("Ad", new BoundExpression(
                    new BoundExpressionGroup("Start", new BoundSelector("add_list add_type4", ">")),
                    new BoundExpressionGroup("Premium", true, new BoundSelector("add_premium", ">")),
                    new BoundExpressionGroup("DetailUrl", new BoundSelector("add_list add_type4", "add_info", "a href=\"", "\"")),
                    new BoundExpressionGroup("Description", new BoundSelector("\">", "</a>")),
                    new BoundExpressionGroup("Price", new BoundSelector("add_cost", ">", "</div>")),
                    new BoundExpressionGroup("Date", new BoundSelector("add_data",">", "</p>"))
                ));
        }

        public override Selector CreateDetailsSelector()
        {
            return new BoundExpressionSelector("Details", new BoundExpression(new BoundExpressionGroup("Images", new BoundSelector("<div class=\"viewport\">", "</div>"))),
                        new BoundExpressionSelector("Preview", new BoundExpression(new BoundExpressionGroup("Url", new BoundSelector("img src=\"", "\""))))           		
		    );            		
        }

        protected override IEnumerable<string> GetPageUrlFormats()
        {
            yield return "http://saratov.irr.ru/real-estate/apartments-sale/search/price=%D0%BC%D0%B5%D0%BD%D1%8C%D1%88%D0%B5+1200000/currency=RUR/page{0}/";
            yield return "http://saratov.irr.ru/real-estate/apartments-sale/search/price=%D0%BE%D1%82+1200000+%D0%B4%D0%BE+1400000/currency=RUR/page{0}/";
            yield return "http://saratov.irr.ru/real-estate/apartments-sale/search/price=%D0%BE%D1%82+1400000+%D0%B4%D0%BE+1600000/currency=RUR/page{0}/";
            yield return "http://saratov.irr.ru/real-estate/apartments-sale/search/price=%D0%BE%D1%82+1600000+%D0%B4%D0%BE+1800000/currency=RUR/page{0}/";
        }

        public override int GetAvailablePagesCount(string pageFormat)
        {
            return 25;
        }

        private string[] _streetsDictionary;
        public string[] StreetsDictionary
        {
            get
            {
                if (_streetsDictionary == null)
                {
                    _streetsDictionary = Managers.StreetsManager.GetList(8452/*Saratov city*/).SelectMany(s => s.Name.Split(' ').Where(n => n.Length > 3)).ToArray();
                }
                return _streetsDictionary;
            }
        }

        public override bool FillDetails(Ad ad)
        {
            return false;
        }

        public override Ad CreateAd(Match match)
        {
            AdRealty ad = new AdRealty()
            {
                Title = match["Description"],
                Description = match["Description"],
                Url = Id + match["DetailUrl"],
                PublishDate = ParseDate(match["Date"]),
                CreationDate = ParseDate(match["Date"]),
                ConnectorId = this.Id,   
                BuildingType = Entities.Enums.BuildingType.Flat,
                Price = ParsePrice(match["Price"])
            };

            string[] description = match["Description"].Split(',');


            string roomsStr = description.FirstOrDefault(s => s.Contains("комн"));
            if (roomsStr != null)
            {
                RGX.Regex rgx = new RGX.Regex(@"(?<Rooms>\d{1})");
                var matches = rgx.Matches(roomsStr);
                if (matches.Count == 1)
                {
                    ad.RoomsCount = int.Parse(matches[0].Groups["Rooms"].Value);
                }
            }


            string spaceStr = description.FirstOrDefault(s => s.Contains("площадь") && s.Contains("общая"));
            if (spaceStr != null)
            {
                RGX.Regex rgx = new RGX.Regex(@"(?<Space>\d{2,3})");
                var matches = rgx.Matches(spaceStr);
                if (matches.Count == 1)
                {
                    ad.LivingSpace = int.Parse(matches[0].Groups["Space"].Value);
                }
            }


            string addressStr = description.FirstOrDefault(s =>
                LastWith(s, "ул") ||
                LastWith(s, "пр-кт") ||
                LastWith(s, "проезд") ||
                s.Contains("й проезд") ||
                LastWith(s, "улица"));
            addressStr = addressStr ?? description.FirstOrDefault(s => StreetsDictionary.Any(street => s.IndexOf(street, StringComparison.OrdinalIgnoreCase) > 0)); 
            if (addressStr != null)
            {
                ad.Address = addressStr.Replace("адрес:", "");
            }


            RGX.Regex floorsRgx = new RGX.Regex(@"(?<Floors>\d{1,2}/\d{1,2})");
            foreach (var descrPart in description)
            {
                var matches = floorsRgx.Matches(descrPart);
                if (matches.Count == 1)
                {
                    var floors = matches[0].Groups["Floors"].Value.Split('/');
                    ad.Floor = int.Parse(floors[0]);
                    ad.FloorsCount = int.Parse(floors[1]);
                }
            }
            return ad;
        }

        private bool LastWith(string str, string subStr)
        {
            int idx = str.IndexOf(subStr);
            return idx > 0 && str.Length == (idx + subStr.Length);
        }

        protected double ParsePrice(string price)
        {
            if (price.Contains("руб"))
            {
                return double.Parse(price.Replace("руб.", "").Replace(".", "").Trim(), CultureInfo.InvariantCulture);
            }
            return 0;            
        }

        protected DateTime ParseDate(string date)
        {
            return DateTime.ParseExact(date.Trim(), "HH:mm, dd.MM.yyyy", CultureInfo.InvariantCulture);
        }
    }
}
