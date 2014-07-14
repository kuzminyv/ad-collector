using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Connectors
{
    public class ConnectorOptions
    {
        public int FrameSize { get; set; }
        public int MinAds { get; set;}
        public double MinNewAdsInFrame {get; set;}
        public bool IsSupportedIdOnWebSite { get; set; }

        public ConnectorOptions()
        {
            FrameSize = 100;
            MinAds = 300;
            MinNewAdsInFrame = 0.05;
            IsSupportedIdOnWebSite = false;
        }
    }
}
