using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using System.Text.RegularExpressions;
using Core.Utils;
using System.Security.Policy;
using Core.Expressions;
using System.Linq;
using System.Globalization;

namespace Core.Connectors
{
    [Obsolete]
	public class Cn64AutoRu 
	{
		#region IConnector Members

		public string SourceUrl
		{
			get
			{
				return @"http://www.64-auto.ru";
			}
		}

		private Ad GetAdWithMaxDate(List<Ad> list)
		{
			if (list == null || list.Count == 0)
			{
				return null;
			}

			Ad result = list.First();
			foreach (var item in list)
			{
				if (item.PublishDate > result.PublishDate)
				{
					result = item;
				}
			}
			return result;
		}

		public List<Ad> GetNewAdsList(int maxCount, List<Ad> lastList)
		{
            List<Ad> ads;
			Ad lastAd = GetAdWithMaxDate(lastList);
			List<Ad> result = new List<Ad>();
            int i = 1;

            do
            {
                ads = GetAdsByUrl(String.Format(@"http://www.64-auto.ru/market/?&p={0}", i));
                foreach (var ad in ads)
                {
                    if (lastAd == null || ad.PublishDate > lastAd.PublishDate)
                    {
                        result.Add(ad);
                        if (result.Count == maxCount)
                        {
                            return result;
                        }
                    }
                    else
                    {
                        return result;
                    }
                }
                i++;
            }
            while (ads.Count > 0);

            return result;
		}

		private List<Ad> GetAdsByUrl(string url)
		{
			string content = WebHelper.GetStringFromUrl(url);

			BoundExpression exp = new BoundExpression(
				new BoundExpressionGroup("Price", new BoundSelector("color:#ffffff; white-space:nowrap; padding:2px 5px 0px 5px'>", "</span>")),
				new BoundExpressionGroup("DetailUrl", new BoundSelector("<a href='", "'")),
				new BoundExpressionGroup("Header", new BoundSelector(">", "</a>")),
				new BoundExpressionGroup("Text", new BoundSelector("</h3>", "<small class='artdate'>")),
				new BoundExpressionGroup("Date", 
					new BoundSelector("<nobr><b>", "</b>"),
					new BoundSelector("<nobr>", ", <a")));

			List<Ad> result = new List<Ad>();
			foreach (var match in exp.Matches(content))
			{
				result.Add(new Ad()
				{
					Title     = match["Header"] + " " + match["Price"],
					Description       = match["Text"],
					Url = SourceUrl + match["DetailUrl"],
					PublishDate       = ParseDate(match["Date"]),
					ConnectorId  = this.SourceUrl
				});
			}

			return result;
		}

		private DateTime ParseDate(string date)
		{
			string todayStr     = "сегодня в";
			string yesterdayStr = "вчера в";

			int index = date.IndexOf(todayStr, StringComparison.OrdinalIgnoreCase);
			if (index >= 0)
			{
				string timeStr = date.Remove(index, todayStr.Length).Trim();
				return DateTime.Parse(timeStr);
			}

			index = date.IndexOf(yesterdayStr, StringComparison.OrdinalIgnoreCase);
			if (index >= 0)
			{
				string timeStr = date.Remove(index, todayStr.Length).Trim();
				return DateTime.Parse(timeStr).AddDays(-1);
			}

			return DateTime.ParseExact(date, "d MMMM yyyy", CultureInfo.CreateSpecificCulture("ru-RU"));
		}

		#endregion
	}
}
