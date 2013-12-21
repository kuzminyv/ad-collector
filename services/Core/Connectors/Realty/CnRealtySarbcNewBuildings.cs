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
            get { return @"http://realty.sarbc.ru/catalog/buy/newbuilding/?page={0}"; }
        }

        public override Ad CreateAd(Match match)
        {
            var ad = (AdRealty)base.CreateAd(match);
            ad.IsNewBuilding = true;
            return ad;
        }
    }
}
