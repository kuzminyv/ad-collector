using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Connectors
{
    public class RealtyCorrector : ICorrector
    {
        public void Correct(Ad ad)
        {
            AdRealty adRealty = (AdRealty)ad;
            if (ad.Price > 500 && ad.Price < 50000)
            {
                ad.Price = ad.Price * 1000;
            }

            if (adRealty.Floor > adRealty.FloorsCount)
            {
                var floor = adRealty.Floor;
                adRealty.Floor = adRealty.FloorsCount;
                adRealty.FloorsCount = floor;
            }
        }
    }
}
