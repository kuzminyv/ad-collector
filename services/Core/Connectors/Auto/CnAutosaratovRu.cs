using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using System.Text.RegularExpressions;
using Core.Utils;
using System.Linq;
using Core.Expressions;
using System.Globalization;

namespace Core.Connectors
{
    [Obsolete]
	public class CnAutosaratovRu 
	{
		public string SourceUrl
		{
			get
			{
				return "http://www.autosaratov.ru";
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
			Ad lastAd = GetAdWithMaxDate(lastList);
			List<Ad> result = new List<Ad>();

			for (int i = 1; true; i++)
			{
				List<Ad> ads = GetAdsByUrl(String.Format(@"http://www.autosaratov.ru/vehicle/browse/cars/index{0}.html", i));
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
			}
		}

		private List<Ad> GetAdsByUrl(string url)
		{
			string content = WebHelper.GetStringFromUrl(url);

			BoundExpression exp = new BoundExpression(
				new BoundExpressionGroup("DetailsUrl", new BoundSelector("<div class=\"field\">Марка:</div>", "href=\"", "\">")),
				new BoundExpressionGroup("CarModel", new BoundSelector("<b>", "</b>")),
				new BoundExpressionGroup("CarYear", new BoundSelector("<div class=\"field\">Год выпуска:</div>", "<div class=\"value\">", "</div>")),
				new BoundExpressionGroup("CarRun", new BoundSelector("<div class=\"field\">Пробег:</div>", "<div class=\"value\">", "</div>")),
				new BoundExpressionGroup("CarPrice", new BoundSelector("<div class=\"field\">Цена:</div>", "<div class=\"value\">", "</div>")),
				new BoundExpressionGroup("Date", new BoundSelector("<div class=\"field\">Размещено:</div>", "<div class=\"value\">", "</div>")));

			List<Ad> result = new List<Ad>();
			foreach (var match in exp.Matches(content))
			{
				result.Add(new Ad()
				{
					Title = String.Format("Продаю {0} {1}", match["CarModel"], match["CarPrice"]),
					Description = String.Format("{0} {1}, {2} года выпуска, пробег {3}", match["CarModel"], match["CarPrice"], match["CarYear"], match["CarRun"]),
					Url = match["DetailsUrl"],
					PublishDate = ParseDate(match["Date"]),
					ConnectorId = this.SourceUrl
				});
			}

			return result;
		}

		private DateTime ParseDate(string date)
		{
			int delimitterIndex = date.IndexOf("в");
			string dateStr = date.Substring(0, delimitterIndex).Trim();
			string timeStr = date.Substring(delimitterIndex + 1).Trim();

			DateTime d = DateTime.ParseExact(dateStr, "dd.MM.yyyy", null);
			DateTime t = DateTime.Parse(timeStr);
			d = d.AddHours(t.Hour);
			d = d.AddMinutes(t.Minute);

			return d;
		}
	}
}
