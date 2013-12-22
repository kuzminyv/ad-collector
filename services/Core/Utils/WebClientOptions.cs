using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class WebClientOptions
    {
        public bool IgnoreError404
        {
            get;
            set;
        }

        public static WebClientOptions Default
        {
            get
            {
                return new WebClientOptions();
            }
        }
    }
}
