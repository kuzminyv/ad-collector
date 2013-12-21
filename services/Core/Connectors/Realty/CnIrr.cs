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
using Core.Expressions.AdParsers;

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


        public override Selector CreateSelector()
        {
            return new BoundExpressionSelector("Ad", new BoundExpression(
                    new BoundExpressionGroup("Start", new BoundSelector("add_type4", ">")),
                    new BoundExpressionGroup("Date", true, new BoundSelector("adv_data", ">", "<")),
                    new BoundExpressionGroup("DetailUrl", new BoundSelector("add_title_wrap", "href=\"", "\"")),
                    new BoundExpressionGroup("Address", new BoundSelector("add_title", ">", "<")),
                    new BoundExpressionGroup("Rooms", new BoundSelector("p_room", "<p>", "<")),
                    new BoundExpressionGroup("LivingSpace", new BoundSelector("p_square", "<p>", "<")),
                    new BoundExpressionGroup("Floor", new BoundSelector("p_etage", "<p>", "<")),
                    new BoundExpressionGroup("Floors", new BoundSelector("sub>/", "<")),
                    new BoundExpressionGroup("Price", new BoundSelector("add_cost", ">", "<"))
                ));
        }

        public override Selector CreateDetailsSelector()
        {
            return new BoundExpressionSelector("Details", 
                new BoundExpression(new BoundExpressionGroup("Images", true, new BoundSelector("<div class=\"viewport\">", "</div>")),
                                    new BoundExpressionGroup("Description", new BoundSelector("Описание товара<", "text\">", "</p>")),
                                    new BoundExpressionGroup("Coord", true, new BoundSelector("Расположение", "coords:", "\"") {HoldPosition = true}),
                                    new BoundExpressionGroup("Address", new BoundSelector("address_link", ">", "<"))),
                        new BoundExpressionSelector("Preview", "Images", new BoundExpression(new BoundExpressionGroup("Url", new BoundSelector("img src=\"", "\""))))           		
		    );            		
        }

        public override void FillAdDetails(Ad ad, Match match)
        {
            ad.Images = match.GetByPath(@"Images\Preview\Url", true).Select(previewUrl => new AdImage() 
            {
                AdId = ad.Id,
                PreviewUrl = previewUrl.Value,
                Url = previewUrl.Value.Replace("small", "view")
            }).ToList();
            ad.Description = match["Description"];
        }

        public override Ad CreateAd(Match match)
        {
            AdRealty ad = new AdRealty()
            {
                ConnectorId = this.Id,
                BuildingType = Entities.Enums.BuildingType.Flat,
                Title = "",
                Description = "",
                Url = match["DetailUrl"],
                Address = match["Address"],
                RoomsCount = ParsersHelper.ParseInt(match["Rooms"]),
                LivingSpace = ParsersHelper.ParseFloat(match["LivingSpace"], "."),
                PublishDate = ParsersHelper.ParseDate(match["Date"], "HH:mm, dd.MM.yyyy"),
                CreationDate = ParsersHelper.ParseDate(match["Date"], "HH:mm, dd.MM.yyyy"),
                Price = ParsersHelper.ParseDouble(match["Price"]),
                Floor = ParsersHelper.ParseInt(match["Floor"]),
                FloorsCount = ParsersHelper.ParseInt(match["Floors"])
            };

            return ad;
        }
    }
}
