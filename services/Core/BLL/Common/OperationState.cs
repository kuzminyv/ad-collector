using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BLL.Common
{
    public class OperationState
    {
        public bool Canceled
        {
            get;
            set;
        }

        public double Progress
        {
            get;
            set;
        }

        public double ProgressTotal
        {
            get;
            set;
        }

        public double ProgressPercentage
        {
            get
            {
                return ProgressTotal == 0 ? 0 : (Progress * 100 / ProgressTotal);
            }
        }

        public string Description
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }
    }
}
