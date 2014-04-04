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
            if (ad.Price > 500 && ad.Price < 50000)
            {
                ad.Price = ad.Price * 1000;
            }
        }
    }
}
