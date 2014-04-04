using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Core.Connectors
{
    public class RealtyVerificator : IVerificator
    {
        public string Verify(Ad ad)
        {
            AdRealty adRealty = (AdRealty)ad;

            if (ad.Price > 0 && (ad.Price < 10000 || ad.Price > 100000000))
                return string.Format("Invalid price value '{0}'", ad.Price);

            if (adRealty.Floor > 0 && (adRealty.Floor > 100 || (adRealty.FloorsCount > 0 && adRealty.Floor > adRealty.FloorsCount)))
                return string.Format("Invalid floor value '{0}/{1}'", adRealty.Floor, adRealty.FloorsCount);

            if (adRealty.FloorsCount > 0 && (adRealty.FloorsCount > 100))
                return string.Format("Invalid FloorsCount value '{0}'", adRealty.FloorsCount);

            if (adRealty.RoomsCount > 0 && (adRealty.RoomsCount > 20))
                return string.Format("Invalid RoomsCount value '{0}'", adRealty.RoomsCount);

            if (adRealty.LivingSpace > 0 && (adRealty.LivingSpace > 300 || adRealty.LivingSpace < 5))
                return string.Format("Invalid LivingSpace value '{0}'", adRealty.LivingSpace);

            if (string.IsNullOrEmpty(adRealty.Address) || adRealty.Address.Length < 3)
                return string.Format("Invalid Address value '{0}'", adRealty.Address);

            if (!Uri.IsWellFormedUriString(adRealty.Url, UriKind.Absolute))
            {
                return string.Format("Invalid Url value '{0}'", adRealty.Url);
            }

            if (string.IsNullOrEmpty(ad.ConnectorId))
            {
                return string.Format("ConnectorId is null or empty.", adRealty.Url);
            }

            if (adRealty.Images != null)
            {
                foreach (var img in adRealty.Images)
                {
                    if ((!string.IsNullOrEmpty(img.Url) && !Uri.IsWellFormedUriString(img.Url, UriKind.Absolute)) ||
                        (!string.IsNullOrEmpty(img.PreviewUrl) && !Uri.IsWellFormedUriString(img.PreviewUrl, UriKind.Absolute)) ||
                        (string.IsNullOrEmpty(img.PreviewUrl) && string.IsNullOrEmpty(img.Url)))
                    {
                        return string.Format("Invalid Image '{0}/{1}'", img.Url, img.PreviewUrl);
                    }
                }
            }
            return null;
        }
    }
}
