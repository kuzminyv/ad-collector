using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Entities
{
    public class Settings
    {
        public int CheckForNewAdsMaxAdsCount
        {
            get;
            set;
        }

        public int CheckForNewAdsIntervalMinutes
        {
            get;
            set;
        }
    }
}
